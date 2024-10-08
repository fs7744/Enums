﻿using BenchmarkDotNet.Attributes;
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
using System.Collections.Immutable;

namespace Benchmark
{
    [Flags]
    public enum Fruits2
    {
        Apple = 1,
        Lemon = 2,
        Melon = 4,
        Banana = 8
    }

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
            //var a = FastEnum.IsContinuous<Fruits>();
            return test.GetName(Fruits.Lemon);
        }

        [Benchmark(Baseline = true), BenchmarkCategory("GetNames")]
        public string[] GetNames()
        {
            return Enum.GetNames<Fruits>();
        }

        [Benchmark, BenchmarkCategory("GetNames")]
        public IReadOnlyList<string> FastEnumGetNames()
        {
            return FastEnum.GetNames<Fruits>();
        }

        [Benchmark, BenchmarkCategory("GetNames")]
        public ImmutableArray<string> SVEnumsGetNames()
        {
            return Enums<Fruits>.GetNames();
        }

        [Benchmark(Baseline = true), BenchmarkCategory("GetValues")]
        public Fruits[] GetValues()
        {
            return Enum.GetValues<Fruits>();
        }

        [Benchmark, BenchmarkCategory("GetValues")]
        public IReadOnlyList<Fruits> FastEnumGetValues()
        {
            return FastEnum.GetValues<Fruits>();
        }

        [Benchmark, BenchmarkCategory("GetValues")]
        public ImmutableArray<Fruits> SVEnumsGetValues()
        {
            return Enums<Fruits>.GetValues();
        }

        [Benchmark(Baseline = true), BenchmarkCategory("IsDefinedName")]
        public bool IsDefinedName()
        {
            return Enum.TryParse<Fruits>("Melon", false, out var r);
        }

        [Benchmark, BenchmarkCategory("IsDefinedName")]
        public bool FastEnumIsDefinedName()
        {
            return FastEnum.IsDefined<Fruits>("Melon");
        }

        [Benchmark, BenchmarkCategory("IsDefinedName")]
        public bool SVEnumsIsDefinedName()
        {
            return Enums<Fruits>.IsDefined("Melon");
        }

        [Benchmark, BenchmarkCategory("IsDefinedName")]
        public bool EnumInfoIsDefinedName()
        {
            return test.IsDefined("Melon");
        }
    }

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
}