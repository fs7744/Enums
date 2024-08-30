using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using FastEnumUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SV;

namespace Benchmark
{
    [Flags]
    public enum Fruits
    {
        Apple = 1,
        Lemon = 2,
        Melon = 4,
        Banana = 8,
    }

    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class EnumBenchmarks
    {
        private readonly EnumInfo<Fruits> test;

        public EnumBenchmarks()
        {
            test = new EnumInfo<Fruits>();
            Enums.SetEnumInfo<Fruits>(new TestIEnumInfo());
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

        [Benchmark(Baseline = true), BenchmarkCategory("GetName")]
        public string? GetName()
        {
            return Enum.GetName<Fruits>(Fruits.Lemon);
        }

        [Benchmark, BenchmarkCategory("GetName")]
        public string? FastEnumGetName()
        {
            return FastEnum.GetName<Fruits>(Fruits.Lemon);
        }

        [Benchmark, BenchmarkCategory("GetName")]
        public string? SVEnumsGetName()
        {
            return Enums<Fruits>.GetName(Fruits.Lemon);
        }

        [Benchmark, BenchmarkCategory("GetName")]
        public string? EnumInfoGetName()
        {
            return test.GetName(Fruits.Lemon);
        }
    }

    public class TestIEnumInfo : EnumBase<Fruits>
    {
        public override string GetName(Fruits t)
        {
            switch (t)
            {
                case Fruits.Apple:
                    return nameof(Fruits.Apple);

                case Fruits.Lemon:
                    return nameof(Fruits.Lemon);

                case Fruits.Melon:
                    return nameof(Fruits.Melon);

                case Fruits.Banana:
                    return nameof(Fruits.Banana);

                default:
                    return null;
            }
        }

        protected override bool TryParseCase(in ReadOnlySpan<char> name, out Fruits result)
        {
            switch (name)
            {
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Apple).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits.Apple;
                    return true;

                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Lemon).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits.Lemon;
                    return true;

                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Melon).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits.Melon;
                    return true;

                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Banana).AsSpan(), global::System.StringComparison.Ordinal):
                    result = global::Benchmark.Fruits.Banana;
                    return true;

                default:
                    result = default;
                    return false;
            }
        }

        protected override bool TryParseIgnoreCase(in ReadOnlySpan<char> name, out Fruits result)
        {
            switch (name)
            {
                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Apple).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits.Apple;
                    return true;

                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Lemon).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits.Lemon;
                    return true;

                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Melon).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits.Melon;
                    return true;

                case ReadOnlySpan<char> current when current.Equals(nameof(global::Benchmark.Fruits.Banana).AsSpan(), global::System.StringComparison.OrdinalIgnoreCase):
                    result = global::Benchmark.Fruits.Banana;
                    return true;

                default:
                    result = default;
                    return false;
            }
        }

        protected override bool TryParseUnderlyingTypeString(string value, out Fruits result)
        {
            if (int.TryParse(value, out var numericResult))
            {
                result = (Fruits)numericResult;
                return true;
            }
            result = default;
            return false;
        }
    }
}