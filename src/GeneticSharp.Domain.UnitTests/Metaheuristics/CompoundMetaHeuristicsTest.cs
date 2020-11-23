using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
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
        public void Evolve_WhaleOptimisation_StubChromosomeSeveralSizes_Optmization()
        {
            for (int i = 1; i < 20; i++)
            {
                var maxValue = 4 * i;
                var metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(100,
                    geneValue => (double)geneValue,
                    ChromosomeStub.GeneFromDouble(maxValue));

                var result = EvolveMetaHeuristicStubChromosome(metaHeuristic, maxValue, maxValue, 100, 1, 2000, 100, TimeSpan.FromSeconds(2));
                // We ensure that both GAs optimize. 
                AssertEvolution(result, 0.6);
                
            }
        }

        [Test()]
        public void Evolve_WhaleReducedOptimisation_StubChromosomeSeveralSizes_Optmization()
        {
            for (int i = 1; i < 20; i++)
            {

                var maxValue = 4 * i;
                var metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithmReduced<int>(100,
                    geneValue => (double)geneValue,
                    ChromosomeStub.GeneFromDouble(maxValue));

                var result = EvolveMetaHeuristicStubChromosome(metaHeuristic, maxValue, maxValue, 100, 1, 2000, 100, TimeSpan.FromSeconds(2));
                // We ensure that both GAs optimize. 
                AssertEvolution(result, 0.6);

            }
        }

        [Test()]
        public void Evolve_WhaleOptimisation_StubChromosomeSeveralSizes_CloseToUniformCrossover()
        {
           
            var crossover = new UniformCrossover();
            Compare_WhaleOptimisation_StubChromosomeSeveralSizes_CloseToCrossover(crossover, 1.1);

        }

        [Test()]
        public void Evolve_WhaleOptimisation_StubChromosomeSeveralSizes_BetterThanOnePointCrossover()
        {
            var crossover = new OnePointCrossover(2);
           Compare_WhaleOptimisation_StubChromosomeSeveralSizes_CloseToCrossover(crossover, 1);
            
        }

        [Test()]
        public void Evolve_WhaleOptimisationReduced_StubChromosomeSeveralSizes_FasterThanWithoutReduction()
        {

            var compoundResults = new List<IList<EvolutionResult>>();
            for (int i = 1; i < 20; i++)
            {

                var maxValue = 4 * i;
                var originalMetaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(300,
                    geneValue => (double)geneValue,
                    ChromosomeStub.GeneFromDouble(maxValue));
                var reducedMetaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithmReduced<int>(300,
                    geneValue => (double)geneValue,
                    ChromosomeStub.GeneFromDouble(maxValue));
                //EvolveMetaHeuristicStubChromosome(reducedMetaHeuristic, maxValue, maxValue, 100, 1, 2000, 100,
                //    TimeSpan.FromSeconds(2));

                var results = CompareMetaHeuristics(originalMetaHeuristic, reducedMetaHeuristic, maxValue, maxValue, 100, 1, 2000, 100, TimeSpan.FromSeconds(2));
                compoundResults.Add(results);
                // We ensure that both GAs optimize. 
                AssertEvolution(results[0], 0.6);
                AssertEvolution(results[1], 0.6);
            }
            //reduced heuristic faster
            var sumNative = TimeSpan.FromTicks(compoundResults.Sum(c => c[0].TimeEvolving.Ticks));
            var sumReduced = TimeSpan.FromTicks(compoundResults.Sum(c => c[1].TimeEvolving.Ticks));

            Assert.GreaterOrEqual(TimeSpan.FromTicks(sumNative.Ticks * 110 / 100), sumReduced);

            
        }



        private EvolutionResult EvolveMetaHeuristicStubChromosome(IMetaHeuristic metaHeuristic, int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {
            var crossover = new OnePointCrossover(2);
            var chromosome = new ChromosomeStub(maxValue, chromosomeLength);
            var target = InitGa(chromosome, crossover, maxValue, chromosomeLength, populationSize, minFitness, maxNbGenerations,
                stagnationNb, maxTimeEvolving);
            target.Metaheuristic = metaHeuristic;
            target.Start();
            return new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };

        }

        

        private void Compare_WhaleOptimisation_StubChromosomeSeveralSizes_CloseToCrossover(ICrossover crossover, double percentDiff)
        {

            var compoundResults = new List<IList<EvolutionResult>>();
            for (int i = 1; i < 10; i++)
            {
                var maxValue = 4 * i;
                var metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(100,
                    geneValue => (double) geneValue,
                    ChromosomeStub.GeneFromDouble(maxValue));
                var results = CompareMetaHeuristicToCrossover(metaHeuristic, crossover, maxValue, maxValue, 100, 1, 2000, 100, TimeSpan.FromSeconds(2));
                compoundResults.Add(results);
                // We ensure that both GAs optimize. 
                AssertEvolution(results[0], 0.6);
                AssertEvolution(results[1], 0.6);
            }
            //Whale metaheuristic better
            compoundResults.Each(results => Assert.LessOrEqual(results[0].Population.BestChromosome.Fitness, results[1].Population.BestChromosome.Fitness * percentDiff));

        }



        private IList<EvolutionResult> CompareMetaHeuristicToCrossover(IMetaHeuristic metaHeuristic, ICrossover crossover, int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var toReturn = new List<EvolutionResult>(2);

            var chromosome = new ChromosomeStub(maxValue, chromosomeLength);
            var target = InitGa(chromosome, crossover, maxValue, chromosomeLength, populationSize, minFitness, maxNbGenerations,
                stagnationNb, maxTimeEvolving);

            target.Start();
            var firstResult = new EvolutionResult(){Population = target.Population, TimeEvolving = target.TimeEvolving};
            toReturn.Add(firstResult);

            target.Reset(new Population(target.Population.MinSize, target.Population.MaxSize, chromosome) { GenerationStrategy = target.Population.GenerationStrategy });
            //target.Metaheuristic =
            //    MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(100,
            //        i => (double)i,
            //        d => ((int)Math.Abs(d)) % maxValue + 1);
            target.Metaheuristic = metaHeuristic;
            target.Start();
            var secondResult = new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };
            toReturn.Add(secondResult);

            return toReturn;

        }

        private IList<EvolutionResult> CompareMetaHeuristics(IMetaHeuristic metaHeuristic1, IMetaHeuristic metaHeuristic2, int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var toReturn = new List<EvolutionResult>(2);
            var crossover = new OnePointCrossover(2);
            var chromosome = new ChromosomeStub(maxValue, chromosomeLength);
            var target = InitGa(chromosome,crossover, maxValue, chromosomeLength, populationSize, minFitness, maxNbGenerations,
                stagnationNb, maxTimeEvolving);
            target.Metaheuristic = metaHeuristic1;

            target.Start();
            var firstResult = new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };
            toReturn.Add(firstResult);

            target.Reset(new Population(target.Population.MinSize, target.Population.MaxSize, chromosome) { GenerationStrategy = target.Population.GenerationStrategy });
            target.Metaheuristic = metaHeuristic2;
            target.Start();
            var secondResult = new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };
            toReturn.Add(secondResult);

            return toReturn;

        }


        private GeneticAlgorithm InitGa(IChromosome adamChromosome, ICrossover crossover, int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {
            var selection = new EliteSelection();
            var mutation = new UniformMutation();
            var generationStragegy = new TrackingGenerationStrategy();
            var initialPopulation = new Population(populationSize, populationSize, adamChromosome) { GenerationStrategy = generationStragegy };
            var target = new GeneticAlgorithm(initialPopulation,
                new FitnessStub(maxValue) { SupportsParallel = false }, selection, crossover, mutation);
            target.Reinsertion = new FitnessBasedElitistReinsertion();
            target.Termination = new OrTermination(
                new FitnessThresholdTermination(minFitness),
                new FitnessThresholdTermination(1),
                new GenerationNumberTermination(maxNbGenerations),
                new FitnessStagnationTermination(stagnationNb),
                new TimeEvolvingTermination(maxTimeEvolving));
            target.MutationProbability = 0.1f;
            return target;
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


        private class EvolutionResult
        {
            public IPopulation Population { get; set; }

            public TimeSpan TimeEvolving { get; set; }
        }


    }
}
