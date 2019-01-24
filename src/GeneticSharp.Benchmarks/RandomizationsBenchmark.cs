using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class RandomizationsBenchmark
    {
        private BasicRandomization _basic = new BasicRandomization();
        private FastRandomRandomization _fastRandom = new FastRandomRandomization();
        private const int _min = -100;
        private const int _max = 100;
        private const int _arrayLength = 10;

        #region Basic
        [Benchmark]
        public void Basic_GetDouble()
        {
            _basic.GetDouble();
        }

        [Benchmark]
        public void Basic_GetEvenInt()
        {
            _basic.GetEvenInt(_min, _max);
        }

        [Benchmark]
        public void Basic_GetFloat()
        {
            _basic.GetFloat();
        }

        [Benchmark]
        public void Basic_GetInt()
        {
            _basic.GetInt(_min, _max);
        }

        [Benchmark]
        public void Basic_GetInts()
        {
            _basic.GetInts(_arrayLength, _min, _max);
        }

        [Benchmark]
        public void Basic_GetOddInt()
        {
            _basic.GetOddInt(_min, _max);
        }

        [Benchmark]
        public void Basic_GetUniqueInts()
        {
            _basic.GetUniqueInts(_arrayLength, _min, _max);
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
            _fastRandom.GetEvenInt(_min, _max);
        }

        [Benchmark]
        public void FastRandom_GetFloat()
        {
            _fastRandom.GetFloat();
        }

        [Benchmark]
        public void FastRandom_GetInt()
        {
            _fastRandom.GetInt(_min, _max);
        }

        [Benchmark]
        public void FastRandom_GetInts()
        {
            _fastRandom.GetInts(_arrayLength, _min, _max);
        }

        [Benchmark]
        public void FastRandom_GetOddInt()
        {
            _basic.GetOddInt(_min, _max);
        }

        [Benchmark]
        public void FastRandom_GetUniqueInts()
        {
            _fastRandom.GetUniqueInts(_arrayLength, _min, _max);
        }
        #endregion 
    }
}