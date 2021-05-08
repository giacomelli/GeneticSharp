using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using GeneticSharp.Extensions.Mathematic.Functions;
using GeneticSharp.Infrastructure.Framework.Commons;
using GeneticSharp.Infrastructure.Framework.Threading;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [Category("MetaHeuristics")]
    public abstract class MetaHeuristicTestBase
    {

        public const bool EnableOperatorsParallelism = false;
        public const bool EnableEvaluatorParallelism = true;

        protected IEnumerable<int> VerySmallSizes = Enumerable.Range(1, 3).Select(x => 5 * x);
        protected IEnumerable<int> SmallSizes = Enumerable.Range(1, 3).Select(x => 20 * x);
        protected IEnumerable<int> LargeSizes = Enumerable.Range(1, 3).Select(x => 200 * x);
        protected IEnumerable<int> VeryLargeSizes = Enumerable.Range(1, 3).Select(x => 5000 * x);


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


        protected IList<IChromosome> GetStubChromosomes(int nbChromosomes)
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
                var geomCrossover = new GeometricCrossover<int>().WithLinearGeometricOperator((geneIndex, geneValues)   => iClosure);
                var geomHeuristic = new CrossoverHeuristic().WithCrossover(geomCrossover);

                geometricHeuristics.Add(geomHeuristic);

            }

            return geometricHeuristics;
        }

        protected List<(string fName, Func<Gene[], double> function)> GetKnownFunctions(bool normalizeFitness)
        {
            var functions = new string[] {
                nameof(KnownFunctions.Ackley),
                nameof(KnownFunctions.Rastrigin) ,
                nameof(KnownFunctions.Levy),
                nameof(KnownFunctions.Rosenbrock),
            };
            var knownFunctions = functions.Select((fName, findex) => ((string fName, Func<Gene[], double> function))(fName, (Func<Gene[], double>) (genes =>
            {
                var knownFunction = KnownFunctions.GetKnownFunctions()[fName];
                var geneValues = genes.Select(g => g.Value.To<double>()).ToArray();
                if (normalizeFitness)
                {
                    return knownFunction.Fitness(geneValues, knownFunction.Function.Shift(-4)(geneValues));
                }
                else
                {
                    //usual functions have minima, whereas we seek to maximize fitness
                    return -knownFunction.Function.Shift(-4)(geneValues);
                }
            }))).ToArray();
            var toReturn = knownFunctions.ToList();


            //Adding custom composite function with partitioned chromosomes and multiplied function values
            
            int aggregatorSeed = -1;
            if (normalizeFitness)
            {
                aggregatorSeed = 1;
            }
           
            Func<double, double, double> compositeAggregate;
            if (normalizeFitness)
            {
                compositeAggregate = (f1, f2) =>  f1 * f2;
            }
            else
            {
                compositeAggregate = (f1, f2) => f1 + (f1 * -f2);
            }
            
            (string fName, Func<Gene[], double> function) compositeFunction = ("composite",
                genes => knownFunctions
                    .Select((functionTuple, functionIndex) => functionTuple.function(genes
                        .Skip(functionIndex * genes.Length / knownFunctions.Count())
                        .Take(genes.Length / toReturn.Count).ToArray())).Aggregate(aggregatorSeed, compositeAggregate));
            toReturn.Add(compositeFunction);
            return toReturn;
        }

        protected void AssertIsPerformingLessByRatio(ITermination termination, double ratio, IEvolutionResult result1, IEvolutionResult result2)
        {
            switch (termination.GetType().Name)
            {
                case nameof(TimeEvolvingTermination):
                case nameof(FitnessStagnationTermination):
                    AssertIsPerformingLessByRatio(EvolutionMeasure.Fitness, ratio, result1, result2);
                    break;
                case nameof(FitnessThresholdTermination):
                case nameof(GenerationNumberTermination):
                    AssertIsPerformingLessByRatio(EvolutionMeasure.Duration, ratio, result1, result2);
                    break;
                default: throw new InvalidOperationException("Termination not supported");
            }
        }

        protected void AssertIsPerformingLessByRatio(EvolutionMeasure comparisonMeasure, double ratio, IEvolutionResult result1, IEvolutionResult result2)
        {
            switch (comparisonMeasure)
            {
                case EvolutionMeasure.Fitness:
                    var measure = Math.Sign(result1.Fitness) * result2.Fitness / result1.Fitness;
                    Assert.LessOrEqual(measure, Math.Sign(result1.Fitness) * ratio );
                    break;
                case EvolutionMeasure.Duration:
                    var timeMeasure = result2.TimeEvolving.Ticks / (double) result1.TimeEvolving.Ticks;
                    Assert.LessOrEqual(timeMeasure, ratio);
                    break;
                default: throw new InvalidOperationException("Termination not supported");
            }
        }

        public enum EvolutionMeasure
        {
            Fitness,
            Duration
        }

        protected void AssertEvolution(IEvolutionResult result, double minLastFitness, bool assertRegularEvolution)
        {
            var lastFitness = double.MinValue;

            if (assertRegularEvolution)
            {
                for (var index = 10; index < result.Population.Generations.Count; index += Math.Max(10, result.Population.Generations.Count-index-1))
                {
                    var g = result.Population.Generations[index];
                    Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
                    lastFitness = g.BestChromosome.Fitness.Value;
                }
            }
            else
            {
                lastFitness = result.Population.BestChromosome.Fitness.Value;
                Assert.GreaterOrEqual(lastFitness, result.Population.Generations[0].BestChromosome.Fitness.Value);
            }
            

            Assert.GreaterOrEqual(lastFitness, minLastFitness);
        }

        public const int SmallPopulationSize = 100;

        protected IList<(IEvolutionResult result, double minFitness)> EvolveMetaHeuristicDifferentSizes(int repeatNb, Func<int, IFitness> fitness, Func<int, IChromosome> adamChromosome, IEnumerable<int> sizes, Func<int, IMetaHeuristic> metaHeuristic, Func<int, double> minFitness, IReinsertion reinsertion)
        {

            var toReturn = new List<(IEvolutionResult result, double minFitness)>();
            var minFitnesses = new List<double>();
            foreach (var maxValue in sizes)
            {
                //Population Size
                var populationSize = SmallPopulationSize;

                //Termination
                var terminationFitness = double.MaxValue;
                int maxNbGenerations = 2000;
                int stagnationNb = 100;
                TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2);
                var termination = GetTermination(terminationFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);

                var result = EvolveMetaHeuristic(repeatNb, fitness, adamChromosome, metaHeuristic(maxValue), maxValue,  populationSize, termination, reinsertion);
                toReturn.Add((result, minFitness(maxValue)));
            }

            return toReturn;

        }


        protected virtual IEvolutionResult EvolveMetaHeuristic(int repeatNb, Func<int, IFitness> fitness, Func<int, IChromosome> adamChromosome, IMetaHeuristic metaHeuristic, int maxValue, int populationSize, ITermination termination, IReinsertion reinsertion)
        {
            var crossover = new OnePointCrossover(2);



            var meanResult = new MeanEvolutionResult { TestSettings = (metaHeuristic, populationSize, termination, reinsertion), SkipExtremaPercentage = 0.2 };
            for (int i = 0; i < repeatNb; i++)
            {
                var target = InitGa(metaHeuristic, fitness(maxValue), adamChromosome(maxValue), crossover, populationSize, termination, reinsertion, true);
                target.Start();
                var result = target.GetResult();
                meanResult.Results.Add(result);
            }
           

           
            return meanResult;

        }

        protected List<IList<(IEvolutionResult result1, IEvolutionResult result2)>> CompareMetaHeuristicsKnownFunctionsDifferentSizes(int repeatNb, double maxCoordinate, Func<int, IMetaHeuristic> metaHeuristic1, Func<int, IMetaHeuristic> metaHeuristic2, ICrossover crossover, IEnumerable<int> sizes, ITermination termination, IReinsertion reinsertion)
        {
            //double GetGeneValueFunction(double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);
            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-maxCoordinate, maxCoordinate, i) ;

            var knownFunctions = GetKnownFunctions(true);


            //Population Size
            var populationSize = 100;

            

            var resultsByFunctionBySize = new List<IList<(IEvolutionResult result1, IEvolutionResult result2)>>();

            foreach (var functionToSolve in knownFunctions)
            {
                IFitness Fitness(int i) => new FunctionFitness<double>(functionToSolve.function);

                IList<(IEvolutionResult result1, IEvolutionResult result2)> results;
                results = CompareMetaHeuristicsDifferentSizes(repeatNb,
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



        protected virtual IList<(IEvolutionResult result1, IEvolutionResult result2)> CompareMetaHeuristicsDifferentSizes(int repeatNb, IEnumerable<int> problemSizes, 
            Func<int, IFitness> fitness, 
            Func<int, IChromosome> adamChromosome, 
            Func<int, IMetaHeuristic> metaHeuristic1, 
            Func<int, IMetaHeuristic> metaHeuristic2, 
            ICrossover crossover, 
            int populationSize,
            ITermination termination, IReinsertion reinsertion)
        {
            var compoundResults = new List<(IEvolutionResult result1, IEvolutionResult result2)>();
            foreach (var size in problemSizes)
            {
                var heuristics = new List<IMetaHeuristic> {metaHeuristic1(size), metaHeuristic2(size)};

                var results = CompareMetaHeuristicsSamePopulation(repeatNb, fitness(size), adamChromosome(size), heuristics, crossover, populationSize, termination, reinsertion);
                compoundResults.Add((results[0], results[1]));
            }
            return compoundResults;
        }
     
        protected virtual IList<IEvolutionResult> CompareMetaHeuristicsSamePopulation(int repeatNb, IFitness fitness, IChromosome adamChromosome, IList<IMetaHeuristic> metaHeuristics, ICrossover crossover,  int populationSize, ITermination termination, IReinsertion reinsertion)
        {
            var toReturn = new List<IEvolutionResult>();
            foreach (var metaHeuristic in metaHeuristics)
            {
                var meanResult = new MeanEvolutionResult { TestSettings = (metaHeuristic, populationSize, termination, reinsertion), SkipExtremaPercentage = 0.2 };
                for (int i = 0; i < repeatNb; i++)
                {
                    var target = InitGa(metaHeuristic, fitness, adamChromosome, crossover, populationSize, termination, reinsertion, EnableOperatorsParallelism);
                    target.Start();
                    var result = target.GetResult();
                    meanResult.Results.Add(result);
                }
                toReturn.Add(meanResult);
            }

            return toReturn;
        }

        protected virtual IList<(string functionName, IList<IList<MeanEvolutionResult>> sizeResults)> EvolveMetaHeuristicsFunctionsTestParams( IEnumerable<(string fName, Func<Gene[], double> function)> functions, IEnumerable<int> sizes, int repeatNb, params 
            (KnownCompoundMetaheuristics kind, double duration, int nbGenerations, bool noMutation, int populationSize,
                IReinsertion reinsertion, bool forceReinsertion)[] testParams)
        {

            

            //Termination
            var minFitness = double.MaxValue;
            int maxNbGenerations = 200;
            int stagnationNb = 100000;

            var crossover = new UniformCrossover();

            var maxCoordinate = 10;

            double GetGeneValueFunction(int geneIndex, double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);

            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-maxCoordinate, maxCoordinate, i)
                {GetGeneValueFunction = GetGeneValueFunction};


          



            var functionResults = new List<(string functionName, IList<IList<MeanEvolutionResult>>)>();
            //var sw = Stopwatch.StartNew();
            foreach (var functionToSolve in functions)
            {
                IFitness fitness = new FunctionFitness<double>(functionToSolve.function);


                var sizeResults = new List<IList<MeanEvolutionResult>>();
                foreach (var size in sizes)
                {

                    var testResults = new List<MeanEvolutionResult>();

                    foreach (var (kind, duration, nbGenerations, noMutation, populationSize, reinsertion,
                        forceReinsertion) in testParams)
                    {

                        TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(duration);
                        maxNbGenerations = nbGenerations;
                        var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);

                        var noEmbeddingConverter = new GeometricConverter<double>
                        {
                            IsOrdered = false,
                            DoubleToGeneConverter = GetGeneValueFunction,
                            GeneToDoubleConverter = (genIndex, geneValue) => geneValue
                        };
                        var typedNoEmbeddingConverter = new TypedGeometricConverter();
                        typedNoEmbeddingConverter.SetTypedConverter(noEmbeddingConverter);

                        IMetaHeuristic metaHeuristic = MetaHeuristicsService.CreateMetaHeuristicByName(kind.ToString(),
                            nbGenerations, populationSize, typedNoEmbeddingConverter, noMutation);

                        //Enforcing reinsertion
                        if (forceReinsertion && metaHeuristic is IContainerMetaHeuristic containerMetaHeuristic)
                        {
                            var subMH = containerMetaHeuristic.SubMetaHeuristic;
                            if (subMH is ReinsertionHeuristic rh)
                            {
                                rh.StaticOperator = reinsertion;
                            }
                            else
                            {
                                containerMetaHeuristic.SubMetaHeuristic = new ReinsertionHeuristic()
                                    {StaticOperator = reinsertion, SubMetaHeuristic = subMH};
                            }
                        }

                        var meanResult = new MeanEvolutionResult
                        {
                            TestSettings = (kind, duration, nbGenerations, noMutation, populationSize, reinsertion,
                                forceReinsertion),
                            SkipExtremaPercentage = 0.2
                        };
                        for (int i = 0; i < repeatNb; i++)
                        {
                            var target = InitGa(metaHeuristic, fitness, AdamChromosome(size), crossover, populationSize,
                                termination, reinsertion, EnableOperatorsParallelism);
                            target.Start();
                            var result = target.GetResult();
                            meanResult.Results.Add(result);
                        }


                        testResults.Add(meanResult);


                    }

                    sizeResults.Add(testResults);
                }

                functionResults.Add((functionToSolve.fName, sizeResults));

            }

            return functionResults;
        }

        protected virtual GeneticAlgorithm InitGa(IMetaHeuristic metaHeuristic, IFitness fitness, IChromosome adamChromosome, ICrossover crossover, int populationSize, ITermination termination, IReinsertion reinsertion, bool enableParallelism)
        {
            var selection = new EliteSelection();
            var mutation = new UniformMutation();

            var generationStrategy = new TrackingGenerationStrategy();

            var initialPopulation = new Population(populationSize, populationSize, adamChromosome) { GenerationStrategy = generationStrategy };
            GeneticAlgorithm target;
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
            if (enableParallelism)
            {
                target.OperatorsStrategy = new TplOperatorsStrategy();
            }

            if (EnableEvaluatorParallelism)
            {
                target.TaskExecutor = new TplTaskExecutor();
            }
            else
            {
                target.TaskExecutor = new LinearTaskExecutor();
            }


            return target;
        }



        
    }
}