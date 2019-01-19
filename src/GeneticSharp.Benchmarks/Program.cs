using System;
using BenchmarkDotNet.Running;

namespace GeneticSharp.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CrossoversBenchmark>();
        }
    }
}
