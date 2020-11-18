﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        public void Evolve_WhaleOptimisation_StubChromosomeSeveralSizes_BetterThanOnePointCrossover()
        {
            var crossover = new OnePointCrossover(2);
            var compoundResults = new List<IList<EvolutionResult>>();
            for (int i = 1; i < 10; i++)
            {
                var results = CompareWhaleEvolutionsToCrossover(crossover, 4 * i, 4 * i, 50, 1, 2000, 100, TimeSpan.FromSeconds(2));
                compoundResults.Add(results);
                // We ensure that both GAs optimize. 
                AssertEvolution(results[0],  0.6);
                AssertEvolution(results[1], 0.7);
            }
            //Whale metaheuristic better
            compoundResults.Each(results=> Assert.LessOrEqual(results[0].Population.BestChromosome.Fitness, results[1].Population.BestChromosome.Fitness));
            
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




        private IList<EvolutionResult> CompareWhaleEvolutionsToCrossover(ICrossover crossover, int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var toReturn = new List<EvolutionResult>(2);

            var selection = new EliteSelection();
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
                MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(100,
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
