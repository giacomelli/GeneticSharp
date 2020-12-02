using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture()]
    [Category("MetaHeuristics")]
    class CompoundMetaHeuristicsTest: MetaHeuristicTestBase
    {
        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        private const double DefaultHelicoidScale = 0.2;
        

        [Test()]
        public void Evolve_WOAParams_Stub_Small_Optmization()
        {
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => GetDefaultWhaleHeuristicForChromosomStub(false, 300, maxValue);
            Func<int, IChromosome> adamChromosome = maxValue => new ChromosomeStub(maxValue, maxValue);
            Func<int, IFitness> fitness = maxValue => new FitnessStub(maxValue) { SupportsParallel = false };


          var compoundResults=  EvolveMetaHeuristicDifferentSizes(
                fitness,
                adamChromosome,
                SmallSizes,
                metaHeuristic, 
                i => 0.6);


            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness);
            }

        }

        [Test()]
        public void Evolve_WOA_Stub_Small_Optmization()
        {
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => GetDefaultWhaleHeuristicForChromosomStub(true, 300, maxValue);
            Func<int, IChromosome> adamChromosome = maxValue => new ChromosomeStub(maxValue, maxValue);
            Func<int, IFitness> fitness = maxValue => new FitnessStub(maxValue) { SupportsParallel = false };

            var compoundResults =  EvolveMetaHeuristicDifferentSizes(
                fitness,
                adamChromosome,
                SmallSizes, 
                metaHeuristic, 
                i => 0.6);

            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness);
            }
        }

        [Test()]
        public void Evolve_WOA_KnownFunctions_Small_Optmization()
        {

            var helicoidScale = DefaultHelicoidScale;
            var functionHalfRange = 5;

            Func<double, double> getGeneValueFunction =  d => d % functionHalfRange;
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => MetaHeuristicsFactory.WhaleOptimisationAlgorithm<double>(false, 300, helicoidScale,
                geneValue => geneValue, getGeneValueFunction);
            Func<int, IChromosome> adamChromosome = i => new EquationChromosome<double>(-functionHalfRange, functionHalfRange, i)
            {
                GetGeneValueFunction = getGeneValueFunction
            };
            
            Dictionary<Func<Gene[], double>, Func<int, double>> functionsToSolveWithTargets = new Dictionary<Func<Gene[], double>, Func<int, double>>();
            functionsToSolveWithTargets.Add(genes => KnownFunctionsFactory.Rastrigin(genes.Select(g => g.Value.To<double>()).ToArray()), i => 10 * i);
            functionsToSolveWithTargets.Add(genes => 1 / (1 - KnownFunctionsFactory.ReverseAckley(genes.Select(g => g.Value.To<double>()).ToArray())), i => 0.1);

            foreach (var functionToSolve in functionsToSolveWithTargets)
            {
                Func<int, IFitness> fitness = i => new FunctionFitness<double>(functionToSolve.Key);
                var compoundResults = EvolveMetaHeuristicDifferentSizes(
                    fitness,
                    adamChromosome,
                    SmallSizes, 
                    metaHeuristic, 
                    functionToSolve.Value);

                for (int i = 0; i < compoundResults.Count; i++)
                {
                    AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness);
                }

            }
            
        }


        [Test()]
        public void Compare_WOA_OnePoint_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new OnePointCrossover(2);

            var resultsRatio = new[] {1.3, 5, 50, 1.5};

            var helicoidScale = DefaultHelicoidScale;
            var maxCoordinate = 5;
            Func<double, double> getGeneValueFunction = d => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);

            Func<int, IMetaHeuristic> standardHeuristic = i => new DefaultMetaHeuristic();
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => MetaHeuristicsFactory.WhaleOptimisationAlgorithm<double>(false, 300, helicoidScale,
                geneValue => geneValue, getGeneValueFunction);

            //Termination
            var minFitness = 1;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);


            var compoundResults = CompareMetaHeuristicsKnownFunctionsDifferentSizes(maxCoordinate, standardHeuristic, metaHeuristic, crossover, SmallSizes, termination);

           for (int i = 0; i < compoundResults.Count; i++)
           {
               var functionResults = compoundResults[i];
               var meanRatio = functionResults.Sum(c => c.result1.Fitness / c.result2.Fitness) / functionResults.Count;

               Assert.GreaterOrEqual(meanRatio, resultsRatio[i]);
            }

        }



        [Test()]
        public void Compare_WOA_Uniform_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new UniformCrossover();

            var resultsRatio = new[] { 1, 3.0, 30, 1.3 };

            var helicoidScale = DefaultHelicoidScale;
            var maxCoordinate = 5;
            Func<double, double> getGeneValueFunction = d => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);

            Func<int, IMetaHeuristic> standardHeuristic = i => new DefaultMetaHeuristic();
            Func<int, IMetaHeuristic> metaHeuristic = maxValue => MetaHeuristicsFactory.WhaleOptimisationAlgorithm<double>(false, 300, helicoidScale,
                geneValue => geneValue, getGeneValueFunction);

            //Termination
            var minFitness = 1;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);

            var compoundResults = CompareMetaHeuristicsKnownFunctionsDifferentSizes(maxCoordinate, standardHeuristic, metaHeuristic, crossover, SmallSizes, termination);

            for (int i = 0; i < compoundResults.Count; i++)
            {
                var functionResults = compoundResults[i];
                var meanRatio = functionResults.Sum(c => c.result1.Fitness / c.result2.Fitness) / functionResults.Count;

                Assert.GreaterOrEqual(meanRatio, resultsRatio[i]);
            }

        }


        [Test()]
        public void Compare_WOA_Uniform_Stub_Large_LargerFitness()
        {
           
            var crossover = new UniformCrossover();
            var results = Compare_WOAReduced_Crossover_ChromosomeStub_LargerFitness(crossover, LargeSizes);

            var meanRatio = results.Sum(c => c.result1.Fitness / c.result2.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, 1);

        }

        [Test()]
        public void Compare_WOA_OnePoint_Stub_Small_LargerFitness()
        {
            var crossover = new OnePointCrossover(2);
            var results = Compare_WOAReduced_Crossover_ChromosomeStub_LargerFitness(crossover, SmallSizes);

            var meanRatio = results.Sum(c => c.result1.Fitness / c.result2.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, 1);

        }


        [Test()]
        public void Compare_WOA_WOAParams_Stub_Large_MoreGenerations()
        {
            Func<int,IMetaHeuristic> originalMetaHeuristic = maxValue => GetDefaultWhaleHeuristicForChromosomStub(false, 300, maxValue);
            Func<int, IMetaHeuristic> reducedMetaHeuristic = maxValue => GetDefaultWhaleHeuristicForChromosomStub(true, 300, maxValue);
            var crossover = new UniformCrossover();


            //Population Size
            var populationSize = 100;

            //Termination
            var minFitness = 1.0;
            int maxNbGenerations = 2000;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            
            var results = CompareMetaHeuristicsDifferentSizes(
               LargeSizes, 
                maxValue =>new FitnessStub(maxValue) { SupportsParallel = false }, 
                maxValue => new ChromosomeStub(maxValue, maxValue) , 
                originalMetaHeuristic, 
                reducedMetaHeuristic, 
                crossover,
               populationSize, termination);


            var meanRatio = results.Sum(c => c.result1.Population.GenerationsNumber / c.result2.Population.GenerationsNumber) / results.Count;

            Assert.GreaterOrEqual(meanRatio, 1);

        }

        private enum  KnownMetaheuristics
        {
            Default,
            WhaleOptmizerAlgorithm
        }



        //[Test()]
        public void GridSearch_WOA()
        {
            var repeatNb = 3;
            var testParams = new List<(KnownMetaheuristics kind,  double seconds, double helicoidScale, int nbGenerationsWOA, bool noMutation)>
            {
                //(KnownMetaheuristics.Default,  1, 1, 200, 1.2),
                //(KnownMetaheuristics.WhaleOptmizerAlgorithm,  1, 100, 200, 1.2),
                (KnownMetaheuristics.WhaleOptmizerAlgorithm,  1.0, 50.0, 200, false),
                (KnownMetaheuristics.WhaleOptmizerAlgorithm,  1.0, 5.0, 200, true),
                (KnownMetaheuristics.WhaleOptmizerAlgorithm,  1.0, 2.0, 200,  true),
                (KnownMetaheuristics.WhaleOptmizerAlgorithm,  1.0, 0.5, 200,  true),
                //(KnownMetaheuristics.WhaleOptmizerAlgorithm,  1, 2, 200, 1.2),
            };

            var sizes = new[] {/*50,*/ 100/*, 200 */}.ToList();

            // population parameters
            int populationSize = 100;

            //Termination
            var minFitness = double.MaxValue;
            int maxNbGenerations = 100000;
            int stagnationNb = 100000;

            var crossover = new UniformCrossover();
            Func<int, IMetaHeuristic> standardHeuristic = size => new DefaultMetaHeuristic();

            var maxCoordinate = 5;

            Func<double, double> getGeneValueFunction = d => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);
            Func<int, IChromosome> adamChromosome = i => new EquationChromosome<double>(-maxCoordinate, maxCoordinate, i)
            {
                GetGeneValueFunction = getGeneValueFunction
            };

            var knownFunctions = GetKnownFunctions().Take(1);

            var functionResults = new List<List<List<MeanEvolutionResult>>>();
            foreach (var functionToSolve in knownFunctions)
            {
                Func<int, IFitness> fitness = i => new FunctionFitness<double>(functionToSolve);


                var sizeResults = new List<List<MeanEvolutionResult>>();
                foreach (var size in sizes)
                {

                    var testResults = new List<MeanEvolutionResult>();

                    
                    foreach (var testParam in testParams)
                    {

                        TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(testParam.seconds);
                        var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
                        IMetaHeuristic metaHeuristic;
                        if (testParam.kind == KnownMetaheuristics.WhaleOptmizerAlgorithm)
                        {
                           var woaMetaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm<double>(false, testParam.nbGenerationsWOA, testParam.helicoidScale, geneValue => geneValue, getGeneValueFunction, noMutation:testParam.noMutation);
                            metaHeuristic = woaMetaHeuristic;

                        }
                        else
                        {
                            metaHeuristic = new DefaultMetaHeuristic();
                        }

                        var meanResult = new MeanEvolutionResult();
                        for (int i = 0; i < repeatNb; i++)
                        {
                            var target = InitGa(metaHeuristic, fitness(size), adamChromosome(size), crossover, populationSize, termination);
                            target.Start();
                            var result = target.GetResult();
                            meanResult.Results.Add(result);
                        }
                        testResults.Add(meanResult);
                    }
                    sizeResults.Add(testResults);
                }
                functionResults.Add(sizeResults);
            }

            Assert.GreaterOrEqual(functionResults.Count, 0);



        }


        #region private methods





        private IList<(EvolutionResult result1, EvolutionResult result2)> Compare_WOAReduced_Crossover_ChromosomeStub_LargerFitness(ICrossover crossover, IEnumerable<int> sizes)
        {
            Func<int, IMetaHeuristic> standardHeuristic = i => new DefaultMetaHeuristic();
            Func<int, IMetaHeuristic> metaHeuristic = i => GetDefaultWhaleHeuristicForChromosomStub(true, 300, i);

            Func<int, IFitness> fitness = i => new FitnessStub(i) { SupportsParallel = false };
            Func<int, IChromosome> adamChromosome = i => new ChromosomeStub(i, i);


            //Population Size
            var populationSize = 100;

            //Termination
            var minFitness = 1;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);

            var results = CompareMetaHeuristicsDifferentSizes(
                sizes,
                fitness,
                adamChromosome,
                standardHeuristic,
                metaHeuristic,
                crossover, populationSize, termination);

            return results;

        }

       

        private IMetaHeuristic GetDefaultWhaleHeuristicForChromosomStub(bool reduced, int maxOperations, int maxValue)
        {

            Func<int, Double> fromGene = geneValue => (double) geneValue;
            Func<Double, int> ToGene =  ChromosomeStub.GeneFromDouble(maxValue);

            if (!reduced)
            {
                return MetaHeuristicsFactory.WhaleOptimisationAlgorithmWithParams<int>(false, maxOperations, fromGene, ToGene);
            }
            else
            {
                return MetaHeuristicsFactory.WhaleOptimisationAlgorithm<int>(false, maxOperations, DefaultHelicoidScale, fromGene, ToGene);
            }

        }



        #endregion



    }
}
