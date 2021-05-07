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


        [Test]
        public void Compare_WOA_OnePoint_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new OnePointCrossover(2);

            var resultsRatio = new[] { 2.5, 1.5, 1E4, 1E5, 500 };
            int maxNbGenerations = 100;

            Compare_WOA_Crossover_KnownFunctions_Size_LargerFitness_Bounded(crossover, SmallSizes, maxNbGenerations, resultsRatio);

        }



        [Test]
        public void Compare_WOA_Uniform_KnownFunctions_Large_LargerFitness_Bounded()
        {
            var crossover = new UniformCrossover();

            var resultsRatio = new[] { 1.5, 100, 1E10, 1E5, 1E5 };
            int maxNbGenerations = 100;

            Compare_WOA_Crossover_KnownFunctions_Size_LargerFitness_Bounded(crossover, LargeSizes, maxNbGenerations, resultsRatio);


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
            var typedNoEmbeddingConverter = new TypedGeometricConverter();
            typedNoEmbeddingConverter.SetTypedConverter(noEmbeddingConverter);


            IMetaHeuristic MetaHeuristic(int maxValue)
            {
                var woa = new WhaleOptimisationAlgorithm()
                {
                    MaxGenerations = maxNbGenerations,
                    GeometricConverter = typedNoEmbeddingConverter,
                };
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

            ratios.ForEach(ratio=> Assert.GreaterOrEqual(ratio.meanRatio,ratio.limitRatio));

        }



        [Test]
        public void Compare_WOA_Uniform_Stub_Large_LargerFitness()
        {

            var crossover = new UniformCrossover();
            var results = Compare_WOAReduced_Crossover_ChromosomeStub(1, crossover, LargeSizes);

            var meanRatio = results.Sum(c => c.result2.Fitness / c.result1.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, 1);

        }

        [Test]
        public void Compare_WOA_OnePoint_Stub_Small_LargerFitness()
        {
            var crossover = new OnePointCrossover(2);
            var results = Compare_WOAReduced_Crossover_ChromosomeStub(1, crossover, SmallSizes);

            var meanRatio = results.Sum(c => c.result2.Fitness / c.result1.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, 1);

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
                AdamChromosome,
                StandardHeuristic,
                MetaHeuristic,
                crossover, populationSize, termination, reinsertion);

            return results;

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

            var problemSizes = SmallSizes;

            //Population Size
            var populationSize = 400;

            //Termination
            var minFitness = double.MaxValue;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = int.MaxValue;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2.0);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            var reinsertion = new FitnessBasedElitistReinsertion();

            var meanResultsBySize = new List<(MeanEvolutionResult woaWithParams, MeanEvolutionResult woaReduced)>();

            int ResultComparer(IEvolutionResult result1, IEvolutionResult result2)
            {
                var toReturn = Math.Sign(result1.Population.GenerationsNumber - result2.Population.GenerationsNumber);
                if (toReturn != 0)
                {
                    return toReturn;
                }

                return result1.Fitness > result2.Fitness ? 1 : -1;
            }


            for (int i = 0; i < repeatNb; i++)
            {
                var results = CompareMetaHeuristicsDifferentSizes(1,
                    problemSizes,
                    maxValue => new FitnessStub(maxValue) { SupportsParallel = false },
                    maxValue => new ChromosomeStub(maxValue, maxValue),
                    WoaWithParamCalls,
                    WoaWithReducedArgs,
                    crossover,
                    populationSize, termination, reinsertion);



                for (int sizeIndex = 0; sizeIndex < results.Count; sizeIndex++)
                {
                    if (meanResultsBySize.Count < sizeIndex + 1)
                    {
                        meanResultsBySize.Add((new MeanEvolutionResult { ResultComparer = ResultComparer, SkipExtremaPercentage = 0.2 }, new MeanEvolutionResult { ResultComparer = ResultComparer, SkipExtremaPercentage = 0.2 }));
                    }
                    meanResultsBySize[sizeIndex].woaWithParams.Results.Add(results[sizeIndex].result1);
                    meanResultsBySize[sizeIndex].woaReduced.Results.Add(results[sizeIndex].result2);
                }

            }

            foreach (var (woaWithParams, woaReduced) in meanResultsBySize)
            {
                //todo: figure out why such a big swing depending on the Framework version (.net code as reduced version faster, unlike .Net 4.6.2)
                Assert.Greater(woaReduced.GenerationsNumber / (double)woaWithParams.GenerationsNumber, 0.6);
                Assert.Less(woaReduced.GenerationsNumber / (double)woaWithParams.GenerationsNumber, 10);
            }




        }


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
            var ackleyFunctionWithFitness = GetKnownFunctions(true).Skip(1).First();



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
                    var typedNoEmbeddingConverter = new TypedGeometricConverter();
                    typedNoEmbeddingConverter.SetTypedConverter(noEmbeddingConverter);

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
                                GeometricConverter = typedNoEmbeddingConverter,
                                NoMutation = noMutation
                            };
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
                        var target = InitGa(metaHeuristic, Fitness(size), AdamChromosome(size), crossover, populationSize, termination, reinsertion, true);
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
        /// This is a custom unit test to do some preliminary experiences. Interesting results can be made into dedicated unit tests
        /// </summary>
        [Test]
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


        //private static TGeneValue NaiveBubbleNetOperator<TGeneValue>(IList<TGeneValue> geneValues, Func<TGeneValue, double> geneToDoubleConverter, Func<double, TGeneValue> doubleToGeneConverter, double l, double b)
        //{
        //    var metricValues = geneValues.Select(geneToDoubleConverter).ToList();
        //    var geometricValue = (metricValues[1] + metricValues[0]) / 2;
        //    var toReturn = doubleToGeneConverter(geometricValue);
        //    return toReturn;
        //}





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
            var typedNoEmbeddingConverter = new TypedGeometricConverter();
            typedNoEmbeddingConverter.SetTypedConverter(noEmbeddingConverter);


            var woa = new WhaleOptimisationAlgorithm()
            {
                MaxGenerations = maxOperations,
                GeometricConverter = typedNoEmbeddingConverter,
            };

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



        #endregion



    }
}
