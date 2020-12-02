using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture()]
    [Category("MetaHeuristics")]
    class MetaGeneticAlgorithmTest: MetaHeuristicTestBase
    {
        [Test()]
        public void Compare_RegularGA_ChromosomeStub_DifferentSizes_ManyGenerations_DurationBounds()
        {
            var testParams = new List<(int size, double ratio)>
            {
                (10, 1.1), (100, 1.1), (500, 1.1)
            };

            Func<int, IFitness> fitness = i => new FitnessStub(i) { SupportsParallel = false };
            Func<int, IChromosome> adamChromosome = i => new ChromosomeStub(i, i);

            var crossover = new UniformCrossover();
            var termination = new GenerationNumberTermination(1000);

            foreach (var testParam in testParams)
            {

                var heuristics = new List<IMetaHeuristic>();
                heuristics.Add(null);
                heuristics.Add(new DefaultMetaHeuristic());

                var results = CompareMetaHeuristics(fitness(testParam.size), adamChromosome(testParam.size),
                    heuristics, crossover,  100, termination);
                this.AssertIsPerformingLessByRatio(termination, testParam.ratio, results[0], results[1]);

            }

         


        }


      



    }
}