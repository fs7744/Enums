using SV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UT
{
    [Flags]
    public enum Fruits2
    {
        Apple = 1,
        Lemon = 2,
        Melon = 4,
        Banana = 8
    }

    public enum FruitsInt : int
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public enum FruitsByte : byte
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public enum FruitsSByte : sbyte
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public enum FruitsShort : short
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public enum FruitsUShort : ushort
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public enum FruitsLong : long
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public enum FruitsULong : ulong
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public enum FruitsUInt : uint
    {
        Apple,
        Lemon,
        Melon,
        Banana,
        Lemon1,
        Melon2,
        Banana3,
        Lemon11,
        Melon21,
        Banana31,
        Lemon12,
        Melon22,
        Banana32,
        Lemon13,
        Melon23,
        Banana33,
        Lemon131,
        Melon231,
        Banana331,
        Lemon14,
        Melon24,
        Banana34,
    }

    public class EnumTest
    {
        private Dictionary<Type, object> EnumInfos = new Dictionary<Type, object>()
        {
            { typeof(FruitsByte), new EnumInfo<FruitsByte>() },
            { typeof(FruitsSByte), new EnumInfo<FruitsSByte>() },
            { typeof(FruitsShort), new EnumInfo<FruitsShort>() },
            { typeof(FruitsUShort), new EnumInfo<FruitsUShort>() },
            { typeof(FruitsLong), new EnumInfo<FruitsLong>() },
            { typeof(FruitsULong), new EnumInfo<FruitsULong>() },
            { typeof(FruitsUInt), new EnumInfo<FruitsUInt>() },
            { typeof(FruitsInt), new EnumInfo<FruitsInt>() },
            { typeof(Fruits2), new EnumInfo<Fruits2>() }
        };

        public IEnumInfo<T> GetEnumInfo<T>() where T : struct, Enum
        {
            if (!EnumInfos.TryGetValue(typeof(T), out var info))
            {
                info = new EnumInfo<T>();
            }
            return (IEnumInfo<T>)info;
        }

        [Fact]
        public void UnderlyingTypeShouldBeSame()
        {
            void CheckUnderlyingTypeShouldBeSame<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var ut = Enum.GetUnderlyingType(t);
                var typecode = Type.GetTypeCode(t);
                Assert.Same(ut, Enums<T>.GetUnderlyingType());
                Assert.Same(ut, GetEnumInfo<T>().GetUnderlyingType());
                Assert.Equal(typecode, Enums<T>.GetUnderlyingTypeCode());
                Assert.Equal(typecode, GetEnumInfo<T>().GetUnderlyingTypeCode());
            }
            CheckUnderlyingTypeShouldBeSame<FruitsByte>();
            CheckUnderlyingTypeShouldBeSame<FruitsSByte>();
            CheckUnderlyingTypeShouldBeSame<FruitsShort>();
            CheckUnderlyingTypeShouldBeSame<FruitsUShort>();
            CheckUnderlyingTypeShouldBeSame<FruitsLong>();
            CheckUnderlyingTypeShouldBeSame<FruitsULong>();
            CheckUnderlyingTypeShouldBeSame<FruitsUInt>();
            CheckUnderlyingTypeShouldBeSame<FruitsInt>();
            CheckUnderlyingTypeShouldBeSame<Fruits2>();
            CheckUnderlyingTypeShouldBeSame<TypeCode>();
        }

        [Fact]
        public void ValuesShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var vs = Enum.GetValues(t);
                var vs0 = Enums<T>.GetValues();
                var vs1 = GetEnumInfo<T>().GetValues();
                Assert.Equal(vs.Length, vs0.Length);
                Assert.Equal(vs.Length, vs1.Length);
                foreach (var v in vs)
                {
                    Assert.True(vs0.Any(i => i.Equals(v)));
                    Assert.True(vs1.Any(i => i.Equals(v)));
                }
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void NamesShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var vs = Enum.GetNames(t);
                var vs0 = Enums<T>.GetNames();
                var vs1 = GetEnumInfo<T>().GetNames();
                Assert.Equal(vs.Length, vs0.Length);
                Assert.Equal(vs.Length, vs1.Length);
                foreach (var v in vs)
                {
                    Assert.True(vs0.Any(i => i.Equals(v)));
                    Assert.True(vs1.Any(i => i.Equals(v)));
                }
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void GetNameShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                T[] vs = (T[])Enum.GetValues(t);

                foreach (var v in vs)
                {
                    var n = Enum.GetName<T>(v);
                    Assert.Equal(n, Enums<T>.GetName(v));
                    Assert.Equal(n, GetEnumInfo<T>().GetName(v));
                }
                Assert.False(Enums<T>.TryParse("xxxxx", out var vv));
                Assert.False(GetEnumInfo<T>().TryParse("xxxxx", out vv));
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void ParseShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var vs = Enum.GetNames(t);

                foreach (var v in vs)
                {
                    var n = Enum.Parse<T>(v);
                    Assert.Equal(n, Enums<T>.Parse(v));
                    Assert.True(GetEnumInfo<T>().TryParse(v, out var vv));
                    Assert.Equal(n, vv);
                }
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void ParseIngoreCaseShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var vs = Enum.GetNames(t).Select(i => i.ToLower());

                foreach (var v in vs)
                {
                    var n = Enum.Parse<T>(v, true);
                    Assert.Equal(n, Enums<T>.Parse(v, true));
                    Assert.True(GetEnumInfo<T>().TryParse(v, true, out var vv));
                    Assert.Equal(n, vv);
                }
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void ParseCaseShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var vs = Enum.GetNames(t);

                foreach (var v in vs)
                {
                    var n = Enum.Parse<T>(v, false);
                    Assert.Equal(n, Enums<T>.Parse(v, false));
                    Assert.True(GetEnumInfo<T>().TryParse(v, false, out var vv));
                    Assert.Equal(n, vv);
                }
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void IsDefinedShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var vs = Enum.GetNames(t);

                foreach (var v in vs)
                {
                    Assert.True(Enums<T>.IsDefined(v));
                    Assert.True(GetEnumInfo<T>().IsDefined(v));
                }
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void IsFlagsShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                var vs = t.IsDefined(typeof(FlagsAttribute), true);

                Assert.Equal(vs, Enums<T>.IsFlags);
                Assert.Equal(vs, GetEnumInfo<T>().IsFlags);
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }

        [Fact]
        public void ToEnumShouldBeSame()
        {
            void Check<T>() where T : struct, Enum
            {
                var t = typeof(T);
                T[] vs = (T[])Enum.GetValues(t);

                foreach (var v in vs)
                {
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToInt16(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToInt32(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToInt64(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToUInt16(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToUInt32(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToUInt64(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToByte(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToSByte(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToChar(v)));
                    Assert.Equal(v, Enums<T>.ToEnum(v.ToString()));
                    Assert.Equal(v, Enums<T>.ToEnum(Convert.ToInt64(v).ToString()));
                    Assert.Equal(v, Enums<T>.ToEnum((object)v));
                }
            }
            Check<FruitsByte>();
            Check<FruitsSByte>();
            Check<FruitsShort>();
            Check<FruitsUShort>();
            Check<FruitsLong>();
            Check<FruitsULong>();
            Check<FruitsUInt>();
            Check<FruitsInt>();
            Check<Fruits2>();
            Check<TypeCode>();
        }
    }
}