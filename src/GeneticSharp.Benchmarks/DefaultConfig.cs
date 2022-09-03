using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;

namespace GeneticSharp.Benchmarks
{
    public class DefaultConfig : ManualConfig
    {
        public DefaultConfig()
        {
            
            Orderer = new DefaultOrderer(SummaryOrderPolicy.Default, MethodOrderPolicy.Alphabetical);

            AddColumn(RankColumn.Arabic);

            AddJob(Job.Default
                .WithRuntime(CoreRuntime.Core60)
                .WithMinIterationCount(15)
                .WithMaxIterationCount(100));
         }
    }
}
