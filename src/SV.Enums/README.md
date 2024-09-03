# Enums
Support Source Generator or no Source Generator to help you use enum fast

����Ŀ��Ҫ��̽������� enum ����Ч

�����漰���Ż��ֶβ�����ȫ�Դ�

�ܶ����ݲο���������Ŀ 

- [NetEscapades.EnumGenerators](https://github.com/andrewlock/NetEscapades.EnumGenerators)
- [FastEnum](https://github.com/xin9le/FastEnum)
- [runtime](https://github.com/dotnet/runtime)

# ��Ҫ�Ż��ֶ�

��ʵ��Ҫȫ�� �ռ任ʱ�䣬��������


## ��װ��ڷ����Լ� source-generators ����

��������Ŀ�����˷�װ��ڷ�����[ModuleInitializer](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.moduleinitializerattribute?view=net-8.0)��[source-generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) �������ʹ��Ӱ�죨��ʵ����Ҫ�ǳ�����α���ʹ��[interceptors](https://github.com/dotnet/roslyn/blob/main/docs/features/interceptors.md)��

``` csharp
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
```

��������Ҫ�Ϳ������� [source-generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) ����enum ������룬

��ͨ�� [ModuleInitializer](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.moduleinitializerattribute?view=net-8.0) ������ʱ�������ɵ�enum����

����Ϊ����enum ����ʾ��

``` csharp
internal class EnumInfoAD125120120540FC9AA056E2DD394A7C : EnumBase<global::Benchmark.Fruits2>
    {
        public override bool IsDefined(string name)
        {
            return name switch
            {
                nameof(global::Benchmark.Fruits2.Apple) => true,
nameof(global::Benchmark.Fruits2.Lemon) => true,
nameof(global::Benchmark.Fruits2.Melon) => true,
nameof(global::Benchmark.Fruits2.Banana) => true,
                _ => false,
            };
        }

        public override string? GetName(global::Benchmark.Fruits2 t)
        {
            switch (t)
            {
                case global::Benchmark.Fruits2.Apple: return nameof(global::Benchmark.Fruits2.Apple);
case global::Benchmark.Fruits2.Lemon: return nameof(global::Benchmark.Fruits2.Lemon);
case global::Benchmark.Fruits2.Melon: return nameof(global::Benchmark.Fruits2.Melon);
case global::Benchmark.Fruits2.Banana: return nameof(global::Benchmark.Fruits2.Banana);
                default:
                    return null;
            }
        }

        protected override bool TryParseCase(in ReadOnlySpan<char> name, out global::Benchmark.Fruits2 result)
        {
            switch (name)
            {
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Apple).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits2.Apple;
                    return true;
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Lemon).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits2.Lemon;
                    return true;
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Melon).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits2.Melon;
                    return true;
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Banana).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits2.Banana;
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        protected override bool TryParseIgnoreCase(in ReadOnlySpan<char> name, out global::Benchmark.Fruits2 result)
        {
            switch (name)
            {
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Apple).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits2.Apple;
                    return true;
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Lemon).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits2.Lemon;
                    return true;
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Melon).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits2.Melon;
                    return true;
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits2.Banana).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits2.Banana;
                    return true;
                default:
                    result = default;
                    return false;
            }
        }
    }

        internal static partial class EnumsF1029F0E5915401BBDD8559E2B5289B1
    {
        [ModuleInitializer]
        internal static void Init4771B8A4BD2E4761973279D81E61089C()
        {
            global::SV.Enums.SetEnumInfo<global::Benchmark.Fruits2>(new EnumInfoAD125120120540FC9AA056E2DD394A7C());
        }
    }
```

����������װ��ڣ�����һ��������ʧ

## �ռ任ʱ��

��Ȼ�����ʹ�� [source-generators](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)�� ��Ӧ����Ҳ��Ĭ��ʵ��

���ִ������£� �󲿷ֶ������ڴ滺����

``` csharp
    public class EnumInfo<T> : IEnumInfo<T> where T : struct, Enum
    {
        private readonly string[] names;
        private readonly T[] values;
        private readonly (string Name, T Value)[] members;
        private readonly FastReadOnlyDictionary<string, T> membersByName;
        private readonly FastReadOnlyDictionary<T, (string Name, EnumMemberAttribute Member, FastReadOnlyDictionary<int, string> Labels)> namesByMember;
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
            underlyingType = Enum.GetUnderlyingType(t);
            underlyingTypeCode = Type.GetTypeCode(underlyingType);
            IsFlags = t.IsDefined(typeof(FlagsAttribute), true);
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
``` 

## enum ת������

ͬʱ�ṩһЩ ���ò���װ��� enum ת�������������Ƴ������ͼ����߼�����������ֻ�ܱ�֤����ʹ�ò���������

``` csharp
public static T ToEnum(int value)

public static T ToEnum(byte value)

public static T ToEnum(Int16 value)

public static T ToEnum(Int64 value)

...
```

# ���ܲ���

���������ܲ��ԣ� ���ִ�������

``` csharp
public enum Fruits
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

    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class EnumBenchmarks
    {
        private readonly EnumInfo<Fruits> test;

        public EnumBenchmarks()
        {
            test = new EnumInfo<Fruits>();
        }

        [Benchmark(Baseline = true), BenchmarkCategory("IgnoreCase")]
        public Fruits ParseIgnoreCase()
        {
            return Enum.Parse<Fruits>("melon", true);
        }

        [Benchmark, BenchmarkCategory("IgnoreCase")]
        public Fruits FastEnumParseIgnoreCase()
        {
            return FastEnum.Parse<Fruits>("melon", true);
        }

        [Benchmark, BenchmarkCategory("IgnoreCase")]
        public Fruits SVEnumsParseIgnoreCase()
        {
            Enums<Fruits>.TryParse("melon", true, out var v);
            return v;
        }

        [Benchmark, BenchmarkCategory("IgnoreCase")]
        public Fruits EnumInfoParseIgnoreCase()
        {
            test.TryParse("melon", true, out var v);
            return v;
        }

        [Benchmark(Baseline = true)]
        public Fruits Parse()
        {
            return Enum.Parse<Fruits>("Melon", false);
        }

        [Benchmark]
        public Fruits FastEnumParse()
        {
            return FastEnum.Parse<Fruits>("Melon", false);
        }

        [Benchmark]
        public Fruits SVEnumsParse()
        {
            Enums<Fruits>.TryParse("Melon", out var v);
            return v;
        }

        [Benchmark]
        public Fruits EnumInfoParse()
        {
            test.TryParse("Melon", false, out var v);
            return v;
        }

        ...

    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class ToEnumBenchmarks
    {
        private readonly EnumInfo<Fruits> test;

        public ToEnumBenchmarks()
        {
            test = new EnumInfo<Fruits>();
            //Enums.SetEnumInfo<Fruits>(new TestIEnumInfo());
        }

        [Benchmark(Baseline = true), BenchmarkCategory("ToEnumInt")]
        public Fruits ToEnumInt()
        {
            return (Fruits)Enum.ToObject(typeof(Fruits), 11);
        }

        [Benchmark, BenchmarkCategory("ToEnumInt")]
        public Fruits SVEnumsToEnumInt()
        {
            return Enums<Fruits>.ToEnum(11);
        }

        [Benchmark, BenchmarkCategory("ToEnumInt")]
        public Fruits ToEnumIntByte()
        {
            return (Fruits)Enum.ToObject(typeof(Fruits), (byte)11);
        }

        [Benchmark, BenchmarkCategory("ToEnumInt")]
        public Fruits SVEnumsToEnumIntByte()
        {
            return Enums<Fruits>.ToEnum((byte)11);
        }

        [Benchmark, BenchmarkCategory("ToEnumInt")]
        public Fruits ToEnumIntObject()
        {
            return (Fruits)Enum.ToObject(typeof(Fruits), (object)11);
        }

        [Benchmark, BenchmarkCategory("ToEnumInt")]
        public Fruits SVEnumsToEnumIntObject()
        {
            return Enums<Fruits>.ToEnum((object)11);
        }
    }

```


�������

```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4651/22H2/2022Update)
Intel Core i7-10700 CPU 2.90GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100-preview.5.24307.3
  [Host]     : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.6 (8.0.624.26715), X64 RyuJIT AVX2


```
| Method                 | Mean       | Error     | StdDev    | Median     | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|----------------------- |-----------:|----------:|----------:|-----------:|------:|--------:|-------:|----------:|------------:|
| SVEnumsToEnumInt       |  0.9796 ns | 0.0062 ns | 0.0055 ns |  0.9781 ns |  0.02 |    0.00 |      - |         - |        0.00 |
| SVEnumsToEnumIntByte   |  1.0990 ns | 0.0089 ns | 0.0074 ns |  1.0966 ns |  0.03 |    0.00 |      - |         - |        0.00 |
| SVEnumsToEnumIntObject |  5.1211 ns | 0.0842 ns | 0.0746 ns |  5.1295 ns |  0.12 |    0.00 | 0.0029 |      24 B |        1.00 |
| ToEnumIntByte          | 40.9720 ns | 0.2100 ns | 0.1861 ns | 40.9065 ns |  1.00 |    0.03 | 0.0029 |      24 B |        1.00 |
| ToEnumInt              | 41.1962 ns | 0.8452 ns | 1.4122 ns | 40.4985 ns |  1.00 |    0.05 | 0.0029 |      24 B |        1.00 |
| ToEnumIntObject        | 48.2590 ns | 0.4380 ns | 0.3882 ns | 48.0802 ns |  1.17 |    0.04 | 0.0057 |      48 B |        2.00 |



| Method                  | Categories    | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------ |-------------- |-----------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| SVEnumsParse            |               |  2.8382 ns | 0.0508 ns | 0.0450 ns |  0.12 |    0.00 |      - |      - |         - |          NA |
| FastEnumParse           |               |  6.9671 ns | 0.0437 ns | 0.0388 ns |  0.28 |    0.00 |      - |      - |         - |          NA |
| EnumInfoParse           |               |  7.1513 ns | 0.1049 ns | 0.0930 ns |  0.29 |    0.00 |      - |      - |         - |          NA |
| Parse                   |               | 24.5338 ns | 0.0548 ns | 0.0485 ns |  1.00 |    0.00 |      - |      - |         - |          NA |
|                         |               |            |           |           |       |         |        |        |           |             |
| FastEnumGetName         | GetName       |  0.8608 ns | 0.0175 ns | 0.0155 ns |  0.32 |    0.01 |      - |      - |         - |          NA |
| EnumInfoGetName         | GetName       |  1.4291 ns | 0.0147 ns | 0.0130 ns |  0.54 |    0.01 |      - |      - |         - |          NA |
| SVEnumsGetName          | GetName       |  1.6210 ns | 0.0148 ns | 0.0131 ns |  0.61 |    0.01 |      - |      - |         - |          NA |
| GetName                 | GetName       |  2.6512 ns | 0.0150 ns | 0.0125 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
|                         |               |            |           |           |       |         |        |        |           |             |
| SVEnumsGetNames         | GetNames      |  0.2539 ns | 0.0061 ns | 0.0051 ns |  0.01 |    0.00 |      - |      - |         - |        0.00 |
| FastEnumGetNames        | GetNames      |  0.6874 ns | 0.0195 ns | 0.0163 ns |  0.03 |    0.00 |      - |      - |         - |        0.00 |
| GetNames                | GetNames      | 21.0463 ns | 0.4645 ns | 0.5162 ns |  1.00 |    0.03 | 0.0239 | 0.0001 |     200 B |        1.00 |
|                         |               |            |           |           |       |         |        |        |           |             |
| SVEnumsGetValues        | GetValues     |  0.3022 ns | 0.0296 ns | 0.0277 ns | 0.009 |    0.00 |      - |      - |         - |        0.00 |
| FastEnumGetValues       | GetValues     |  0.6683 ns | 0.0098 ns | 0.0082 ns | 0.021 |    0.00 |      - |      - |         - |        0.00 |
| GetValues               | GetValues     | 32.5145 ns | 0.6732 ns | 0.5968 ns | 1.000 |    0.03 | 0.0134 |      - |     112 B |        1.00 |
|                         |               |            |           |           |       |         |        |        |           |             |
| SVEnumsParseIgnoreCase  | IgnoreCase    |  3.0465 ns | 0.0680 ns | 0.0727 ns |  0.12 |    0.00 |      - |      - |         - |          NA |
| EnumInfoParseIgnoreCase | IgnoreCase    | 10.1299 ns | 0.1660 ns | 0.1472 ns |  0.42 |    0.01 |      - |      - |         - |          NA |
| FastEnumParseIgnoreCase | IgnoreCase    | 10.3531 ns | 0.0807 ns | 0.0674 ns |  0.42 |    0.00 |      - |      - |         - |          NA |
| ParseIgnoreCase         | IgnoreCase    | 24.3767 ns | 0.1270 ns | 0.1060 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
|                         |               |            |           |           |       |         |        |        |           |             |
| SVEnumsIsDefinedName    | IsDefinedName |  2.7188 ns | 0.0111 ns | 0.0098 ns |  0.11 |    0.00 |      - |      - |         - |          NA |
| EnumInfoIsDefinedName   | IsDefinedName |  6.6075 ns | 0.0190 ns | 0.0148 ns |  0.26 |    0.00 |      - |      - |         - |          NA |
| FastEnumIsDefinedName   | IsDefinedName |  6.7011 ns | 0.0388 ns | 0.0303 ns |  0.26 |    0.00 |      - |      - |         - |          NA |
| IsDefinedName           | IsDefinedName | 25.3131 ns | 0.2064 ns | 0.1829 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
