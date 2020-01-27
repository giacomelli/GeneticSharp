using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace GeneticSharp.Benchmarks
{
    public class DefaultConfig : ManualConfig
    {
        public DefaultConfig()
        {
            Set(new DefaultOrderer(SummaryOrderPolicy.Default, MethodOrderPolicy.Alphabetical));
            Add(RankColumn.Arabic);

            Add(Job.Core
                .WithMinIterationCount(15)
                .WithMaxIterationCount(100));
         }
    }
}
