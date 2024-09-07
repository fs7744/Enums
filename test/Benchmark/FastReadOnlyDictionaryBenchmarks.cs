using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using SV;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Benchmark
{
    [ShortRunJob, MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class FastReadOnlyDictionaryBenchmarks
    {
        [Params(0, 1, 10, 100, 1000, 10000, 100000)]
        public int Count { get; set; }

        private FastReadOnlyDictionary<string, Customer> fastReadOnlyDictionary;

        private ReadOnlyOrdinalIgnoreCaseStringDictionary<Customer> readOnlyOrdinalIgnoreCaseStringDictionary;
        private FrozenDictionary<string, Customer> frozenDictionary;
        private FrozenDictionary<string, Customer> frozenDictionaryOrdinalIgnoreCase;
        private string test;
        private string test2;

        [GlobalSetup]
        public void Setup()
        {
            List<Customer> data = new();
            var b = new byte[10];
            for (int i = 0; i < Count; i++)
            {
                Random.Shared.NextBytes(b);
                data.Add(new Customer { Id = i, Name = Encoding.UTF8.GetString(b) + i.ToString() });
            }
            fastReadOnlyDictionary = data.ToFastReadOnlyDictionary(i => i.Name, i => i);
            readOnlyOrdinalIgnoreCaseStringDictionary = data.ToReadOnlyOrdinalIgnoreCaseStringDictionary(i => i.Name, i => i);
            frozenDictionary = data.ToFrozenDictionary(i => i.Name, i => i);
            frozenDictionaryOrdinalIgnoreCase = data.ToFrozenDictionary(i => i.Name, i => i, StringComparer.OrdinalIgnoreCase);
            if (data.IsNullOrEmpty())
            {
                test = string.Empty;
            }
            else
            {
                test = data[Random.Shared.Next(0, data.Count)].Name;
            }
            test2 = test?.ToLower();
        }

        [Benchmark(Baseline = true)]
        public bool FrozenDictionary()
        {
            return frozenDictionary.TryGetValue(test, out _);
        }

        [Benchmark]
        public bool FastReadOnlyDictionary()
        {
            return fastReadOnlyDictionary.TryGetValue(test, out _);
        }

        [Benchmark(Baseline = true), BenchmarkCategory("OrdinalIgnoreCase")]
        public bool FrozenDictionaryOrdinalIgnoreCase()
        {
            return frozenDictionaryOrdinalIgnoreCase.TryGetValue(test2, out _);
        }

        [Benchmark, BenchmarkCategory("OrdinalIgnoreCase")]
        public bool ReadOnlyOrdinalIgnoreCaseStringDictionary()
        {
            return readOnlyOrdinalIgnoreCaseStringDictionary.TryGetValue(test2, out _);
        }
    }
}