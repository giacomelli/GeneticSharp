using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [Category("MetaHeuristics")]
    public abstract class MetaHeuristicTestBase
    {

        protected IEnumerable<int> SmallSizes = Enumerable.Range(1, 3).Select(x => 10 * x);
        protected IEnumerable<int> LargeSizes = Enumerable.Range(1, 3).Select(x => 500 * x);


        protected OrTermination GetTermination(double minFitness, int maxNbGenerations, int stagnationNb,
            TimeSpan maxTimeEvolving)
        {
            var termination = new OrTermination(
                new FitnessThresholdTermination(minFitness),
                new GenerationNumberTermination(maxNbGenerations),
                new FitnessStagnationTermination(stagnationNb),
                new TimeEvolvingTermination(maxTimeEvolving));
            return termination;
        }


        protected IList<IChromosome> GetStubs(int nbChromosomes)
        {
            IList<IChromosome> stubParents = Enumerable.Range(0, nbChromosomes).Select(i => (IChromosome)new ChromosomeStub(20, 10).Initialized()).ToList();
            for (int i = 0; i < stubParents.Count; i++)
            {
                for (int g = 0; g < stubParents[0].Length; g++)
                {
                    stubParents[i].ReplaceGene(g, new Gene(i));
                }
            }

            return stubParents;
        }


        protected List<IMetaHeuristic> GetGeometricCrossoverStubs(int nbPhases)
        {
            var geometricHeuristics = new List<IMetaHeuristic>();
            for (int i = 0; i < nbPhases; i++)
            {
                var iClosure = i;
                var geomCrossover = new GeometricCrossover<int>().WithGeometricOperator(geneValues => iClosure);
                var geomHeuristic = new CrossoverHeuristic().WithCrossover(geomCrossover);

                geometricHeuristics.Add(geomHeuristic);

            }

            return geometricHeuristics;
        }

        protected List<Func<Gene[], double>> GetKnownFunctions()
        {
            var toReturn = new List<Func<Gene[], double>>
            {
                genes => KnownFunctionsFactory.Rastrigin(genes.Select(g => g.Value.To<double>()).ToArray()),
                genes => 1 /
                         (1 - KnownFunctionsFactory.ReverseAckley(genes.Select(g => g.Value.To<double>()).ToArray())),
                genes => 1 / (1 -
                              KnownFunctionsFactory.ReverseRosenbrock(genes.Select(g => g.Value.To<double>()).ToArray())
                    ),
                genes => KnownFunctionsFactory.ReverseLevy(genes.Select(g => g.Value.To<double>()).ToArray())
            };
            return toReturn;
        }

        protected void AssertIsPerformingLessByRatio(ITermination termination, double ratio, EvolutionResult result1, EvolutionResult result2)
        {
            switch (termination.GetType().Name)
            {
                case nameof(TimeEvolvingTermination):
                case nameof(FitnessStagnationTermination):
                    Assert.GreaterOrEqual(result1.Fitness * ratio, result2.Fitness);
                    break;
                case nameof(FitnessThresholdTermination):
                case nameof(GenerationNumberTermination):
                    Assert.GreaterOrEqual(TimeSpan.FromTicks(Convert.ToInt64(result1.TimeEvolving.Ticks * ratio)), result2.TimeEvolving);
                    break;
                default: throw new InvalidOperationException("Termination not supported");
            }
        }

        protected void AssertEvolution(EvolutionResult result, double minLastFitness)
        {
            var lastFitness = double.MinValue;
            foreach (var g in result.Population.Generations)
            {
                Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
                lastFitness = g.BestChromosome.Fitness.Value;
            }

            Assert.GreaterOrEqual(lastFitness, minLastFitness);
        }



        protected IList<(EvolutionResult result, double minFitness)> EvolveMetaHeuristicDifferentSizes(Func<int, IFitness> fitness, Func<int, IChromosome> adamChromosome, IEnumerable<int> sizes, Func<int, IMetaHeuristic> metaHeuristic, Func<int, double> minFitness, IReinsertion reinsertion)
        {

            var toReturn = new List<(EvolutionResult result, double minFitness)>();
            var minFitnesses = new List<double>();
            foreach (var maxValue in sizes)
            {
                //Population Size
                var populationSize = 100;

                //Termination
                var terminationFitness = double.MaxValue;
                int maxNbGenerations = 2000;
                int stagnationNb = 100;
                TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2);
                var termination = GetTermination(terminationFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);

                var result = EvolveMetaHeuristic(fitness, adamChromosome, metaHeuristic(maxValue), maxValue,  populationSize, termination, reinsertion);
                toReturn.Add((result, minFitness(maxValue)));
            }

            return toReturn;

        }


        protected virtual EvolutionResult EvolveMetaHeuristic(Func<int, IFitness> fitness, Func<int, IChromosome> adamChromosome, IMetaHeuristic metaHeuristic, int maxValue, int populationSize, ITermination termination, IReinsertion reinsertion)
        {
            var crossover = new OnePointCrossover(2);
            
            var target = InitGa(metaHeuristic, fitness(maxValue), adamChromosome(maxValue), crossover,  populationSize, termination, reinsertion);
            target.Start();
            return new EvolutionResult { Population = target.Population, TimeEvolving = target.TimeEvolving };

        }

        protected List<IList<(EvolutionResult result1, EvolutionResult result2)>> CompareMetaHeuristicsKnownFunctionsDifferentSizes(double maxCoordinate, Func<int, IMetaHeuristic> metaHeuristic1, Func<int, IMetaHeuristic> metaHeuristic2, ICrossover crossover, IEnumerable<int> sizes, ITermination termination, IReinsertion reinsertion)
        {
            double GetGeneValueFunction(double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);
            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-maxCoordinate, maxCoordinate, i) {GetGeneValueFunction = GetGeneValueFunction};

            var knownFunctions = GetKnownFunctions();


            //Population Size
            var populationSize = 100;

            

            var resultsByFunctionBySize = new List<IList<(EvolutionResult result1, EvolutionResult result2)>>();

            foreach (var functionToSolve in knownFunctions)
            {
                IFitness Fitness(int i) => new FunctionFitness<double>(functionToSolve);

                IList<(EvolutionResult result1, EvolutionResult result2)> results;
                results = CompareMetaHeuristicsDifferentSizes(
                    sizes,
                    Fitness,
                    AdamChromosome,
                    metaHeuristic1,
                    metaHeuristic2,
                    crossover, populationSize, termination, reinsertion);

                resultsByFunctionBySize.Add(results);

            }

            return resultsByFunctionBySize;

        }



        protected virtual IList<(EvolutionResult result1, EvolutionResult result2)> CompareMetaHeuristicsDifferentSizes(IEnumerable<int> sizes, 
            Func<int, IFitness> fitness, 
            Func<int, IChromosome> adamChromosome, 
            Func<int, IMetaHeuristic> metaHeuristic1, 
            Func<int, IMetaHeuristic> metaHeuristic2, 
            ICrossover crossover, 
            int populationSize,
            ITermination termination, IReinsertion reinsertion)
        {


            var compoundResults = new List<(EvolutionResult result1, EvolutionResult result2)>();
            foreach (var size in sizes)
            {
                var heuristics = new List<IMetaHeuristic> {metaHeuristic1(size), metaHeuristic2(size)};

                var results = CompareMetaHeuristics(fitness(size), adamChromosome(size), heuristics, crossover, populationSize, termination, reinsertion);
                compoundResults.Add((results[0], results[1]));
            }

            return compoundResults;

        }

     
        protected virtual IList<EvolutionResult> CompareMetaHeuristics(IFitness fitness, IChromosome adamChromosome, IList<IMetaHeuristic> metaHeuristics, ICrossover crossover,  int populationSize, ITermination termination, IReinsertion reinsertion)
        {

            var toReturn = new List<EvolutionResult>();

            foreach (var metaHeuristic in metaHeuristics)
            {
                var target = InitGa(metaHeuristic, fitness, adamChromosome, crossover, populationSize, termination, reinsertion);

                target.Start();
                var firstResult = target.GetResult();
                toReturn.Add(firstResult);
            }

            return toReturn;
        }



        protected virtual GeneticAlgorithm InitGa(IMetaHeuristic metaHeuristic, IFitness fitness, IChromosome adamChromosome, ICrossover crossover, int populationSize, ITermination termination, IReinsertion reinsertion)
        {
            var selection = new EliteSelection();
            var mutation = new UniformMutation();
            var generationStragegy = new PerformanceGenerationStrategy();
            var initialPopulation = new Population(populationSize, populationSize, adamChromosome) { GenerationStrategy = generationStragegy };
            GeneticAlgorithm target = null;
            if (metaHeuristic != null)
            {
                var metaTarget = new MetaGeneticAlgorithm(initialPopulation, fitness, selection, crossover, mutation,
                    metaHeuristic);
                target = metaTarget;
            }
            else
            {
                target = new GeneticAlgorithm(initialPopulation, fitness, selection, crossover, mutation);
            }

            if (reinsertion is PureReinsertion)
            {
                target.CrossoverProbability = 1;
            }

            target.Reinsertion = reinsertion;
            target.Termination = termination;
            target.MutationProbability = 0.1f;

            return target;
        }




    }
}