using System.Linq;
using BenchmarkDotNet.Attributes;
using GeneticSharp.Infrastructure.Framework.Collections;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class RangeBenchmark
    {
        public int RepeatNb { get; set; } = 1000;

        public int Size { get; set; } = 100;


        [Benchmark]
        public void ForLoop()
        {
            for (int r = 0; r < RepeatNb; r++)
            {
                for (int i = 0; i <= Size; i++)
                {
                    DoNothing();
                }
            }
          
        }

        [Benchmark]
        public void Range()
        {
            for (int r = 0; r < RepeatNb; r++)
            {
                foreach (var _ in Enumerable.Range(0, Size))
                {
                    DoNothing();
                }
            }
            
        }

        //[Benchmark]
        //public void CachedRange()
        //{
        //    for (int r = 0; r < RepeatNb; r++)
        //    {
        //        foreach (var _ in CachedEnumerable.Range(0, Size))
        //        {
        //            DoNothing();
        //        }
        //    }
        //}

        //[Benchmark]
        //public void CachedRangeEach()
        //{
        //    for (int r = 0; r < RepeatNb; r++)
        //    {
        //        CachedEnumerable.Range(0, Size).Each(i => DoNothing());
        //    }
        //}


        private static void DoNothing()
        {

        }

    }
}