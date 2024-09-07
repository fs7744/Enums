using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SV
{
    public interface IEnumInfo<T> where T : struct, Enum
    {
        public bool TryParse(string name, bool ignoreCase, out T result);

        public bool TryParse(string name, out T result);

        Type GetUnderlyingType();

        TypeCode GetUnderlyingTypeCode();

        string GetName(T t);

        ImmutableArray<string> GetNames();

        ImmutableArray<T> GetValues();

        bool IsDefined(string name);

        EnumMemberAttribute GetEnumMember(T t);

        IReadOnlyDictionary<int, string> GetLabels(T t);

        string GetLabel(T t, int index);

        bool IsFlags { get; }

        bool IsEmpty { get; }
    }

    public abstract class EnumBase<T> : IEnumInfo<T> where T : struct, Enum
    {
        protected readonly Type underlyingType;
        protected readonly TypeCode underlyingTypeCode;
        private string[] names;
        private readonly T[] values;
        private readonly FastReadOnlyDictionary<T, (string Name, EnumMemberAttribute Member, FastReadOnlyDictionary<int, string> Labels)> namesByMember;
        public bool IsFlags { get; private set; }
        public bool IsEmpty => values.Length == 0;

        public EnumBase()
        {
            var t = typeof(T);
            underlyingType = Enum.GetUnderlyingType(t);
            underlyingTypeCode = Type.GetTypeCode(underlyingType);
            names = Enum.GetNames(t);
            values = names.Select(i => (T)Enum.Parse(t, i)).ToArray();
            IsFlags = t.IsDefined(typeof(FlagsAttribute), true);
            namesByMember = names.Select(i => (Key: i, Value: (T)Enum.Parse(t, i))).DistinctBy(i => i.Value).AsEnumerable().DistinctBy(i => i.Value).ToFastReadOnlyDictionary(i => i.Value, i =>
            {
                var fieldInfo = t.GetField(i.Key)!;
                return (i.Key, fieldInfo.GetCustomAttribute<EnumMemberAttribute>(), fieldInfo.GetCustomAttributes<LabelAttribute>().DistinctBy(i => i.Index).ToFastReadOnlyDictionary(x => x.Index, x => x.Value));
            });
        }

        public Type GetUnderlyingType() => underlyingType;

        public TypeCode GetUnderlyingTypeCode() => underlyingTypeCode;

        protected abstract bool TryParseIgnoreCase(in ReadOnlySpan<char> name, out T result);

        protected abstract bool TryParseCase(in ReadOnlySpan<char> name, out T result);

        public abstract string GetName(T t);

        public abstract bool IsDefined(string name);

        public EnumMemberAttribute GetEnumMember(T t)
        {
            return namesByMember.TryGetValue(t, out var r) ? r.Member : null;
        }

        public IReadOnlyDictionary<int, string> GetLabels(T t)
        {
            return namesByMember.TryGetValue(t, out var r) ? r.Labels : null;
        }

        public string GetLabel(T t, int index)
        {
            return namesByMember.TryGetValue(t, out var r) && r.Labels.TryGetValue(index, out var rr) ? rr : null;
        }

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
            if (Enums.TryParseUnderlyingTypeString<T>(underlyingTypeCode, name, out result))
                return true;
            return false;
        }

        public bool TryParse(string name, out T result)
        {
            if (TryParseCase(name.AsSpan(), out result))
            {
                return true;
            }
            if (Enums.TryParseUnderlyingTypeString<T>(underlyingTypeCode, name, out result))
                return true;
            return false;
        }
    }

    public static partial class Enums
    {
#if NETCOREAPP3_0_OR_GREATER
        internal const MethodImplOptions Optimization = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;
#else
        internal const MethodImplOptions Optimization = MethodImplOptions.AggressiveInlining;
#endif

        public static void SetEnumInfo<T>(IEnumInfo<T> enumInfo) where T : struct, Enum
        {
            Enums<T>.Info = enumInfo;
        }

        internal static bool TryParseUnderlyingTypeString<T>(TypeCode underlyingTypeCode, string value, out T result) where T : struct, Enum
        {
            switch (underlyingTypeCode)
            {
                case TypeCode.Byte:
                    {
                        if (Byte.TryParse(value, out var v))
                        {
                            result = Unsafe.As<Byte, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.Int16:
                    {
                        if (short.TryParse(value, out var v))
                        {
                            result = Unsafe.As<short, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.Int32:
                    {
                        if (int.TryParse(value, out var v))
                        {
                            result = Unsafe.As<int, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.Int64:
                    {
                        if (long.TryParse(value, out var v))
                        {
                            result = Unsafe.As<long, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.SByte:
                    {
                        if (sbyte.TryParse(value, out var v))
                        {
                            result = Unsafe.As<sbyte, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.Single:
                    {
                        if (Single.TryParse(value, out var v))
                        {
                            result = Unsafe.As<Single, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.UInt16:
                    {
                        if (UInt16.TryParse(value, out var v))
                        {
                            result = Unsafe.As<UInt16, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.UInt32:
                    {
                        if (UInt32.TryParse(value, out var v))
                        {
                            result = Unsafe.As<UInt32, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.UInt64:
                    {
                        if (UInt32.TryParse(value, out var v))
                        {
                            result = Unsafe.As<UInt32, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.Char:
                    {
                        if (Char.TryParse(value, out var v))
                        {
                            result = Unsafe.As<Char, T>(ref v);
                            return true;
                        }
                    }
                    break;

                case TypeCode.Boolean:
                    {
                        if (Boolean.TryParse(value, out var v))
                        {
                            var vv = v ? 1L : 0L;
                            result = Unsafe.As<long, T>(ref vv);
                            return true;
                        }
                    }
                    break;

                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.String:
                case TypeCode.Empty:
                case TypeCode.Object:
                default:
                    break;
            }

            result = default;
            return false;
        }
    }

    public static class Enums<T> where T : struct, Enum
    {
        public static bool IsFlags => CheckInfo().IsFlags;
        public static bool IsEmpty => CheckInfo().IsEmpty;

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

        public static T Parse(string name, bool ignoreCase)
        {
            if (CheckInfo().TryParse(name, ignoreCase, out var result))
                return result;
            throw new ArgumentException($"Specified value '{name}' is not defined.", nameof(name));
        }

        public static T Parse(string name)
        {
            if (CheckInfo().TryParse(name, out var result))
                return result;
            throw new ArgumentException($"Specified value '{name}' is not defined.", nameof(name));
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

        public static bool IsDefined(string name)
        {
            return CheckInfo().IsDefined(name);
        }

        public static Type GetUnderlyingType() => CheckInfo().GetUnderlyingType();

        public static TypeCode GetUnderlyingTypeCode() => CheckInfo().GetUnderlyingTypeCode();

        public static EnumMemberAttribute GetEnumMember(T t)
        {
            return CheckInfo().GetEnumMember(t);
        }

        public static IReadOnlyDictionary<int, string> GetLabels(T t)
        {
            return CheckInfo().GetLabels(t);
        }

        public static string GetLabel(T t, int index)
        {
            return CheckInfo().GetLabel(t, index);
        }

        public static T ToEnum(int value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    return Unsafe.As<int, T>(ref value);

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<byte, T>(ref v);
                    }
                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(byte value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    return Unsafe.As<byte, T>(ref value);

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(char value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    return Unsafe.As<Char, T>(ref value);

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(Int16 value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(Int64 value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(SByte value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(UInt16 value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(UInt32 value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(UInt64 value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(bool value)
        {
            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(object value)
        {
            if (value is string s)
            {
                return ToEnum(s);
            }

            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }

        public static T ToEnum(string value)
        {
            if (TryParse(value, out T result))
                return result;

            switch (GetUnderlyingTypeCode())
            {
                case TypeCode.Int32:
                    {
                        var v = Convert.ToInt32(value);
                        return Unsafe.As<Int32, T>(ref v);
                    }

                case TypeCode.Byte:
                    {
                        var v = Convert.ToByte(value);
                        return Unsafe.As<Byte, T>(ref v);
                    }

                case TypeCode.Char:
                    {
                        var v = Convert.ToChar(value);
                        return Unsafe.As<Char, T>(ref v);
                    }

                case TypeCode.Int16:
                    {
                        var v = Convert.ToInt16(value);
                        return Unsafe.As<Int16, T>(ref v);
                    }

                case TypeCode.Int64:
                    {
                        var v = Convert.ToInt64(value);
                        return Unsafe.As<Int64, T>(ref v);
                    }

                case TypeCode.SByte:
                    {
                        var v = Convert.ToSByte(value);
                        return Unsafe.As<SByte, T>(ref v);
                    }

                case TypeCode.UInt16:
                    {
                        var v = Convert.ToUInt16(value);
                        return Unsafe.As<UInt16, T>(ref v);
                    }

                case TypeCode.UInt32:
                    {
                        var v = Convert.ToUInt32(value);
                        return Unsafe.As<UInt32, T>(ref v);
                    }

                case TypeCode.UInt64:
                    {
                        var v = Convert.ToUInt64(value);
                        return Unsafe.As<UInt64, T>(ref v);
                    }

                default:
                    throw new InvalidOperationException();
            }
        }
    }

    public class EnumInfo<T> : IEnumInfo<T> where T : struct, Enum
    {
        private readonly string[] names;
        private readonly T[] values;
        private readonly (string Name, T Value)[] members;
        private readonly FastReadOnlyDictionary<string, T> membersByName;
        private readonly FastReadOnlyDictionary<T, (string Name, EnumMemberAttribute Member, FastReadOnlyDictionary<int, string> Labels)> namesByMember;
#if NET7_0_OR_GREATER
        private readonly ReadOnlyOrdinalIgnoreCaseStringDictionary<T> membersByNameOrdinalIgnoreCase;
#endif
        private readonly Type underlyingType;
        private readonly TypeCode underlyingTypeCode;

        public bool IsFlags { get; private set; }
        public bool IsEmpty => values.Length == 0;

        public EnumInfo() : base()
        {
            var t = typeof(T);
            names = Enum.GetNames(t);
            members = names.Select(i => (i, (T)Enum.Parse(t, i))).ToArray();
            values = members.Select(i => i.Value).ToArray();
            membersByName = members.ToFastReadOnlyDictionary(i => i.Name, i => i.Value);
            namesByMember = membersByName.AsEnumerable().DistinctBy(i => i.Value).ToFastReadOnlyDictionary(i => i.Value, i =>
            {
                var fieldInfo = t.GetField(i.Key)!;
                return (i.Key, fieldInfo.GetCustomAttribute<EnumMemberAttribute>(), fieldInfo.GetCustomAttributes<LabelAttribute>().DistinctBy(i => i.Index).ToFastReadOnlyDictionary(x => x.Index, x => x.Value));
            });
#if NET7_0_OR_GREATER
            membersByNameOrdinalIgnoreCase = membersByName.AllKV().ToReadOnlyOrdinalIgnoreCaseStringDictionary();
#endif
            underlyingType = Enum.GetUnderlyingType(t);
            underlyingTypeCode = Type.GetTypeCode(underlyingType);
            IsFlags = t.IsDefined(typeof(FlagsAttribute), true);
        }

        [MethodImpl(Enums.Optimization)]
        public bool TryParseIgnoreCase(in ReadOnlySpan<char> name, out T result)
        {
#if NET7_0_OR_GREATER
            return membersByNameOrdinalIgnoreCase.TryGetValueSpan(name, out result);
#else
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
#endif
        }

        public bool TryParse(string name, bool ignoreCase, out T result)
        {
            if (ignoreCase ? TryParseIgnoreCase(name.AsSpan(), out result) : membersByName.TryGetValue(name, out result))
            {
                return true;
            }
            if (Enums.TryParseUnderlyingTypeString<T>(underlyingTypeCode, name, out result))
                return true;
            return false;
        }

        public bool TryParse(string name, out T result)
        {
            if (membersByName.TryGetValue(name, out result))
                return true;
            if (Enums.TryParseUnderlyingTypeString<T>(underlyingTypeCode, name, out result))
                return true;
            return false;
        }

        [MethodImpl(Enums.Optimization)]
        public string GetName(T t)
        {
            return namesByMember.TryGetValue(t, out var r) ? r.Name : null;
        }

        public EnumMemberAttribute GetEnumMember(T t)
        {
            return namesByMember.TryGetValue(t, out var r) ? r.Member : null;
        }

        public IReadOnlyDictionary<int, string> GetLabels(T t)
        {
            return namesByMember.TryGetValue(t, out var r) ? r.Labels : null;
        }

        public string GetLabel(T t, int index)
        {
            return namesByMember.TryGetValue(t, out var r) && r.Labels.TryGetValue(index, out var rr) ? rr : null;
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

        public TypeCode GetUnderlyingTypeCode() => underlyingTypeCode;

        public bool IsDefined(string name)
        {
            return membersByName.TryGetValue(name, out var r);
        }
    }
}