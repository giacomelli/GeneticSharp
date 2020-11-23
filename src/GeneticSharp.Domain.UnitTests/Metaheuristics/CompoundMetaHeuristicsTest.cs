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

        private IEnumerable<int> DefaultStubSizes = Enumerable.Range(1, 10).Select(x => 4 * x);
        private IEnumerable<int> VeryLargeSizes = Enumerable.Range(1, 3).Select(x =>  500 * x);

        [Test()]
        public void Evolve_WhaleOptimisation_Small_StubChromosomes_Optmization()
        {
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(false, 300, maxValue);
            Evolve_MetaHeuristic_StubChromosomeSeveralSizes_Optmization(Enumerable.Range(1, 10).Select(x => 4 * x), metaHeuristic, 0.6);
        }

        [Test()]
        public void Evolve_WhaleOptimisationReduced_Small_StubChromosomes_Optmization()
        {
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(true, 300, maxValue);
            Evolve_MetaHeuristic_StubChromosomeSeveralSizes_Optmization(DefaultStubSizes, metaHeuristic, 0.6);
        }

        [Test()]
        public void Compare_WhaleOptimisation_Large_ChromosomeStub_BetterThanUniformCrossover()
        {
           
            var crossover = new UniformCrossover();
            CompareWhaleOptimisationToDefaultWithCrossover(VeryLargeSizes,crossover, 0.5,1, TimeSpan.FromSeconds(5));

        }

        [Test()]
        public void Compare_WhaleOptimisation_Small_ChromosomeStub_BetterThanOnePointCrossover()
        {
            var crossover = new OnePointCrossover(2);
           CompareWhaleOptimisationToDefaultWithCrossover(DefaultStubSizes,crossover, 0.6,1, TimeSpan.FromSeconds(2));
            
        }

        [Test()]
        public void Compare_WhaleOptimisationReduced_StubChromosomeSeveralSizes_MoreGenerationsThanWithoutReduction()
        {
            Func<int,IMetaHeuristic> originalMetaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(false, 300, maxValue);
            Func<int, IMetaHeuristic> reducedMetaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(true, 300, maxValue);
            var crossover = new UniformCrossover();
            CompareMetaHeuristics(result => Convert.ToDouble(-result.Population.GenerationsNumber), 1,  VeryLargeSizes, originalMetaHeuristic, reducedMetaHeuristic, crossover, 100, 1, 2000, 100, TimeSpan.FromSeconds(2));
            
        }


        #region private methods

        private IMetaHeuristic GetWhaleHeuristicForChromosomStub(bool reduced, int maxOperations, int maxValue)
        {
            if (!reduced)
            {
                return MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(maxOperations,
                    geneValue => (double)geneValue,
                    ChromosomeStub.GeneFromDouble(maxValue));
            }
            else
            {
                return MetaHeuristicsFactory.WhaleOptimisationAlgorithmReduced<int>(maxOperations,
                    geneValue => (double)geneValue,
                    ChromosomeStub.GeneFromDouble(maxValue));
            }

        }



        private void Evolve_MetaHeuristic_StubChromosomeSeveralSizes_Optmization(IEnumerable<int> sizes, Func<int, IMetaHeuristic> metaHeuristic, double minFitness)
        {

            var compoundResults = new List<IList<EvolutionResult>>();
            foreach (var maxValue in sizes)
            {
                var result = EvolveMetaHeuristicChromosomeStub(metaHeuristic(maxValue), maxValue, maxValue, 100, 1, 2000, 100, TimeSpan.FromSeconds(2));
                // We ensure that both GAs optimize. 
                AssertEvolution(result, minFitness);
            }

        }


        private EvolutionResult EvolveMetaHeuristicChromosomeStub(IMetaHeuristic metaHeuristic, int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {
            var crossover = new OnePointCrossover(2);
            var chromosome = new ChromosomeStub(maxValue, chromosomeLength);
            var target = InitGa(chromosome, crossover, maxValue, chromosomeLength, populationSize, minFitness, maxNbGenerations,
                stagnationNb, maxTimeEvolving);
            target.Metaheuristic = metaHeuristic;
            target.Start();
            return new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };

        }



        private void CompareWhaleOptimisationToDefaultWithCrossover(IEnumerable<int> sizes, ICrossover crossover, double minFitness, double percentDiff, TimeSpan maxTimeEvolving)
        {

            var compoundResults = new List<IList<EvolutionResult>>();
            foreach (var maxValue in sizes)
            {
                var standardHeuristic = new DefaultMetaHeuristic();
                var metaHeuristic = GetWhaleHeuristicForChromosomStub(true, 300, maxValue);
                var results = CompareMetaHeuristics(standardHeuristic, metaHeuristic, crossover, maxValue, maxValue, 100, 1, 2000, 100, maxTimeEvolving);
                compoundResults.Add(results);
                // We ensure that both GAs optimize. 
                AssertEvolution(results[0], minFitness);
                AssertEvolution(results[1], minFitness);
            }

            //Whale metaheuristic better
            compoundResults.Each(results => Assert.LessOrEqual(results[0].Population.BestChromosome.Fitness, results[1].Population.BestChromosome.Fitness * percentDiff));

        }

        private void CompareMetaHeuristics(Func<EvolutionResult,double> scoreFunction, double ratio, IEnumerable<int> sizes, Func<int, IMetaHeuristic> metaHeuristic1, Func<int, IMetaHeuristic> metaHeuristic2, ICrossover crossover, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var compoundResults = new List<IList<EvolutionResult>>();
            foreach (var maxValue in sizes)
            {
                var results = CompareMetaHeuristics(metaHeuristic1(maxValue), metaHeuristic2(maxValue), crossover, maxValue, maxValue, populationSize, minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
                compoundResults.Add(results);
            }

            //reduced heuristic faster
            var sumNative = compoundResults.Sum(c => scoreFunction(c[0]));
            var sumReduced = compoundResults.Sum(c => scoreFunction(c[1]));

            Assert.GreaterOrEqual(sumNative * ratio, sumReduced);

        }

        private IList<EvolutionResult> CompareMetaHeuristics(IMetaHeuristic metaHeuristic1, IMetaHeuristic metaHeuristic2, ICrossover crossover, int maxValue, int chromosomeLength, int populationSize, int minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var toReturn = new List<EvolutionResult>(2);
            var chromosome = new ChromosomeStub(maxValue, chromosomeLength);
            var target = InitGa(chromosome, crossover, maxValue, chromosomeLength, populationSize, minFitness, maxNbGenerations,
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



        #endregion



    }
}
