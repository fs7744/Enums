using Benchmark;
using BenchmarkDotNet.Running;

//var a = new EnumBenchmarks().EnumInfoGetName();
//var summary = BenchmarkRunner.Run<EnumBenchmarks>();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly);