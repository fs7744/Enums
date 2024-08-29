using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
    }

    public static class Enums
    {
        internal static readonly FrozenDictionary<Type, IEnumUnderlyingTypeInfo> EnumUnderlyingTypeInfos = new Dictionary<Type, IEnumUnderlyingTypeInfo>()
        {
            { typeof(int),  new Int32EnumUnderlyingTypeInfo() }
        }.ToFrozenDictionary();

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
    }

    internal class EnumInfo<T> : IEnumInfo<T> where T : struct, Enum
    {
        private readonly (string Name, T Value)[] members;
        private readonly IEnumUnderlyingTypeInfo enumUnderlyingTypeInfo;
        private readonly Type underlyingType;

        public EnumInfo()
        {
            var t = typeof(T);
            var names = Enum.GetNames(t);
            members = names.Select(i => (i, (T)Enum.Parse(t, i))).ToArray();
            underlyingType = Enum.GetUnderlyingType(t);
            enumUnderlyingTypeInfo = Enums.EnumUnderlyingTypeInfos[underlyingType];
        }

        [MethodImpl(Enums.Optimization)]
        public bool TryParseIgnoreCase(ReadOnlySpan<char> name, out T result)
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

        [MethodImpl(Enums.Optimization)]
        private bool TryParseInternal(ReadOnlySpan<char> name, out T result)
        {
            foreach (var member in members.AsSpan())
            {
                if (name.Equals(member.Name.AsSpan(), StringComparison.Ordinal))
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
            if (ignoreCase ? TryParseIgnoreCase(name.AsSpan(), out result) : TryParseInternal(name.AsSpan(), out result))
            {
                return true;
            }
            if (enumUnderlyingTypeInfo.TryParse(name, out result))
                return true;
            return false;
        }

        public bool TryParse(string name, out T result)
        {
            if (TryParseInternal(name.AsSpan(), out result))
            {
                return true;
            }
            if (enumUnderlyingTypeInfo.TryParse(name, out result))
                return true;
            return false;
        }

        public Type GetUnderlyingType() => underlyingType;
    }
}