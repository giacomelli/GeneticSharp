using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Reinsertions;
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

            IFitness Fitness(int i) => new FitnessStub(i) {SupportsParallel = false};
            IChromosome AdamChromosome(int i) => new ChromosomeStub(i, i);

            var crossover = new UniformCrossover();
            var termination = new GenerationNumberTermination(1000);
            var reinsertion = new FitnessBasedElitistReinsertion();


            foreach (var (size, ratio) in testParams)
            {

                var heuristics = new List<IMetaHeuristic> {null, new DefaultMetaHeuristic()};

                var results = CompareMetaHeuristics(Fitness(size), AdamChromosome(size),
                    heuristics, crossover,  100, termination, reinsertion);
                this.AssertIsPerformingLessByRatio(termination, ratio, results[0], results[1]);

            }

         


        }


      



    }
}