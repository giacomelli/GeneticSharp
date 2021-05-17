using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Crossovers.Geometric;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Compound;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Results;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Extensions.Mathematic.Functions;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class CompoundMetaHeuristicsTest : MetaHeuristicTestBase
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }




        /// <summary>
        /// Default Whale Optimization Algorithm Compound MetaHeuristic exhibits optimization on stub chromosome
        /// </summary>
        [Test]
        public void Evolve_WOA_Stub_Small_Optmization()
        {
            IMetaHeuristic MetaHeuristic(int maxValue) => GetDefaultWhaleHeuristicForChromosomStub(true, 300, maxValue);
            IChromosome AdamChromosome(int maxValue) => new ChromosomeStub(maxValue, maxValue);
            IFitness Fitness(int maxValue) => new FitnessStub(maxValue) { SupportsParallel = false };

            var reinsertion = new FitnessBasedElitistReinsertion();

            var compoundResults = EvolveMetaHeuristicDifferentSizes(1,
                Fitness,
                AdamChromosome,
                SmallSizes,
                MetaHeuristic,
                i => 0.8,
                reinsertion);

            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness, false);
            }
        }

        /// <summary>
        /// Whale Optimization Algorithm Compound MetaHeuristic with params variant exhibits optimization on stub chromosome
        /// </summary>
        [Test]
        public void Evolve_WOAParams_Stub_Small_Optmization()
        {
            IMetaHeuristic MetaHeuristic(int maxValue) => GetDefaultWhaleHeuristicForChromosomStub(false, 300, maxValue);
            IChromosome AdamChromosome(int maxValue) => new ChromosomeStub(maxValue, maxValue);
            IFitness Fitness(int maxValue) => new FitnessStub(maxValue) { SupportsParallel = false };

            var reinsertion = new FitnessBasedElitistReinsertion();

            var compoundResults = EvolveMetaHeuristicDifferentSizes(1,
                Fitness,
                AdamChromosome,
                SmallSizes,
                MetaHeuristic,
                i => 0.8,
                reinsertion);


            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness, true);
            }

        }

        /// <summary>
        /// Whale Optimization Algorithm Compound MetaHeuristic optimizes better than regular GA with Uniform crossover on Stub Chromosome on large problem sizes
        /// </summary>
        [Test]
        public void Compare_WOA_Uniform_Stub_Large_LargerFitness()
        {
            var targetRatio = 1;
            var crossover = new UniformCrossover();
            var results = Compare_WOAReduced_Crossover_ChromosomeStub(1, crossover, LargeSizes);

            var meanRatio = results.Sum(c => c.result2.Fitness / c.result1.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, targetRatio);

        }

        /// <summary>
        /// Whale Optimization Algorithm Compound MetaHeuristic optimizes better than regular GA with One Point crossover on Stub Chromosome on small problem sizes
        /// </summary>
        [Test]
        public void Compare_WOA_OnePoint_Stub_Small_LargerFitness()
        {
            var targetRatio = 1;
            var crossover = new OnePointCrossover(2);
            var results = Compare_WOAReduced_Crossover_ChromosomeStub(1, crossover, SmallSizes);

            var meanRatio = results.Sum(c => c.result2.Fitness / c.result1.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, targetRatio);

        }

      


        /// <summary>
        /// Whale Optimization Algorithm Compound MetaHeuristic exhibits optimization on Known parametric functions with small problem sizes
        /// </summary>
        [Test]
        public void Evolve_WOA_KnownFunctions_Small_Optmization()
        {

            var functionHalfRange = 5;

            double GetGeneValueFunction(int geneIndex, double metricValue) => metricValue % functionHalfRange;
            var converter = new GeometricConverter<double>
            {
                DoubleToGeneConverter = GetGeneValueFunction,
                GeneToDoubleConverter = (genIndex, geneValue) => geneValue
            };
            var typedConverter = new TypedGeometricConverter();
            typedConverter.SetTypedConverter(converter);

            IMetaHeuristic MetaHeuristic(int maxValue)
            {
                var woa = new WhaleOptimisationAlgorithm()
                {
                    MaxGenerations = 300,
                    GeometricConverter = typedConverter,
                };
                return woa.Build();
            }

            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-functionHalfRange, functionHalfRange, i) { GetGeneValueFunction = GetGeneValueFunction };

            var reinsertion = new FitnessBasedElitistReinsertion();

            Dictionary<Func<Gene[], double>, Func<int, double>> functionsToSolveWithTargets =
                new Dictionary<Func<Gene[], double>, Func<int, double>>
                {
                    {
                        genes => KnownFunctions.GetKnownFunctions()[nameof(KnownFunctions.Rastrigin)].Function(genes.Select(g => g.Value.To<double>()).ToArray()),
                        i => 10 * i
                    },
                    {
                        genes =>
                        {
                            var knownAckley = KnownFunctions.GetKnownFunctions()[nameof(KnownFunctions.Ackley)];
                            var geneValues = genes.Select(g => g.Value.To<double>()).ToArray();
                            return knownAckley.Fitness(geneValues, knownAckley.Function(geneValues));
                        },
                        i => -0.1
                    }
                };

            foreach (var functionToSolve in functionsToSolveWithTargets)
            {
                IFitness Fitness(int i) => new FunctionFitness<double>(functionToSolve.Key);
                var compoundResults = EvolveMetaHeuristicDifferentSizes(1,
                    Fitness,
                    AdamChromosome,
                    SmallSizes,
                    MetaHeuristic,
                    functionToSolve.Value,
                    reinsertion);

                for (int i = 0; i < compoundResults.Count; i++)
                {
                    AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness, false);
                }

            }

        }


        /// <summary>
        /// Whale Optimization Algorithm Compound MetaHeuristic optimizes better than regular GA with OnePointCrossover on several Known parametric functions on small problem sizes
        /// </summary>
        [Test]
        public void Compare_WOA_OnePoint_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new OnePointCrossover(2);

            var resultsRatio = new[] { 2.5, 1.5, 1E4, 1E5, 500 };
            int maxNbGenerations = 100;

            Compare_WOA_Crossover_KnownFunctions_Size_LargerFitness_Bounded(crossover, SmallSizes, maxNbGenerations, resultsRatio);

        }


        /// <summary>
        /// Whale Optimization Algorithm Compound MetaHeuristic optimizes better  than regular GA with Uniform crossover on several Known parametric functions on large problem sizes
        /// </summary>
        [Test]
        public void Compare_WOA_Uniform_KnownFunctions_Large_LargerFitness_Bounded()
        {
            var crossover = new UniformCrossover();

            var resultsRatio = new[] { 1.5, 100, 1E10, 1E6, 1E5 };
            int maxNbGenerations = 100;

            Compare_WOA_Crossover_KnownFunctions_Size_LargerFitness_Bounded(crossover, LargeSizes, maxNbGenerations, resultsRatio);


        }


      


        /// <summary>
        /// The version with parameter calls should start faster with less preprocessing but then pay an additional cost for parameter lookup
        /// </summary>
        [Test]
        public void Compare_WOAReduced_WOAParamCalls_Stub_Small_Bounds()
        {
            var repeatNb = 5;
            IMetaHeuristic WoaWithParamCalls(int maxValue) => GetDefaultWhaleHeuristicForChromosomStub(false, 300, maxValue);
            IMetaHeuristic WoaWithReducedArgs(int maxValue) => GetDefaultWhaleHeuristicForChromosomStub(true, 300, maxValue);
            var crossover = new UniformCrossover();

            var problemSizes = SmallSizes.Skip(1).Take(1);

            //Population Size
            var populationSize = 200;

            //Termination
            var minFitness = double.MaxValue;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = int.MaxValue;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2.0);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            var reinsertion = new FitnessBasedElitistReinsertion();

            //var meanResultsBySize = new List<(MeanEvolutionResult woaWithParams, MeanEvolutionResult woaReduced)>();

            int ResultComparer(IEvolutionResult result1, IEvolutionResult result2)
            {
                var toReturn = Math.Sign(result1.Population.GenerationsNumber - result2.Population.GenerationsNumber);
                if (toReturn != 0)
                {
                    return toReturn;
                }

                return result1.Fitness > result2.Fitness ? 1 : -1;
            }
            var results = CompareMetaHeuristicsDifferentSizes(repeatNb,
                problemSizes,
                maxValue => new FitnessStub(maxValue) { SupportsParallel = false },
                maxValue => new ChromosomeStub(maxValue, maxValue),
                false,
                WoaWithParamCalls,
                WoaWithReducedArgs,
                crossover,
                populationSize, termination, reinsertion);

            //todo: figure out why such a big swing depending on the Framework version (.net code as reduced version faster, unlike .Net 4.6.2)

#if NETCOREAPP
            double lowerBoundGenRatio = 0.9;
            double upperBoundGenRatio = 1.5;
#else
            double lowerBoundGenRatio = 0.6;
            double upperBoundGenRatio = 1.5;
#endif



            foreach (var (woaWithParams, woaReduced) in results)
            {
               

                Assert.Greater(woaReduced.GenerationsNumber / (double)woaWithParams.GenerationsNumber, lowerBoundGenRatio);
                Assert.Less(woaReduced.GenerationsNumber / (double)woaWithParams.GenerationsNumber, upperBoundGenRatio);
            }




        }

        /// <summary>
        /// Whale Optimization Algorithm performs similarly in lower dimension and better in higher dimensions than WOA with BubblenetOperator replaced by a simpler centroid oprator 
        /// </summary>
        [Test]
        public void Compare_WOA_WOANaive_Ackley_Bounds()
        {
            var repeatNb = 5;
            var testParams = new List<(KnownCompoundMetaheuristics kind, double seconds, int nbGenerationsWOA, bool noMutation)>
            {

                (KnownCompoundMetaheuristics.WhaleOptimisation,  1.0,  100,  true),
                (KnownCompoundMetaheuristics.WhaleOptimisationNaive,  1.0, 100,  true),

            };

            var sizes = new[] { 50, 200 }.ToList();

            // population parameters
            int populationSize = 100;

            //Termination
            var minFitness = double.MaxValue;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = int.MaxValue;

            //var reinsertion = new FitnessBasedElitistReinsertion();
            var reinsertion = new PureReinsertion();

            var crossover = new UniformCrossover();


            var maxCoordinate = 10;

            double GetGeneValueFunction(int geneIndex, double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);
            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-maxCoordinate, maxCoordinate, i) { GetGeneValueFunction = GetGeneValueFunction };

            // Ackley function
            var ackleyFunctionWithFitness = GetKnownFunctions(true).First();



            IFitness Fitness(int i) => new FunctionFitness<double>(ackleyFunctionWithFitness.function);


            var sizeResults = new List<List<MeanEvolutionResult>>();
            foreach (var size in sizes)
            {

                var testResults = new List<MeanEvolutionResult>();


                foreach (var (kind, duration, nbGenerationsWoa, noMutation) in testParams)
                {

                    TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(duration);
                    var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
                    IMetaHeuristic metaHeuristic;
                    var noEmbeddingConverter = new GeometricConverter<double>
                    {
                        IsOrdered = false,
                        DoubleToGeneConverter = GetGeneValueFunction,
                        GeneToDoubleConverter = (genIndex, geneValue) => geneValue
                    };

                    switch (kind)
                    {
                        case KnownCompoundMetaheuristics.Default:
                            metaHeuristic = new DefaultMetaHeuristic();
                            break;
                        case KnownCompoundMetaheuristics.WhaleOptimisation:
                        case KnownCompoundMetaheuristics.WhaleOptimisationNaive:
                            var woa = new WhaleOptimisationAlgorithm()
                            {
                                MaxGenerations = nbGenerationsWoa,
                                NoMutation = noMutation
                            };
                            woa.SetGeometricConverter(noEmbeddingConverter);
                            if (kind == KnownCompoundMetaheuristics.WhaleOptimisationNaive)
                            {
                                woa.BubbleOperator = WhaleOptimisationAlgorithm.GetSimpleBubbleNetOperator();
                            }
                            metaHeuristic = woa.Build();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var meanResult = new MeanEvolutionResult { SkipExtremaPercentage = 0.2 };
                    for (int i = 0; i < repeatNb; i++)
                    {
                        var target = InitMetaGeneticAlgorithm(metaHeuristic, Fitness(size), AdamChromosome(size), false, crossover, populationSize, termination, reinsertion, true);
                        target.Start();
                        var result = target.GetResult();
                        meanResult.Results.Add(result);
                    }
                    testResults.Add(meanResult);
                }
                sizeResults.Add(testResults);
            }




            var smallSizeResult = sizeResults[0];
            var largeSizeResult = sizeResults[1];

            //WOA and WOA naive similar in low dimensions given the termination duration
            Assert.Less(Math.Abs(smallSizeResult[0].Fitness - smallSizeResult[1].Fitness), 0.05);
            //WOA better than WOA "Naïve" on large dimensions
            Assert.Greater(largeSizeResult[0].Fitness, largeSizeResult[1].Fitness);
            
           

        }


        /// <summary>
        /// Population Adamchromosome is responsible for creating the initial generation. Eve chromosomes are fixed individuals that can be added to the first generation. This is illustrated here by reinjecting an evolution's best result using standard GA into the same settings for a second run resulting in improved fitness mechanically with FitnesElitist reinsertion. The 2nd evolution run isn't necessarily more successful though with frequent collapse to the Eve chromosome.   
        /// </summary>
        [Test]
        public void Compare_Default_DefaultWithEveBootstrap_Ackley_BetterFitness()
        {
            var repeatNb = 1;
            

            //Problem Sizes
            var sizes = SmallSizes;//.Skip(2).Take(1);

            var resultsRatio = new[] { 1, 1, 1, 1, 1};


            //Heuristics


            var defaultHeuristic = new DefaultMetaHeuristic();


            //Population Size
            var populationSize = 100;


            int maxNbGenerations = 100;


            //Functions

            var maxCoordinate = 5;
            double GetGeneValueFunction(int geneIndex, double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);

            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-maxCoordinate, maxCoordinate, i);

            var knownFunctions = GetKnownFunctions(false);
            var targetFunction = knownFunctions;//.Where(f => f.fName == nameof(KnownFunctions.Rastrigin));


            //Reinsertion
            var reinsertion = new FitnessBasedElitistReinsertion();


            //Crossover
            var crossover = new UniformCrossover();


            //Termination
            var minFitness = double.MaxValue;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(5);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);


           


            var resultsByFunctionBySize = new List<IList<(IEvolutionResult resultWithoutEve, IEvolutionResult resultWithEve)>>();

            foreach (var functionToSolve in targetFunction)
            {
                IFitness Fitness(int i) => new FunctionFitness<double>(functionToSolve.function);

                var results = CompareMetaHeuristicsDifferentSizes(repeatNb,
                    sizes,
                    Fitness,
                    AdamChromosome, 
                    true,
                    i=> defaultHeuristic,
                    i=> defaultHeuristic,
                    crossover, populationSize, termination, reinsertion);

                resultsByFunctionBySize.Add(results);

            }

            for (int functionIndex = 0; functionIndex < resultsByFunctionBySize.Count; functionIndex++)
            {
                var resultsBySize = resultsByFunctionBySize[functionIndex];
                foreach (var sizeResult in resultsBySize)
                {
                    AssertIsPerformingLessByRatio(EvolutionMeasure.Fitness, resultsRatio[functionIndex], sizeResult.resultWithEve, sizeResult.resultWithoutEve);
                }
            }

        }



        /// <summary>
        /// This is a custom unit test to do some preliminary experiences. Interesting results can be made into dedicated unit tests. The test attribute is commented out by default
        /// </summary>
        //[Test]
        public void GridSearch()
        {
            try
            {

                var repeatNb = 1;
                //repeatNb = 3;

                var sizes = SmallSizes;

                var knownFunctions = GetKnownFunctions(false);
                var allFunctions = new List<(string fName, Func<Gene[], double> function)>(knownFunctions);
                allFunctions = knownFunctions.Take(1).ToList();


                var fitnessBasedElitist = new FitnessBasedElitistReinsertion();
                var pure = new PureReinsertion();
                var pairwise = new FitnessBasedPairwiseReinsertion();

                var defaultMaxTime = 300;
                var defaultNbGens = 200;
                var defaultPopSize = 100;

                var testParams = new (KnownCompoundMetaheuristics kind, double duration, int nbGenerations, bool noMutation, int populationSize, IReinsertion reinsertion, bool forceReinsertion)[]
            {
                (KnownCompoundMetaheuristics.Islands5DefaultNoMigration,  defaultMaxTime , defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),
                (KnownCompoundMetaheuristics.Islands5Default,  defaultMaxTime , defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),
                //(KnownCompoundMetaheuristics.Islands5BestMixture,  maxTime, defaultNbGens,  true, defaultPopSize, fitnessBasedElitist, false),
                //(KnownCompoundMetaheuristics.Islands5BestMixtureNoMigration,  maxTime, defaultNbGens,  true, defaultPopSize, fitnessBasedElitist, false),

                (KnownCompoundMetaheuristics.Default,  defaultMaxTime , defaultNbGens, false, defaultPopSize, fitnessBasedElitist, false),
                //(KnownCompoundMetaheuristics.Default,  maxTime, defaultNbGens, true, defaultPopSize, fitnessBasedElitist, false),

                //(KnownCompoundMetaheuristics.EquilibriumOptimizer,  maxTime, defaultNbGens,  false, defaultPopSize, fitnessBasedElitist, false),

               
                //(KnownCompoundMetaheuristics.ForensicBasedInvestigation,  maxTime, 2* defaultNbGens,  true, 50, pairwise, false),
                //(KnownCompoundMetaheuristics.ForensicBasedInvestigation,  maxTime, 2* defaultNbGens,  true, 250, pairwise, true),

                (KnownCompoundMetaheuristics.WhaleOptimisation,  defaultMaxTime , defaultNbGens,  true, 50, pure, false),
                (KnownCompoundMetaheuristics.WhaleOptimisation,  defaultMaxTime , defaultNbGens,  true, defaultPopSize, pure, false),

            };

                var sw = Stopwatch.StartNew();
                var functionResults = EvolveMetaHeuristicsFunctionsTestParams(allFunctions, sizes, repeatNb, testParams);
                
                sw.Stop();
                var testDuration = sw.Elapsed;
                Assert.GreaterOrEqual(functionResults.Count, 0);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }



        }





        #region private methods



        private IMetaHeuristic GetDefaultWhaleHeuristicForChromosomStub(bool reduced, int maxOperations, int maxValue)
        {
            double FromGene(int geneIndex, int geneValue) => geneValue;
            var toGene = ChromosomeStub.GeneFromDouble(maxValue);
            //int ToGene(int i, double d) => toGene(d);

            IMetaHeuristic metaHeuristic;
            var noEmbeddingConverter = new GeometricConverter<int>
            {
                IsOrdered = false,
                DoubleToGeneConverter = toGene,
                GeneToDoubleConverter = FromGene
            };

            var woa = new WhaleOptimisationAlgorithm()
            {
                MaxGenerations = maxOperations,
            };
            woa.SetGeometricConverter(noEmbeddingConverter);

            if (!reduced)
            {
                metaHeuristic = woa.CreateWithParams();
            }
            else
            {
                metaHeuristic = woa.Build();
            }


            return metaHeuristic;

        }

        private IList<(IEvolutionResult result1, IEvolutionResult result2)> Compare_WOAReduced_Crossover_ChromosomeStub(int repeatNb, ICrossover crossover, IEnumerable<int> sizes)
        {
            IMetaHeuristic StandardHeuristic(int i) => new DefaultMetaHeuristic();
            IMetaHeuristic MetaHeuristic(int i) => GetDefaultWhaleHeuristicForChromosomStub(true, 50, i);

            IFitness Fitness(int i) => new FitnessStub(i) { SupportsParallel = false };
            IChromosome AdamChromosome(int i) => new ChromosomeStub(i, i);


            //Population Size
            var populationSize = 100;

            //Termination
            var minFitness = double.MaxValue;
            int maxNbGenerations = 50;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(5);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            var reinsertion = new FitnessBasedElitistReinsertion();

            var results = CompareMetaHeuristicsDifferentSizes(1,
                sizes,
                Fitness,
                AdamChromosome, false,
                StandardHeuristic,
                MetaHeuristic,
                crossover, populationSize, termination, reinsertion);

            return results;

        }


        private void Compare_WOA_Crossover_KnownFunctions_Size_LargerFitness_Bounded(ICrossover crossover, IEnumerable<int> sizes, int maxNbGenerations, double[] resultsRatio)
        {

            var maxCoordinate = 5;
            double GetGeneValueFunction(int geneIndex, double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);

            IMetaHeuristic StandardHeuristic(int i) => new DefaultMetaHeuristic();

            var noEmbeddingConverter = new GeometricConverter<double>
            {
                IsOrdered = false,
                DoubleToGeneConverter = GetGeneValueFunction,
                GeneToDoubleConverter = (genIndex, geneValue) => geneValue
            };

            IMetaHeuristic MetaHeuristic(int maxValue)
            {
                var woa = new WhaleOptimisationAlgorithm()
                {
                    MaxGenerations = maxNbGenerations,
                };
                woa.SetGeometricConverter(noEmbeddingConverter);
                return woa.Build();
            }

            //Termination
            var minFitness = double.MaxValue;

            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(5);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            var reinsertion = new FitnessBasedElitistReinsertion();

            var compoundResults = CompareMetaHeuristicsKnownFunctionsDifferentSizes(1, maxCoordinate, StandardHeuristic, MetaHeuristic, crossover, sizes, termination, reinsertion);

            var ratios = new List<(double meanRatio, double limitRatio)>();
            for (int i = 0; i < compoundResults.Count; i++)
            {
                var functionResults = compoundResults[i];
                var meanRatio = functionResults.Sum(c => c.result2.Fitness / c.result1.Fitness) / functionResults.Count;
                ratios.Add((meanRatio, resultsRatio[i]));

            }

            ratios.ForEach(ratio => Assert.GreaterOrEqual(ratio.meanRatio, ratio.limitRatio));

        }



        #endregion



    }
}
