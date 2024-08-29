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
            Enums<Fruits>.TryParse("Melon", false, out var v);
            return v;
        }
    }
}