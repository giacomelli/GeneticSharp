using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Collections;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
    [TestFixture]
    [Category("Extensions")]
    public class TspFitnessTest
    {
        [Test]
        public void Constructor_MaxEqualsIntEdges_Exception()
        {
            var actual = Assert.Catch<ArgumentOutOfRangeException>(() => new TspFitness(10, 0, int.MaxValue, 0, 10000000));
            Assert.AreEqual("maxX", actual.ParamName);

            actual = Assert.Catch<ArgumentOutOfRangeException>(() => new TspFitness(10, 0, 10000000, 0, int.MaxValue));
            Assert.AreEqual("maxY", actual.ParamName);
        }


        // Ok it seems after usage of Math.Pow was replaced with simpler squares, city distance caching isn't an edge anymore, and direct computation is faster
        [Test]
        public void Evaluate_Manychromosomes_Cached_Faster()
        {

            var repeatNb = 40;

            // Note that with higher numbers the situation eventually reverses and the memory impediment must be detrimental
            var cityNbsAndRatios = new (int cityNb, double ratio)[] { (5, 0.9), (100, 0.95), (500, 1.1), (2000, 1.7)};


            var testResults = new List<(TimeSpan durationUncached, TimeSpan durationCached, double ratio)>();

            foreach (var (cityNb, ratio) in cityNbsAndRatios)
            {

               

                var repeatCityResults = new List<(TimeSpan durationUncached, TimeSpan durationCached, double ratio)>();

                for (int i = 0; i < repeatNb; i++)
                {

                    var chromosomes = Enumerable.Range(0, 5000 / cityNb).Select(_ => new TspChromosome(cityNb).Initialized()).ToList();

                    var fitness = new TspFitness(cityNb, -cityNb, cityNb, -cityNb, cityNb) {Cached = false};
                    fitness.Evaluate(chromosomes.First());
                    var sw = Stopwatch.StartNew();
                    var fitnesses = chromosomes.Select(tspChromosome => fitness.Evaluate(tspChromosome)).ToList();
                    var durationNonOptimised = sw.Elapsed;

                    fitness.Cached = true;
                    var cachedDistances = fitness.CityDistances;
                    sw.Restart();
                    var fitnessesCached = chromosomes.Select(tspChromosome => fitness.Evaluate(tspChromosome)).ToList();
                    var durationOptimised = sw.Elapsed;

                    repeatCityResults.Add((durationNonOptimised, durationOptimised, ratio));

                    for (int j = 0; j < fitnesses.Count; j++)
                    {
                        Assert.LessOrEqual(Math.Abs(fitnesses[j] - fitnessesCached[j]), double.Epsilon);
                    }

                }

                var resultsWithoutExtrema = repeatCityResults.OrderByDescending(r => r.durationUncached).Skip(5).Take(30).ToList();
                resultsWithoutExtrema = resultsWithoutExtrema.OrderByDescending(r => r.durationCached).Skip(5).Take(20).ToList();
                var meanResult = (
                    TimeSpan.FromTicks(resultsWithoutExtrema.Sum(r => r.durationUncached.Ticks / resultsWithoutExtrema.Count)),
                    TimeSpan.FromTicks(resultsWithoutExtrema.Sum(r => r.durationCached.Ticks/ resultsWithoutExtrema.Count)),
                    ratio);

                testResults.Add(meanResult);
            }


            testResults.Each(r=> Assert.LessOrEqual(r.durationCached.Ticks / (double) r.durationUncached.Ticks, r.ratio));

        }


        [Test]
        public void Evaluate_DefaultChromosome_FitnessDividedByMissingCities()
        {

            var cityNbs = new[] {5, 10, 100, 1000, 10000};

            foreach (var cityNb in cityNbs)
            {
                var target = new TspFitness(cityNb, -cityNb, cityNb, -cityNb, cityNb);
                var chromosome = new TspChromosome(cityNb).Initialized();

                //A random chromosome should have a strictly positive fitness
                var initial = target.Evaluate(chromosome);
                Assert.Greater(initial, 0);

                chromosome = new TspChromosome(cityNb-1).Initialized();

                var divided = target.Evaluate(chromosome);
                Assert.Greater(divided, 0);

                Assert.Less(divided, initial);

                chromosome = new TspChromosome(cityNb - 2).Initialized();

                var dividedBis = target.Evaluate(chromosome);
                Assert.Greater(dividedBis, 0);

                Assert.Less(dividedBis, divided);

                chromosome = new TspChromosome(cityNb/2).Initialized();

                var scaledAndDivided = target.Evaluate(chromosome);

                Assert.Greater(scaledAndDivided, 0);

                Assert.Less(scaledAndDivided, dividedBis);
            }

        }

        [Test]
        public void Evaluate_RandomChromosome_TightBounds()
        {
            var repeatNb = 100;
            var maxCityNb = 100000;
            var cityNbs = new[] { 5, 10, 50, 100, 1000, 10000, maxCityNb };
            var fitnesses = new List<double>(cityNbs.Length);
            foreach (var cityNb in cityNbs)
            {
                var target = new TspFitness(cityNb, -cityNb, cityNb, -cityNb, cityNb);
                var cityFitnesses = new List<double>(repeatNb);

                //We run several passes for small city numbers
                for (int i = 0; i <1 + maxCityNb / (1 + 100*i); i++)
                {
                    var chromosome = new TspChromosome(cityNb).Initialized();
                    var fitness = target.Evaluate(chromosome);
                    if (fitness>1)
                    {
                        Debugger.Break();
                    }

                    Assert.Greater(fitness, 0.5);
                    Assert.Less(fitness, 9);
                   
                    cityFitnesses.Add(fitness);
                }
                fitnesses.Add(cityFitnesses.Sum()/ cityFitnesses.Count);
            }

            
            var withoutSmallNbCases = fitnesses.Skip(4).ToList();
            var meanDefaultFitness = withoutSmallNbCases.Sum() / withoutSmallNbCases.Count;
            //Converges to 0.631 (~log3(2)), maybe useful to calibrate to 0.5
            Assert.Greater(meanDefaultFitness,0.62);
            Assert.Less(meanDefaultFitness, 0.64);

        }

        [Test]
        public void Evaluate_FitnessGreaterThanZero()
        {
            var target = new TspFitness(10, 0, 10000000, 0, 10000000);
            var chromosome = new TspChromosome(10).Initialized();
            var actual = target.Evaluate(chromosome);
            Assert.Greater(actual,0);
        }
    }
}

