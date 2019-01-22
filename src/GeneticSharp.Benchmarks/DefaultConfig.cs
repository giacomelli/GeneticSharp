using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace GeneticSharp.Benchmarks
{
    public class DefaultConfig : ManualConfig
    {
        public DefaultConfig()
        {
            Set(new DefaultOrderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Declared));
            Add(new MemoryDiagnoser());

            Add(Job.Core
                .WithMinIterationCount(5)
                .WithMaxIterationCount(10));
                
         }
    }
}
