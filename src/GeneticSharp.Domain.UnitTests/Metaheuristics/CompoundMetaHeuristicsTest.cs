using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Framework.Collections;
using GeneticSharp.Infrastructure.Framework.Commons;
using GeneticSharp.Infrastructure.Framework.Texts;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture()]
    [Category("MetaHeuristics")]
    class CompoundMetaHeuristicsTest
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }


        [Test()]
        public void Evolve_WhaleOptimisation_StubChromosomeSeveralSizes_BetterThanRegularGA()
        {
            for (int i = 1; i < 11; i++)
            {
                var results = CompareEvolutions(4 * i, 4 * i, 100, 1, 5000, 100, TimeSpan.FromSeconds(2));

                // We ensure that both GAs optimize. What is expected to keep a good final fitness as the problem size grows, unlike regular GA
                AssertEvolution(results[0],  Math.Max(0.6, 0.9 -(0.1*i)));
                AssertEvolution(results[1], Math.Max(0.7, 1 -(0.01*Math.Pow(i,2))));

                //Whale metaheuristic better
                Assert.LessOrEqual(results[0].Population.BestChromosome.Fitness, results[1].Population.BestChromosome.Fitness);
            }
        }



        private void AssertEvolution(EvolutionResult result, double minLastFitness)
        {
            var lastFitness = 0.0;
            foreach (var g in result.Population.Generations)
            {
                Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
                lastFitness = g.BestChromosome.Fitness.Value;
            }

            Assert.GreaterOrEqual(lastFitness, minLastFitness);

        }




        private IList<EvolutionResult> CompareEvolutions(int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var toReturn = new List<EvolutionResult>(2);

            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub(maxValue, chromosomeLength);
            var generationStragegy = new TrackingGenerationStrategy();
            var initialPopulation = new Population(populationSize, populationSize, chromosome) { GenerationStrategy = generationStragegy };
            var target = new GeneticAlgorithm(initialPopulation,
                new FitnessStub(maxValue) { SupportsParallel = false }, selection, crossover, mutation);
            target.Termination = new OrTermination(
                new FitnessThresholdTermination(minFitness), 
                new FitnessThresholdTermination(1), 
                new GenerationNumberTermination(maxNbGenerations), 
                new FitnessStagnationTermination(stagnationNb), 
                new TimeEvolvingTermination(maxTimeEvolving));
            target.MutationProbability = 0.2f;
            
            target.Start();
            var firstResult = new EvolutionResult(){Population = initialPopulation, TimeEvolving = target.TimeEvolving};
            toReturn.Add(firstResult);

            target.Reset(new Population(initialPopulation.MinSize, initialPopulation.MaxSize, chromosome) { GenerationStrategy = generationStragegy });
            target.Metaheuristic =
                MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(50,
                    i => i.To<double>(),
                    d => Math.Abs(d).To<int>() % maxValue + 1);
            target.Start();
            var secondResult = new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };
            toReturn.Add(secondResult);

            return toReturn;

        }




        private class EvolutionResult
        {
            public IPopulation Population { get; set; }

            public TimeSpan TimeEvolving { get; set; }
        }


    }
}
