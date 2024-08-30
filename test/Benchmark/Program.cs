using Benchmark;
using BenchmarkDotNet.Running;

var a = new ToEnumBenchmarks().SVEnumsToEnumIntByte();
var summary = BenchmarkRunner.Run<ToEnumBenchmarks>();
//var summary = BenchmarkRunner.Run(typeof(Program).Assembly);