using Benchmark;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<EnumBenchmarks>();
//var summary = BenchmarkRunner.Run(typeof(Program).Assembly);