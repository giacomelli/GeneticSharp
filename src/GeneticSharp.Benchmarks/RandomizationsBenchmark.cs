using BenchmarkDotNet.Attributes;
using GeneticSharp.Domain.Randomizations;

namespace GeneticSharp.Benchmarks
{
    [Config(typeof(DefaultConfig))]
    public class RandomizationsBenchmark
    {
        private BasicRandomization _basic = new BasicRandomization();
        private FastRandomRandomization _fastRandom = new FastRandomRandomization();
        private XorShiftRandomRandomization _xorShiftRandomRandomization = new XorShiftRandomRandomization();
        private Xoshiro256StarStarRandomRandomization _xoshiro256StarStarRandomRandomization = new Xoshiro256StarStarRandomRandomization();
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
            _fastRandom.GetOddInt(_min, _max);
        }

        [Benchmark]
        public void FastRandom_GetUniqueInts()
        {
            _fastRandom.GetUniqueInts(_arrayLength, _min, _max);
        }
        #endregion

        #region XorShiftRandom
        [Benchmark(Baseline = false)]
        public void XorShiftRandom_GetDouble()
        {
            _xorShiftRandomRandomization.GetDouble();
        }

        [Benchmark]
        public void XorShiftRandom_GetEvenInt()
        {
            _xorShiftRandomRandomization.GetEvenInt(_min, _max);
        }

        [Benchmark]
        public void XorShiftRandom_GetFloat()
        {
            _xorShiftRandomRandomization.GetFloat();
        }

        [Benchmark]
        public void XorShiftRandom_GetInt()
        {
            _xorShiftRandomRandomization.GetInt(_min, _max);
        }

        [Benchmark]
        public void XorShiftRandom_GetInts()
        {
            _xorShiftRandomRandomization.GetInts(_arrayLength, _min, _max);
        }

        [Benchmark]
        public void XorShiftRandom_GetOddInt()
        {
            _xorShiftRandomRandomization.GetOddInt(_min, _max);
        }

        [Benchmark]
        public void XorShiftRandom_GetUniqueInts()
        {
            _xorShiftRandomRandomization.GetUniqueInts(_arrayLength, _min, _max);
        }
        #endregion 

        #region Xoshiro256StarStarRandom
        [Benchmark(Baseline = false)]
        public void Xoshiro256StarStarRandom_GetDouble()
        {
            _xoshiro256StarStarRandomRandomization.GetDouble();
        }

        [Benchmark]
        public void Xoshiro256StarStarRandom_GetEvenInt()
        {
            _xoshiro256StarStarRandomRandomization.GetEvenInt(_min, _max);
        }

        [Benchmark]
        public void Xoshiro256StarStarRandom_GetFloat()
        {
            _xoshiro256StarStarRandomRandomization.GetFloat();
        }

        [Benchmark]
        public void Xoshiro256StarStarRandom_GetInt()
        {
            _xoshiro256StarStarRandomRandomization.GetInt(_min, _max);
        }

        [Benchmark]
        public void Xoshiro256StarStarRandom_GetInts()
        {
            _xoshiro256StarStarRandomRandomization.GetInts(_arrayLength, _min, _max);
        }

        [Benchmark]
        public void Xoshiro256StarStarRandom_GetOddInt()
        {
            _xoshiro256StarStarRandomRandomization.GetOddInt(_min, _max);
        }

        [Benchmark]
        public void Xoshiro256StarStarRandom_GetUniqueInts()
        {
            _xoshiro256StarStarRandomRandomization.GetUniqueInts(_arrayLength, _min, _max);
        }
        #endregion 
    }
}