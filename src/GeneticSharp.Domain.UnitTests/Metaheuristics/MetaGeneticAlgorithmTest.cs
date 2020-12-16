using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Terminations;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class MetaGeneticAlgorithmTest: MetaHeuristicTestBase
    {
        [Test]
        public void Compare_RegularGA_ChromosomeStub_DifferentSizes_ManyGenerations_DurationBounds()
        {
            var repeatNb = 10;
            var testParams = new List<(int size, double ratio)>
            {
                (10, 1.1), (100, 1.1), (500, 1.1), (5000, 1.1)
            };

            IFitness Fitness(int i) => new FitnessStub(i) {SupportsParallel = false};
            IChromosome AdamChromosome(int i) => new ChromosomeStub(i, i);

            var crossover = new UniformCrossover();
            
            var reinsertion = new FitnessBasedElitistReinsertion();

            Func<IEvolutionResult, IEvolutionResult, int> resultComparer = (IEvolutionResult result1, IEvolutionResult result2) => Convert.ToInt32(result1.TimeEvolving.Ticks - result2.TimeEvolving.Ticks);

            var heuristics = new List<IMetaHeuristic> { null, new DefaultMetaHeuristic() };
            foreach (var (size, ratio) in testParams)
            {
                
                var traditionalGaResult = new MeanEvolutionResult { ResultComparer = resultComparer, SkipExtremaPercentage = 0.2};
                var metaGaResult = new MeanEvolutionResult { ResultComparer = resultComparer, SkipExtremaPercentage = 0.2 };
                var nbGenerations = 10000 / size;
                var termination = new GenerationNumberTermination(nbGenerations);
                for (int i = 0; i < repeatNb; i++)
                {
                    var results = CompareMetaHeuristics(Fitness(size), AdamChromosome(size),
                        heuristics, crossover, 100, termination, reinsertion);
                    //if (i>0)//Skip first evolution
                    //{
                        traditionalGaResult.Results.Add(results[0]);
                        metaGaResult.Results.Add(results[1]);
                    //}
                    
                }
                
                AssertIsPerformingLessByRatio(termination, ratio, traditionalGaResult , metaGaResult);

            }

         


        }


      



    }
}