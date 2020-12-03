using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Tsp;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
    [TestFixture()]
    [Category("Extensions")]
    public class TspGeometryEmbeddingTest
    {

        [Test()]
        public void GeometryEmbedding_DefaultGeometryEmbedding_DecentFitness()
        {


            // population parameters
            int numberOfCities = 100;

            // Fitness and chromosomes 
            var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            var adamChromosome = new TspChromosome(fitness.Cities.Count).Initialized();
            var startFitness = fitness.Evaluate(adamChromosome);

            //start fitness should be random path, which is typically better than worst cases
            Assert.GreaterOrEqual(startFitness, 0.1);

            //Empty OrderedEmbedding
            
            var tspGeometryEmbedding = new TspPermutationEmbedding(fitness);

            //Computing distance pairs
            var sw = Stopwatch.StartNew();
            var distancePairs = fitness.CityDistances;
            var timeToPairs = sw.Elapsed;

            Assert.LessOrEqual(timeToPairs, TimeSpan.FromMilliseconds(200));

            // Default Embedding
            sw.Restart();
            tspGeometryEmbedding.RegisterDefaultEmbedding();
            var timeToChromosome = sw.Elapsed;

            Assert.LessOrEqual(timeToChromosome, TimeSpan.FromMilliseconds(200));

            var metricChromosome = tspGeometryEmbedding.GetMetricChromosome();
            var fitnessMetric = fitness.Evaluate(metricChromosome);

            Assert.GreaterOrEqual(fitnessMetric, 0.6);

            //Testing longer default scans

            tspGeometryEmbedding = new TspPermutationEmbedding(fitness) { DefaultNbScans = 5 };
            sw.Restart();
            tspGeometryEmbedding.RegisterDefaultEmbedding();
            var longtimeToChromosome = sw.Elapsed;
            
            Assert.LessOrEqual(longtimeToChromosome, TimeSpan.FromSeconds(1));

            metricChromosome = tspGeometryEmbedding.GetMetricChromosome();
            var longInitFitnessMetric = fitness.Evaluate(metricChromosome);

            Assert.GreaterOrEqual(longInitFitnessMetric, 0.6);


            //Testing agressive skips 
            numberOfCities = 500;
            fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
            adamChromosome = new TspChromosome(fitness.Cities.Count).Initialized();
            startFitness = fitness.Evaluate(adamChromosome);

            //start fitness should be random path, which is typically better than worst cases
            Assert.GreaterOrEqual(startFitness, 0.1);

            tspGeometryEmbedding = new TspPermutationEmbedding(fitness) { DefaultNbScans = 1 };
            tspGeometryEmbedding.SwapSkipsPicker = (rank, repeatIndex) => 4 + (fitness.Cities.Count - 1 - rank) / 5;
            sw.Restart();
            tspGeometryEmbedding.RegisterDefaultEmbedding();
            var skipsToChromosome = sw.Elapsed;
            
            Assert.LessOrEqual(skipsToChromosome, TimeSpan.FromSeconds(20));

            metricChromosome = tspGeometryEmbedding.GetMetricChromosome();
            var skipInitFitnessMetric = fitness.Evaluate(metricChromosome);

            Assert.GreaterOrEqual(skipInitFitnessMetric, 0.5);

        }

       
        [Test()]
        public void Compare_WOA_GeometryEmbedding_OrderedTwors_ManyGenerations_Faster()
        {
            var testParams = new List<(int nbCities, double threshold, double ratio)>
            {
                (100, 0.85, 0.9), (200, 0.75, 0.8), (400, 0.75, 0.8)
            };

            foreach (var testParam in testParams)
            {
                int nbGenerationsWOA = 200;
                var termination = new FitnessThresholdTermination(testParam.threshold);
                Compare_WOA_GeometryEmbedding_ManyGenerations_Criterion(nbGenerationsWOA, testParam.nbCities, termination, testParam.ratio);
            }
        }


        [Test()]
        public void Compare_WOA_GeometryEmbedding_ManyGenerations_SmallerDistance()
        {
            int numberOfCities = 100;
            var nbGenerations = 1000;
            var termination = new GenerationNumberTermination(nbGenerations);
            Compare_WOA_GeometryEmbedding_ManyGenerations_Criterion(nbGenerations, numberOfCities, termination, 1);
        }

       [Test()]
        public void Compare_WOA_GeometryEmbedding_TimeConstraint_SmallerDistance()
        {
            int nbGenerationsWOA = 200;
            int numberOfCities = 200;
            var termination = new TimeEvolvingTermination(TimeSpan.FromSeconds(10));
            Compare_WOA_GeometryEmbedding_ManyGenerations_Criterion(nbGenerationsWOA, numberOfCities, termination, 0.9);
        }

        [Test()]
        public void Compare_WOA_GeometryEmbedding_VeryLargeProblem_TimeConstraint_SmallerDistance()
        {
            int nbGenerationsWOA = 100;
            int numberOfCities = 1000;
            var termination = new TimeEvolvingTermination(TimeSpan.FromSeconds(10));
            Compare_WOA_GeometryEmbedding_ManyGenerations_Criterion(nbGenerationsWOA, numberOfCities, termination, 0.8);
        }



        //[Test()]
        public void GridSearch_WOA()
        {
            var repeatNb = 3;
            var testParams = new List<(int nbCities, double seconds, double helicoidScale, int nbGenerationsWOA, double ratio)>
            {
                (80, 1, 1, 200, 1.2),
                (80, 1, 0, 200, 1.2),
                (80, 1,-1, 200, 1.2),
            };

            var meansByParam = new List<(double meanNative, double meanWoa, double meanWoaGeom, double meanWoaSwap)>();

            var resultDEtail = new List<List<(TspEvolutionResult native, TspEvolutionResult woa, TspEvolutionResult woaGeom, TspEvolutionResult
                woaSwap)>>();
            foreach (var testParam in testParams)
            {

                var results = new List<(TspEvolutionResult native, TspEvolutionResult woa, TspEvolutionResult woaGeom, TspEvolutionResult
                    woaSwap)>();
                var termination = new TimeEvolvingTermination(TimeSpan.FromSeconds(testParam.seconds));
                // population parameters
                int populationSize = 100;

                for (int i = 0; i < repeatNb; i++)
                {
                    // Fitness and chromosomes 
                    var fitness = new TspFitness(testParam.nbCities, 0, 1000, 0, 1000);


                    // Native evolution
                    results.Add(EvolveAlgorithms_Termination(fitness, testParam.helicoidScale, termination, populationSize, testParam.nbGenerationsWOA));

                }
                resultDEtail.Add(results);

               (double meanNative, double meanWoa, double meanWoaGeom, double meanWoaSwap) resultMean = (0,0,0,0);
               resultMean.meanNative = results.Sum(tuple => tuple.native.Fitness.Value)/ results.Count;
               resultMean.meanWoa = results.Sum(tuple => tuple.woa.Fitness.Value)/results.Count;
               resultMean.meanWoaGeom = results.Sum(tuple => tuple.woaGeom.Fitness.Value/ results.Count);
                resultMean.meanWoaSwap = results.Sum(tuple => tuple.woaSwap.Fitness.Value/ results.Count);

                meansByParam.Add(resultMean);

            }

            Debugger.Break();

        }


      
        
        public (TspEvolutionResult native, TspEvolutionResult woa, TspEvolutionResult woaGeom, TspEvolutionResult
            woaSwap) EvolveAlgorithms_Termination(TspFitness fitness, double helicoidScale, ITermination termination,
                int populationSize, int nbGenerationsWOA)
        {
            try
            {
                (TspEvolutionResult native, TspEvolutionResult woa, TspEvolutionResult woaGeom, TspEvolutionResult
                    woaSwap) toReturn = (null, null, null, null);
            
                var adamChromosome = new TspChromosome(fitness.Cities.Count).Initialized();

                // Native operators
                var crossover = new OrderedCrossover();
                var mutation = new TworsMutation();
                // Native evolution
                IMetaHeuristic metaHeuristic = new DefaultMetaHeuristic();
                toReturn.native = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic, crossover, mutation, termination);
                
                // WOA parameters

                Func<double, int> getGeneValueFunction = d => Math.Round(d).PositiveMod(fitness.Cities.Count);

                //Simple WOA
                var defaultEmbedding = new OrderedEmbedding<int>(){GeneSelectionMode = GeneSelectionMode.RandomOrder | GeneSelectionMode.SingleFirstAllowed};
                
                metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(true, nbGenerationsWOA, helicoidScale, geneValue => geneValue, getGeneValueFunction);
                var resultWOA = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic, crossover, mutation, termination);
                toReturn.woa = resultWOA;


                //Default OrderedEmbedding with default embedding
                //var tspGeometryEmbedding = new TspPermutationEmbedding(fitness) { GeneSelectionMode = GeneSelectionMode.All }.WithDefaultEmbedding();
                //var tspFitness = fitness.Evaluate(tspGeometryEmbedding.GetMetricChromosome());

                var tspGeometryEmbedding = new TspPermutationEmbedding(fitness) { GeneSelectionMode = GeneSelectionMode.RandomOrder };

                if (toReturn.woa.Fitness>toReturn.native.Fitness)
                {
                    tspGeometryEmbedding.TargetPermutation =
                        ((TspChromosome)toReturn.woa.Population.BestChromosome).GetCities();
                }
                else
                {
                    tspGeometryEmbedding.TargetPermutation =
                        ((TspChromosome)toReturn.native.Population.BestChromosome).GetCities();
                }
                
                //var newTspFitness = fitness.Evaluate(tspGeometryEmbedding.GetMetricChromosome());
                
                var updateEveryGenerationNb = 20;

                //WOA with Embedding  
                metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(true, nbGenerationsWOA, helicoidScale,
                    geneValue => geneValue,
                    getGeneValueFunction, tspGeometryEmbedding);

                //Embedding metric update routing: the best chromosome takes over target metric space
                var evolvedfitnessMetric = 0.0;
                var resultWOAGeom = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic,
                    crossover, mutation, termination,
                    algorithm =>
                    {
                        if (algorithm.GenerationsNumber % updateEveryGenerationNb == 0 && evolvedfitnessMetric < algorithm.BestChromosome.Fitness)
                        {
                            evolvedfitnessMetric = algorithm.BestChromosome.Fitness.Value;
                            tspGeometryEmbedding.TargetPermutation = ((TspChromosome)algorithm.BestChromosome).GetCities();
                        }
                    });

                toReturn.woaGeom = resultWOAGeom;
                // WOA evolution with simple swap validator (no permutation embedding)
                var simpleGeometryEmbedding = new TspPermutationEmbedding(fitness){GeneSelectionMode = GeneSelectionMode.AllIndexed };
                
                //var simpleGeometryEmbedding = new OrderedEmbedding<int>();
                //simpleGeometryEmbedding.ValidateSwapFunction = tspGeometryEmbedding.ValidateSwapFunction;


                metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(true, nbGenerationsWOA, helicoidScale,
                    geneValue => geneValue,
                    getGeneValueFunction, simpleGeometryEmbedding);
                var resultWOAwithSwap = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic, crossover, mutation, termination);


                toReturn.woaSwap = resultWOAwithSwap;
                //AssertIsPerformingLessByRatio(termination, ratio, resultWOAwithSwap, resultWOAGeom);


                return toReturn;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Compare_WOA_GeometryEmbedding_ManyGenerations_Criterion(int nbGenerationsWOA, int numberOfCities, ITermination termination, double ratio)
        {
            try
            {

                // population parameters
                int populationSize = 100;

                // Fitness and chromosomes 
                var fitness = new TspFitness(numberOfCities, 0, 1000, 0, 1000);
                var adamChromosome = new TspChromosome(fitness.Cities.Count).Initialized();
                var startFitness = fitness.Evaluate(adamChromosome);

                //start fitness should be random path, which is typically better than worst cases
                Assert.GreaterOrEqual(startFitness, 0.2);


                // Native operators
                var crossover = new OrderedCrossover();
                var mutation = new TworsMutation();
                // Native evolution
                var resultOriginal = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, null, crossover, mutation, termination);
                Assert.GreaterOrEqual(resultOriginal.Fitness, 0.2);


                // WOA parameters
                Func<double, int> getGeneValueFunction = d => Math.Round(d).PositiveMod(numberOfCities);
                var helicoidScale = 2;

                //Default OrderedEmbedding with cold start
                var tspGeometryEmbedding = new TspPermutationEmbedding(fitness) { GeneSelectionMode = GeneSelectionMode.RandomOrder  };
                var updateEveryGenerationNb = 20;

                //WOA with Embedding  
                var metaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(true, nbGenerationsWOA, helicoidScale,
                    geneValue => geneValue,
                    getGeneValueFunction, tspGeometryEmbedding, true);

                //Embedding metric update routing: the best chromosome takes over target metric space
                var evolvedfitnessMetric = 0.0;
                var resultWOAGeom = Evolve_NbCities_Fast(fitness, adamChromosome, populationSize, metaHeuristic,
                    crossover, mutation, termination,
                    algorithm =>
                    {
                        if (algorithm.GenerationsNumber % updateEveryGenerationNb == 0 && evolvedfitnessMetric < algorithm.BestChromosome.Fitness)
                        {
                            evolvedfitnessMetric = algorithm.BestChromosome.Fitness.Value;
                            tspGeometryEmbedding.TargetPermutation = ((TspChromosome)algorithm.BestChromosome).GetCities();
                        }
                    });

                AssertIsPerformingLessByRatio(termination, ratio, resultOriginal, resultWOAGeom);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private void AssertIsPerformingLessByRatio(ITermination termination, double ratio, TspEvolutionResult result1, TspEvolutionResult result2)
        {
            switch (termination.GetType().Name)
            {
                case nameof(GenerationNumberTermination):
                case nameof(TimeEvolvingTermination):
                    Assert.GreaterOrEqual(result1.Distance * ratio, result2.Distance);
                    break;
                case nameof(FitnessThresholdTermination):
                    Assert.GreaterOrEqual(TimeSpan.FromTicks(Convert.ToInt64(result1.TimeEvolving.Ticks * ratio)), result2.TimeEvolving);
                    break;
                case nameof(FitnessStagnationTermination):
                    Assert.GreaterOrEqual(result1.Distance * ratio, result2.Distance);
                    Assert.GreaterOrEqual(TimeSpan.FromTicks(Convert.ToInt64(result1.TimeEvolving.Ticks * ratio)), result2.TimeEvolving);
                    break;
                    default:throw new InvalidOperationException("Termination not supported");
            }
        }

        private static TspEvolutionResult Evolve_NbCities_Fast(TspFitness fitness, TspChromosome adamChromosome, int populationSize, IMetaHeuristic metaHeuristic, ICrossover crossover, IMutation mutation, ITermination terminationCriterium, Action<IGeneticAlgorithm> generationUpdate = null)
        {
            var selection = new EliteSelection();
            var population = new Population(populationSize, populationSize, adamChromosome)
            {
                GenerationStrategy = new TrackingGenerationStrategy()
            };

            var ga = new MetaGeneticAlgorithm(population, fitness, selection, crossover, mutation);
            if (metaHeuristic != null)
            {
                ga.Metaheuristic = metaHeuristic;
            }
            if (generationUpdate != null)
            {
                ga.GenerationRan += (sender, args) => generationUpdate(ga);
            }

            var firstValue = fitness.Evaluate(adamChromosome);
            var firstDistance = adamChromosome.Distance;

            ga.Termination = terminationCriterium;
            ga.Start();
            var lastDistance = ((TspChromosome)ga.Population.BestChromosome).Distance;

            Assert.Less(lastDistance, firstDistance);

            return new TspEvolutionResult() { Population = ga.Population, TimeEvolving = ga.TimeEvolving, Distance = lastDistance };
        }

    }
}