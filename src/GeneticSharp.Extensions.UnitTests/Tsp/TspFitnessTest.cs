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
    [TestFixture()]
    [Category("Extensions")]
    public class TspFitnessTest
    {
        [Test()]
        public void Constructor_MaxEqualsIntEdges_Exception()
        {
            var actual = Assert.Catch<ArgumentOutOfRangeException>(() => new TspFitness(10, 0, int.MaxValue, 0, 10000000));
            Assert.AreEqual("maxX", actual.ParamName);

            actual = Assert.Catch<ArgumentOutOfRangeException>(() => new TspFitness(10, 0, 10000000, 0, int.MaxValue));
            Assert.AreEqual("maxY", actual.ParamName);
        }


        [Test()]
        public void Evaluate_Manychromosomes_Cached_Faster()
        {
            // Note that with higher numbers the situation eventually reverses and the memory impediment must be detrimental
            var cityNbs = new[] { 5, 10, 100, 1000 };

            foreach (var cityNb in cityNbs)
            {
                var chromosomes = Enumerable.Range(0, 2000).Select(i => new TspChromosome(cityNb).Initialized());

                var fitness = new TspFitness(cityNb, -cityNb, cityNb, -cityNb, cityNb);
                fitness.Cached = false;

                var sw = Stopwatch.StartNew();
                chromosomes.Each(tspChromosome => fitness.Evaluate(tspChromosome));
                var durationNonOptimised = sw.Elapsed;

                fitness.Cached = true;
                var cachedDistances = fitness.CityDistances;
                sw.Restart();
                chromosomes.Each(tspChromosome => fitness.Evaluate(tspChromosome));
                var durationOptimised = sw.Elapsed;

                Assert.Greater(durationNonOptimised, durationOptimised);
                
            }

        }


        [Test()]
        public void Evaluate_DefaultChromosome_FitnessDividedByMissingCities()
        {

            var cityNbs = new[] {5, 10, 100, 1000, 100000};

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

                Assert.Less(scaledAndDivided, divided);
            }

        }

        [Test()]
        public void Evaluate_RandomChromosome_TightBounds()
        {
            var cityNbs = new[] { 5, 10, 20, 50, 100, 200, 1000, 5000, 10000, 100000 };
            var fitnesses = new List<double>(cityNbs.Length);
            foreach (var cityNb in cityNbs)
            {
                var target = new TspFitness(cityNb, -cityNb, cityNb, -cityNb, cityNb);
                var chromosome = new TspChromosome(cityNb).Initialized();

                fitnesses.Add(target.Evaluate(chromosome));
            }

            fitnesses.Each(d =>
            {
                Assert.Greater(d, 0.3);
                Assert.Less(d, 0.7);
            });

            var meanDefaultFitness = fitnesses.Sum() / fitnesses.Count;
            Assert.Greater(meanDefaultFitness,0.45);
            Assert.Less(meanDefaultFitness, 0.55);

        }

        [Test()]
        public void Evaluate_FitnessGreaterThanZero()
        {
            var target = new TspFitness(10, 0, 10000000, 0, 10000000);
            var chromosome = new TspChromosome(10).Initialized();
            var actual = target.Evaluate(chromosome);
            Assert.Greater(actual,0);
        }
    }
}

