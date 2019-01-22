using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class RandomizationsBenchmark
    {
        private BasicRandomization _basic = new BasicRandomization();
        private FastRandomRandomization _fastRandom = new FastRandomRandomization();

        [Params(-100)]
        public int Min { get; set; }

        [Params(100)]
        public int Max { get; set; }

        [Params(10)]
        public int ArrayLength { get; set; }

        #region Basic
        [Benchmark]
        public void Basic_GetDouble()
        {
            _basic.GetDouble();
        }

        [Benchmark]
        public void Basic_GetEvenInt()
        {
            _basic.GetEvenInt(Min, Max);
        }

        [Benchmark]
        public void Basic_GetFloat()
        {
            _basic.GetFloat();
        }

        [Benchmark]
        public void Basic_GetInt()
        {
            _basic.GetInt(Min, Max);
        }

        [Benchmark]
        public void Basic_GetInts()
        {
            _basic.GetInts(ArrayLength, Min, Max);
        }

        [Benchmark]
        public void Basic_GetOddInt()
        {
            _basic.GetOddInt(Min, Max);
        }

        [Benchmark]
        public void Basic_GetUniqueInts()
        {
            _basic.GetUniqueInts(ArrayLength, Min, Max);
        }
        #endregion 

        #region FastRandom
        [Benchmark(Baseline = true)]
        public void FastRandom_GetDouble()
        {
            _fastRandom.GetDouble();
        }

        [Benchmark]
        public void FastRandom_GetEvenInt()
        {
            _fastRandom.GetEvenInt(Min, Max);
        }

        [Benchmark]
        public void FastRandom_GetFloat()
        {
            _fastRandom.GetFloat();
        }

        [Benchmark]
        public void FastRandom_GetInt()
        {
            _fastRandom.GetInt(Min, Max);
        }

        [Benchmark]
        public void FastRandom_GetInts()
        {
            _fastRandom.GetInts(ArrayLength, Min, Max);
        }

        [Benchmark]
        public void FastRandom_GetOddInt()
        {
            _basic.GetOddInt(Min, Max);
        }

        [Benchmark]
        public void FastRandom_GetUniqueInts()
        {
            _fastRandom.GetUniqueInts(ArrayLength, Min, Max);
        }
        #endregion 
    }
}