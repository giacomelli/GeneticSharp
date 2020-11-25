using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;
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

        private IEnumerable<int> DefaultSizes = Enumerable.Range(1, 5).Select(x => 10 * x);
        private IEnumerable<int> VeryLargeSizes = Enumerable.Range(1, 3).Select(x =>  500 * x);

        [Test()]
        public void Evolve_WOAParams_Stub_Small_Optmization()
        {
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(false, 300, maxValue);
            Evolve_MetaHeuristic_DifferentSizes_Optmization(
                maxValue => new FitnessStub(maxValue) { SupportsParallel = false },
                maxValue => new ChromosomeStub(maxValue, maxValue),
                Enumerable.Range(1, 10).Select(x => 4 * x), metaHeuristic, i => 0.6);
        }

        [Test()]
        public void Evolve_WOA_Stub_Small_Optmization()
        {
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(true, 300, maxValue);
            Evolve_MetaHeuristic_DifferentSizes_Optmization(
                maxValue => new FitnessStub(maxValue) { SupportsParallel = false },
                maxValue => new ChromosomeStub(maxValue, maxValue),
                DefaultSizes, metaHeuristic, i => 0.6);
        }

        [Test()]
        public void Evolve_WOA_KnownFunctions_Small_Optmization()
        {
            var functionHalfRange = 5;
            Func<double, double> getGeneValueFunction =  d => d % functionHalfRange;
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => MetaHeuristicsFactory.WhaleOptimisationAlgorithm<double>(300,
                    geneValue => geneValue, getGeneValueFunction);


            Dictionary<Func<Gene[], double>, Func<int, double>> functionsToSolveWithTargets = new Dictionary<Func<Gene[], double>, Func<int, double>>();
            functionsToSolveWithTargets.Add(genes => KnownFunctionsFactory.Rastrigin(genes.Select(g => g.Value.To<double>()).ToArray()), i => 10 * i);
            functionsToSolveWithTargets.Add(genes => Math.Exp(KnownFunctionsFactory.ReverseAckley(genes.Select(g => g.Value.To<double>()).ToArray())), i => -3.0);

            foreach (var functionToSolve in functionsToSolveWithTargets)
            {
                Evolve_MetaHeuristic_DifferentSizes_Optmization(i => new FunctionFitness<double>(functionToSolve.Key),
                    i => new EquationChromosome<double>(-functionHalfRange, functionHalfRange, i)
                    {
                        GetGeneValueFunction = getGeneValueFunction
                    },
                    DefaultSizes, metaHeuristic, functionToSolve.Value);
            }
            
        }


        [Test()]
        public void Compare_WOA_OnePoint_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new OnePointCrossover(2);
            Compare_WOA_Crossover_KnownFunctions_LargerFitness(crossover, DefaultSizes, new[] { 1.5, 5});
        }



        [Test()]
        public void Compare_WOA_Uniform_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new UniformCrossover();
            Compare_WOA_Crossover_KnownFunctions_LargerFitness(crossover, DefaultSizes, new[] { 1.05, 10.0});
        }


        [Test()]
        public void Compare_WOA_Uniform_Stub_Large_LargerFitness()
        {
           
            var crossover = new UniformCrossover();
            Compare_WOAReduced_Crossover_Small_ChromosomeStub_LargerFitness(crossover, VeryLargeSizes);

        }

        [Test()]
        public void Compare_WOA_OnePoint_Stub_Small_LargerFitness()
        {
            var crossover = new OnePointCrossover(2);
            Compare_WOAReduced_Crossover_Small_ChromosomeStub_LargerFitness(crossover, DefaultSizes);

        }

        
      

        [Test()]
        public void Compare_WOA_WOAParams_Stub_Large_MoreGenerations()
        {
            Func<int,IMetaHeuristic> originalMetaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(false, 300, maxValue);
            Func<int, IMetaHeuristic> reducedMetaHeuristic = maxValue => GetWhaleHeuristicForChromosomStub(true, 300, maxValue);
            var crossover = new UniformCrossover();
            CompareMetaHeuristics_DifferentSizes(result => Convert.ToDouble(result.Population.GenerationsNumber), 1, VeryLargeSizes, maxValue =>new FitnessStub(maxValue) { SupportsParallel = false }, maxValue => new ChromosomeStub(maxValue, maxValue) , originalMetaHeuristic, reducedMetaHeuristic, crossover, 100, 1, 2000, 100, TimeSpan.FromSeconds(2));
            
        }


        #region private methods

       

        private void Evolve_MetaHeuristic_DifferentSizes_Optmization(Func<int, IFitness> fitness, Func<int, IChromosome> adamChromosome, IEnumerable<int> sizes, Func<int, IMetaHeuristic> metaHeuristic, Func<int, double> minFitness)
        {

            var compoundResults = new List<EvolutionResult>();
            var minFitnesses = new List<double>();
            foreach (var maxValue in sizes)
            {
                var result = EvolveMetaHeuristic( fitness, adamChromosome,  metaHeuristic(maxValue), maxValue, maxValue, 100, double.MaxValue, 2000, 100, TimeSpan.FromSeconds(2));
                compoundResults.Add(result);
                minFitnesses.Add(minFitness(maxValue));
                
            }

            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i], minFitnesses[i]);
            }

        }

        public void Compare_WOAReduced_Crossover_Small_ChromosomeStub_LargerFitness(ICrossover crossover, IEnumerable<int> sizes)
        {
            Func<int, IMetaHeuristic> standardHeuristic = i => new DefaultMetaHeuristic();
            Func<int, IMetaHeuristic> metaHeuristic = i => GetWhaleHeuristicForChromosomStub(true, 300, i);

            Func<int, IFitness> fitness = i => new FitnessStub(i) { SupportsParallel = false };
            Func<int, IChromosome> adamChromosome = i => new ChromosomeStub(i, i);

            CompareMetaHeuristics_DifferentSizes(result => result.Population.BestChromosome.Fitness.Value,
                1,
                sizes,
                fitness,
                adamChromosome,
                standardHeuristic,
                metaHeuristic,
                crossover, 100, 1, 2000, 100, TimeSpan.FromSeconds(5));


        }

        private void Compare_WOA_Crossover_KnownFunctions_LargerFitness(ICrossover crossover, IEnumerable<int> sizes, IList<double> progressRatio)
        {

            var functionHalfRange = 5;
            //Func<Gene[], double> functionToSolve = genes => KnownFunctionsFactory.Rastrigin(genes.Select(g => g.Value.To<double>()).ToArray());
            Func<double, double> getGeneValueFunction = d => Math.Sign(d) * Math.Min(Math.Abs(d), functionHalfRange);

            Func<int, IChromosome> adamChromosome = i => new EquationChromosome<double>(-functionHalfRange, functionHalfRange, i);

            Dictionary<Func<Gene[], double>, double> functionsToSolveWithRatios = new Dictionary<Func<Gene[], double>,  double>();
            functionsToSolveWithRatios.Add(genes => KnownFunctionsFactory.Rastrigin(genes.Select(g => g.Value.To<double>()).ToArray()), progressRatio[0]);
            functionsToSolveWithRatios.Add(genes => Math.Exp(KnownFunctionsFactory.ReverseAckley(genes.Select(g => g.Value.To<double>()).ToArray())), progressRatio[1]);


            Func<int, IMetaHeuristic> standardHeuristic = i => new DefaultMetaHeuristic();
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => MetaHeuristicsFactory.WhaleOptimisationAlgorithm<double>(300,
                geneValue => geneValue, getGeneValueFunction);

            foreach (var functionToSolve in functionsToSolveWithRatios)
            {
                Func<int, IFitness> fitness = i => new FunctionFitness<double>(functionToSolve.Key);
                CompareMetaHeuristics_DifferentSizes(result => result.Population.BestChromosome.Fitness.Value,
                    functionToSolve.Value,
                    sizes,
                    fitness,
                    adamChromosome,
                    standardHeuristic,
                    metaHeuristic,
                    crossover, 100, int.MaxValue, 2000, 100, TimeSpan.FromSeconds(2));
            }

        }


        private EvolutionResult EvolveMetaHeuristic(Func<int, IFitness> fitness, Func<int, IChromosome> adamChromosome, IMetaHeuristic metaHeuristic, int maxValue, int chromosomeLength, int populationSize, double minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {
            var crossover = new OnePointCrossover(2);
            var target = InitGa(fitness(maxValue), adamChromosome(maxValue), crossover, maxValue, chromosomeLength, populationSize, minFitness, maxNbGenerations,
                stagnationNb, maxTimeEvolving);
            target.Metaheuristic = metaHeuristic;
            target.Start();
            return new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };

        }


        private void CompareMetaHeuristics_DifferentSizes(Func<EvolutionResult, double> scoreFunction, double ratio, IEnumerable<int> sizes, Func<int, IFitness> fitness, Func<int, IChromosome> adamChromosome,  Func<int, IMetaHeuristic> metaHeuristic1, Func<int, IMetaHeuristic> metaHeuristic2, ICrossover crossover, int populationSize, double minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var compoundResults = new List<IList<EvolutionResult>>();
            foreach (var size in sizes)
            {
                var results = CompareMetaHeuristics(fitness(size), adamChromosome(size), metaHeuristic1(size), metaHeuristic2(size), crossover, size, size, populationSize, minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
                compoundResults.Add(results);
                //AssertEvolution(results[0], minFitness);
                //AssertEvolution(results[1], minFitness);
            }

            //reduced heuristic faster
            var meanRatio = compoundResults.Sum(c => scoreFunction(c[1])/ scoreFunction(c[0]))/ compoundResults.Count;
            

            Assert.GreaterOrEqual(meanRatio, ratio);

        }

        private IList<EvolutionResult> CompareMetaHeuristics(IFitness  fitness,  IChromosome adamChromosome, IMetaHeuristic metaHeuristic1, IMetaHeuristic metaHeuristic2, ICrossover crossover, int maxValue, int chromosomeLength, int populationSize, double minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {

            var toReturn = new List<EvolutionResult>(2);
            //var chromosome = new ChromosomeStub(maxValue, chromosomeLength);
            var target = InitGa(fitness, adamChromosome, crossover, maxValue, chromosomeLength, populationSize, minFitness, maxNbGenerations,
                stagnationNb, maxTimeEvolving);
            target.Metaheuristic = metaHeuristic1;

            target.Start();
            var firstResult = new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };
            toReturn.Add(firstResult);

            target.Reset(new Population(target.Population.MinSize, target.Population.MaxSize, adamChromosome) { GenerationStrategy = target.Population.GenerationStrategy });
            target.Metaheuristic = metaHeuristic2;
            target.Start();
            var secondResult = new EvolutionResult() { Population = target.Population, TimeEvolving = target.TimeEvolving };
            toReturn.Add(secondResult);

            return toReturn;

        }


        private GeneticAlgorithm InitGa(IFitness fitness, IChromosome adamChromosome, ICrossover crossover, int maxValue, int chromosomeLength, int populationSize, double minFitness, int maxNbGenerations, int stagnationNb, TimeSpan maxTimeEvolving)
        {
            var selection = new EliteSelection();
            var mutation = new UniformMutation();
            var generationStragegy = new TrackingGenerationStrategy();
            var initialPopulation = new Population(populationSize, populationSize, adamChromosome) { GenerationStrategy = generationStragegy };
            var target = new GeneticAlgorithm(initialPopulation, fitness, selection, crossover, mutation);
            target.Reinsertion = new FitnessBasedElitistReinsertion();
            target.Termination = new OrTermination(
                new FitnessThresholdTermination(minFitness),
                new GenerationNumberTermination(maxNbGenerations),
                new FitnessStagnationTermination(stagnationNb),
                new TimeEvolvingTermination(maxTimeEvolving));
            target.MutationProbability = 0.1f;
            return target;
        }

        private IMetaHeuristic GetWhaleHeuristicForChromosomStub(bool reduced, int maxOperations, int maxValue)
        {

            Func<int, Double> fromGene = geneValue => (double) geneValue;
            Func<Double, int> ToGene =  ChromosomeStub.GeneFromDouble(maxValue);

            if (!reduced)
            {
                return MetaHeuristicsFactory.WhaleOptimisationAlgorithmWithParams<int>(maxOperations, fromGene, ToGene);
            }
            else
            {
                return MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(maxOperations, fromGene, ToGene);
            }

        }



        private void AssertEvolution(EvolutionResult result, double minLastFitness)
        {
            var lastFitness = double.MinValue;
            foreach (var g in result.Population.Generations)
            {
                Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
                lastFitness = g.BestChromosome.Fitness.Value;
            }

            Assert.GreaterOrEqual(lastFitness, minLastFitness);

        }

        [DebuggerDisplay("TimeEvolving:{TimeEvolving}, Population:{Population}")]
        private class EvolutionResult
        {
            public IPopulation Population { get; set; }

            public TimeSpan TimeEvolving { get; set; }
        }



        #endregion



    }
}
