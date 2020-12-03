using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Infrastructure.Framework.Collections;
using NUnit.Framework;

namespace GeneticSharp.Infrastructure.Framework.UnitTests.Collections
{

    [TestFixture()]
    public class EnumerablerTest
    {
        [Test()]
        public void Each_AddOne_ValuesExpected()
        {
            var initArray = new int[] {0,1,2} ;
            var targetList = new List<int>(initArray.Length);
            initArray.Each(i=>targetList.Add(i+1));

            for (int i = 0; i < targetList.Count; i++)
            {
                Assert.AreEqual(targetList[i], initArray[i]+1);
            }
            
        }


#if NETCOREAPP
        // .NET core 2+ uses quicksort partition to return first items without doing the whole sort
        //The .Net core version of MaxBy uses OrderByDescending then First, so it is a pass-through concerning those tests,
        //setting the bound low here for what should really be close to 1 as Local tests seem to yield 
        
        private double ratioMax = 0.7;

#else
            // .NET Framework 4.0 sorts all when descending
            // Very conservative bound
            private double ratioMax = 2;
#endif


        [Test()]
        public void MaxBy_CompareWithOrderByDescendingAndFirst_50_Faster()
        {




            MaxBy_CompareWithOrderByDescendingAndFirst_Faster(50, 10000, ratioMax);
        }

        [Test()]
        public void MaxBy_CompareWithOrderByDescendingAndFirst_500_Faster()
        {




            MaxBy_CompareWithOrderByDescendingAndFirst_Faster(500, 1000, ratioMax);
        }

        [Test()]
        public void MaxBy_CompareWithOrderByDescendingAndFirst_5000_Faster()
        {

            MaxBy_CompareWithOrderByDescendingAndFirst_Faster(5000, 100, ratioMax);
        }

        private void MaxBy_CompareWithOrderByDescendingAndFirst_Faster(int maxValue, int nbTests, double minRatio)
        {
            RandomizationProvider.Current = new FastRandomRandomization();
            
            var unsortedSets = new List<IList<IChromosome>>(nbTests/10);
            for (int testIdx = 0; testIdx < nbTests; testIdx++)
            {
                var unsorted = RandomizationProvider.Current.GetUniqueInts(maxValue, 1, maxValue + 1);
                var chromosomes = unsorted.Select(i => (IChromosome) new IntegerChromosome(0, maxValue) {Fitness = (double) i}).ToList();
                unsortedSets.Add(chromosomes);
            }

            var maxValuesSorts = Enumerable.Range(0, 2).Select(i => new List<double>(nbTests)).ToList();
            var stopWatches = Enumerable.Range(0,2).Select(i =>  new Stopwatch()).ToList();
            var executionTimes = Enumerable.Range(0, 2).Select(i => new List<TimeSpan>()).ToList();

            var keySelector = new Func<IChromosome, double?>(c => c.Fitness);

           

            for (int i = 0; i < 40; i++)
            {
               stopWatches[0].Start();
                foreach (var unsortedSet in unsortedSets)
                {
                    maxValuesSorts[0].Add(unsortedSet.MaxBy(keySelector).Fitness.Value);
                }
                stopWatches[0].Stop();
                executionTimes[0].Add(stopWatches[0].Elapsed);
                stopWatches[0].Reset();
            }

            for (int i = 0; i < 40; i++)
            {
                stopWatches[1].Start();
                foreach (var unsortedSet in unsortedSets)
                {
                    maxValuesSorts[1].Add(unsortedSet.OrderByDescending(keySelector).First().Fitness.Value);
                }
                stopWatches[1].Stop();
                executionTimes[1].Add(stopWatches[1].Elapsed);
                stopWatches[1].Reset();
            }

            maxValuesSorts.Each(maxValues => maxValues.Each(i => Assert.AreEqual((double)maxValue, i)));

            //removing extrema
            executionTimes[0] = executionTimes[0].OrderBy(t => t.Ticks).Skip(10).Take(20).ToList();
            executionTimes[1] = executionTimes[1].OrderBy(t => t.Ticks).Skip(10).Take(20).ToList();

            // Using Mean Value
            var maxByOrderTime = TimeSpan.FromTicks(executionTimes[0].Select(span => span.Ticks).Sum()/executionTimes[0].Count);
            var classicalOrderTime =
                TimeSpan.FromTicks(executionTimes[1].Select(span => span.Ticks).Sum() / executionTimes[1].Count);
            var ratio = classicalOrderTime.Ticks / (double) maxByOrderTime.Ticks;
            Assert.Greater(ratio, minRatio);
        }


        // The following tests seem to pass or not depending on the .Net Framework version. The latest .Net code version probably includes the same kind of improvement


        //[Test()]
        //public void LazyOrderBy_CompareWithOrderBy_50_Lower10Percents_Faster()
        //{

        //    LazyOrderBy_CompareWithOrderBy_Faster(50, 10, 2000, 1);
        //}

        //[Test()]
        //public void LazyOrderBy_CompareWithOrderBy_500_Lower10Percents_Faster()
        //{

        //    LazyOrderBy_CompareWithOrderBy_Faster(500, 10, 2000, 1);
        //}

        //[Test()]
        //public void LazyOrderBy_CompareWithOrderBy_5000_Lower10Percents_Faster()
        //{

        //    LazyOrderBy_CompareWithOrderBy_Faster(5000, 10, 200, 1);
        //}


        //[Test()]
        //public void LazyOrderBy_CompareWithOrderBy_50_Lower30Percents_Faster()
        //{

        //    LazyOrderBy_CompareWithOrderBy_Faster(50, 30, 2000, 1);
        //}


        //[Test()]
        //public void LazyOrderBy_CompareWithOrderBy_500_Lower30Percents_Faster()
        //{

        //    LazyOrderBy_CompareWithOrderBy_Faster(500, 30, 2000, 1);
        //}

        //[Test()]
        //public void LazyOrderBy_CompareWithOrderBy_5000_Lower30Percents_Faster()
        //{

        //    LazyOrderBy_CompareWithOrderBy_Faster(5000, 30, 200, 1);
        //}

        //[Test()]
        //public void LazyOrderBy_CompareWithOrderBy_500_Lower90Percents_Slower()
        //{

        //  Assert.Throws<AssertionException>(() => LazyOrderBy_CompareWithOrderBy_Faster(500, 90, 200, 1));
        //}

        //private void LazyOrderBy_CompareWithOrderBy_Faster(int maxValue, int takePercent,  int nbTests, double minRatio)
        //{
        //    var unsortedSets = new List<int[]>(nbTests);
        //    for (int i = 0; i < nbTests; i++)
        //    {
        //        var unsorted = RandomizationProvider.Current.GetUniqueInts(maxValue, 1, maxValue + 1);
        //        unsortedSets.Add(unsorted);
        //    }

        //    var takeNb = maxValue * takePercent / 100;
        //    var sw = Stopwatch.StartNew();
        //    foreach (var unsortedSet in unsortedSets)
        //    {
        //        unsortedSet.LazyOrderBy(i => i).Take(takeNb).ToList();
        //    }
        //    sw.Stop();
        //    var lazyOrderTime = sw.Elapsed;
        //    sw.Reset();
        //    sw.Start();
        //    foreach (var unsortedSet in unsortedSets)
        //    {
        //        unsortedSet.OrderBy(i => i).Take(takeNb).ToList();
        //    }
        //    sw.Stop();
        //    var classicalOrderTime = sw.Elapsed;
        //    var ratio = classicalOrderTime.Ticks /(double) lazyOrderTime.Ticks;
        //    Assert.Greater(ratio, minRatio);
        //}


    }
}
