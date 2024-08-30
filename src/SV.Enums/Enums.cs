using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SV
{
    public interface IEnumUnderlyingTypeInfo
    {
        public bool TryParse<T>(string name, out T result) where T : struct, Enum;
    }

    public class Int32EnumUnderlyingTypeInfo : IEnumUnderlyingTypeInfo
    {
        public bool TryParse<T>(string name, out T result) where T : struct, Enum
        {
            if (int.TryParse(name, out var v))
            {
                result = Unsafe.As<int, T>(ref v);
                return true;
            }
            result = default;
            return false;
        }
    }

    public interface IEnumInfo<T> where T : struct, Enum
    {
        public bool TryParse(string name, bool ignoreCase, out T result);

        public bool TryParse(string name, out T result);

        Type GetUnderlyingType();

        string GetName(T t);

        ImmutableArray<string> GetNames();

        ImmutableArray<T> GetValues();
    }

    public abstract class EnumBase<T> : IEnumInfo<T> where T : struct, Enum
    {
        private readonly Type underlyingType;
        private string[] names;
        private readonly T[] values;

        public EnumBase()
        {
            var t = typeof(T);
            underlyingType = Enum.GetUnderlyingType(t);
            names = Enum.GetNames(t);
            values = names.Select(i => (T)Enum.Parse(t, i)).ToArray();
        }

        public Type GetUnderlyingType() => underlyingType;

        protected abstract bool TryParseIgnoreCase(in ReadOnlySpan<char> name, out T result);

        protected abstract bool TryParseCase(in ReadOnlySpan<char> name, out T result);

        protected abstract bool TryParseUnderlyingTypeString(string value, out T result);

        public abstract string GetName(T t);

        public ImmutableArray<string> GetNames()
        {
            return ImmutableCollectionsMarshal.AsImmutableArray(names);
        }

        public ImmutableArray<T> GetValues()
        {
            return ImmutableCollectionsMarshal.AsImmutableArray(values);
        }

        public bool TryParse(string name, bool ignoreCase, out T result)
        {
            if (ignoreCase ? TryParseIgnoreCase(name.AsSpan(), out result) : TryParseCase(name.AsSpan(), out result))
            {
                return true;
            }
            if (TryParseUnderlyingTypeString(name, out result))
                return true;
            return false;
        }

        public bool TryParse(string name, out T result)
        {
            if (TryParseCase(name.AsSpan(), out result))
            {
                return true;
            }
            if (TryParseUnderlyingTypeString(name, out result))
                return true;
            return false;
        }
    }

    public static class Enums
    {
        internal static readonly IReadOnlyDictionary<Type, IEnumUnderlyingTypeInfo> EnumUnderlyingTypeInfos = new FastReadOnlyDictionary<Type, IEnumUnderlyingTypeInfo>(new KeyValuePair<Type, IEnumUnderlyingTypeInfo>[]
        {
            new KeyValuePair<Type, IEnumUnderlyingTypeInfo>(typeof(int),  new Int32EnumUnderlyingTypeInfo() )
        });

#if NETCOREAPP3_0_OR_GREATER
        internal const MethodImplOptions Optimization = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
#else
        internal const MethodImplOptions Optimization = MethodImplOptions.AggressiveInlining;
#endif

        public static void SetEnumInfo<T>(IEnumInfo<T> enumInfo) where T : struct, Enum
        {
            Enums<T>.Info = enumInfo;
        }
    }

    public static class Enums<T> where T : struct, Enum
    {
        internal static IEnumInfo<T> Info;

        [MethodImpl(Enums.Optimization)]
        internal static IEnumInfo<T> CheckInfo()
        {
            if (Info == null)
            {
                Info = new EnumInfo<T>();
            }
            return Info;
        }

        public static bool TryParse(string name, bool ignoreCase, out T result)
        {
            return CheckInfo().TryParse(name, ignoreCase, out result);
        }

        public static bool TryParse(string name, out T result)
        {
            return CheckInfo().TryParse(name, out result);
        }

        public static string GetName(T t)
        {
            return CheckInfo().GetName(t);
        }

        public static ImmutableArray<string> GetNames()
        {
            return CheckInfo().GetNames();
        }

        public static ImmutableArray<T> GetValues()
        {
            return CheckInfo().GetValues();
        }
    }

    public class EnumInfo<T> : IEnumInfo<T> where T : struct, Enum
    {
        private readonly string[] names;
        private readonly T[] values;
        private readonly (string Name, T Value)[] members;
        private readonly IReadOnlyDictionary<string, T> membersByName;
        private readonly IReadOnlyDictionary<T, string> namesByMember;
        private readonly IEnumUnderlyingTypeInfo enumUnderlyingTypeInfo;
        private readonly Type underlyingType;

        public EnumInfo()
        {
            var t = typeof(T);
            names = Enum.GetNames(t);
            members = names.Select(i => (i, (T)Enum.Parse(t, i))).ToArray();
            values = members.Select(i => i.Value).ToArray();
            membersByName = members.ToFastReadOnlyDictionary(i => i.Name, i => i.Value);
            namesByMember = membersByName.AsEnumerable().ToFastReadOnlyDictionary(i => i.Value, i => i.Key);
            underlyingType = Enum.GetUnderlyingType(t);
            enumUnderlyingTypeInfo = Enums.EnumUnderlyingTypeInfos[underlyingType];
        }

        [MethodImpl(Enums.Optimization)]
        public bool TryParseIgnoreCase(in ReadOnlySpan<char> name, out T result)
        {
            foreach (var member in members.AsSpan())
            {
                if (name.Equals(member.Name.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    result = member.Value;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public bool TryParse(string name, bool ignoreCase, out T result)
        {
            if (ignoreCase ? TryParseIgnoreCase(name.AsSpan(), out result) : membersByName.TryGetValue(name, out result))
            {
                return true;
            }
            if (enumUnderlyingTypeInfo.TryParse(name, out result))
                return true;
            return false;
        }

        public bool TryParse(string name, out T result)
        {
            if (membersByName.TryGetValue(name, out result))
                return true;
            if (enumUnderlyingTypeInfo.TryParse(name, out result))
                return true;
            return false;
        }

        public string GetName(T t)
        {
            return namesByMember.TryGetValue(t, out var name) ? name : null;
        }

        public ImmutableArray<string> GetNames()
        {
            return ImmutableCollectionsMarshal.AsImmutableArray(names);
        }

        public ImmutableArray<T> GetValues()
        {
            return ImmutableCollectionsMarshal.AsImmutableArray(values);
        }

        public Type GetUnderlyingType() => underlyingType;
    }
}