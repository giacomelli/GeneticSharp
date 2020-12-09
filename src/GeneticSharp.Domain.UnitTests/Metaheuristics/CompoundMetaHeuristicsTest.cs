using System;
using System.Collections.Generic;
using System.Linq;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Infrastructure.Framework.Commons;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class CompoundMetaHeuristicsTest: MetaHeuristicTestBase
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
            IFitness Fitness(int maxValue) => new FitnessStub(maxValue) {SupportsParallel = false};

            var reinsertion = new FitnessBasedElitistReinsertion();

            var compoundResults=  EvolveMetaHeuristicDifferentSizes(
                Fitness,
                AdamChromosome,
                SmallSizes,
                MetaHeuristic, 
                i => 0.6,
                reinsertion);


            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness);
            }

        }

        [Test]
        public void Evolve_WOA_Stub_Small_Optmization()
        {
            IMetaHeuristic MetaHeuristic(int maxValue) => GetDefaultWhaleHeuristicForChromosomStub(true, 300, maxValue);
            IChromosome AdamChromosome(int maxValue) => new ChromosomeStub(maxValue, maxValue);
            IFitness Fitness(int maxValue) => new FitnessStub(maxValue) {SupportsParallel = false};

            var reinsertion = new FitnessBasedElitistReinsertion();

            var compoundResults =  EvolveMetaHeuristicDifferentSizes(
                Fitness,
                AdamChromosome,
                SmallSizes, 
                MetaHeuristic, 
                i => 0.6,
                reinsertion);

            for (int i = 0; i < compoundResults.Count; i++)
            {
                AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness);
            }
        }

        [Test]
        public void Evolve_WOA_KnownFunctions_Small_Optmization()
        {

            var functionHalfRange = 5;

            double GetGeneValueFunction(double d) => d % functionHalfRange;
            IMetaHeuristic MetaHeuristic(int maxValue) => MetaHeuristicsFactory.WhaleOptimisationAlgorithm(false, 300,  geneValue => geneValue, GetGeneValueFunction);
            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-functionHalfRange, functionHalfRange, i) {GetGeneValueFunction = GetGeneValueFunction};

            var reinsertion = new FitnessBasedElitistReinsertion();

            Dictionary<Func<Gene[], double>, Func<int, double>> functionsToSolveWithTargets =
                new Dictionary<Func<Gene[], double>, Func<int, double>>
                {
                    {
                        genes => KnownFunctionsFactory.Rastrigin(genes.Select(g => g.Value.To<double>()).ToArray()),
                        i => 10 * i
                    },
                    {
                        genes => 1 / (1 -
                                      KnownFunctionsFactory.ReverseAckley(genes.Select(g => g.Value.To<double>())
                                          .ToArray())
                            ),
                        i => 0.1
                    }
                };

            foreach (var functionToSolve in functionsToSolveWithTargets)
            {
                IFitness Fitness(int i) => new FunctionFitness<double>(functionToSolve.Key);
                var compoundResults = EvolveMetaHeuristicDifferentSizes(
                    Fitness,
                    AdamChromosome,
                    SmallSizes, 
                    MetaHeuristic, 
                    functionToSolve.Value,
                    reinsertion);

                for (int i = 0; i < compoundResults.Count; i++)
                {
                    AssertEvolution(compoundResults[i].result, compoundResults[i].minFitness);
                }

            }
            
        }


        [Test]
        public void Compare_WOA_OnePoint_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new OnePointCrossover(2);

            var resultsRatio = new[] {0.95, 5, 50, 1};

            Compare_WOA_Crossover_KnownFunctions_Small_LargerFitness_Bounded(crossover, resultsRatio);

        }



        [Test]
        public void Compare_WOA_Uniform_KnownFunctions_Small_LargerFitness_Bounded()
        {
            var crossover = new UniformCrossover();

            var resultsRatio = new[] { 0.95, 3.0, 30, 0.9 };

          Compare_WOA_Crossover_KnownFunctions_Small_LargerFitness_Bounded(crossover, resultsRatio);
            

        }


        private void Compare_WOA_Crossover_KnownFunctions_Small_LargerFitness_Bounded(ICrossover crossover,double[] resultsRatio)
        {

            var maxCoordinate = 5;
            double GetGeneValueFunction(double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);

            IMetaHeuristic StandardHeuristic(int i) => new DefaultMetaHeuristic();
            IMetaHeuristic MetaHeuristic(int maxValue) => MetaHeuristicsFactory.WhaleOptimisationAlgorithm(false, 100,  geneValue => geneValue, GetGeneValueFunction);

            //Termination
            var minFitness = 1;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(5);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            var reinsertion = new FitnessBasedElitistReinsertion();

            var compoundResults = CompareMetaHeuristicsKnownFunctionsDifferentSizes(maxCoordinate, StandardHeuristic, MetaHeuristic, crossover, SmallSizes, termination, reinsertion);

            for (int i = 0; i < compoundResults.Count; i++)
            {
                var functionResults = compoundResults[i];
                var meanRatio = functionResults.Sum(c => c.result2.Fitness / c.result1.Fitness) / functionResults.Count;

                Assert.GreaterOrEqual(meanRatio, resultsRatio[i]);
            }

        }



        [Test]
        public void Compare_WOA_Uniform_Stub_Large_LargerFitness()
        {
           
            var crossover = new UniformCrossover();
            var results = Compare_WOAReduced_Crossover_ChromosomeStub(crossover, LargeSizes);

            var meanRatio = results.Sum(c => c.result2.Fitness / c.result1.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, 0.95);

        }

        [Test]
        public void Compare_WOA_OnePoint_Stub_Small_LargerFitness()
        {
            var crossover = new OnePointCrossover(2);
            var results = Compare_WOAReduced_Crossover_ChromosomeStub(crossover, SmallSizes);

            var meanRatio = results.Sum(c => c.result2.Fitness / c.result1.Fitness) / results.Count;

            Assert.GreaterOrEqual(meanRatio, 1);

        }


        [Test]
        public void Compare_WOA_WOAParams_Stub_Large_MoreGenerations()
        {
            IMetaHeuristic OriginalMetaHeuristic(int maxValue) => GetDefaultWhaleHeuristicForChromosomStub(false, 300, maxValue);
            IMetaHeuristic ReducedMetaHeuristic(int maxValue) => GetDefaultWhaleHeuristicForChromosomStub(true, 300, maxValue);
            var crossover = new UniformCrossover();


            //Population Size
            var populationSize = 100;

            //Termination
            var minFitness = 1.0;
            int maxNbGenerations = 2000;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(2);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            var reinsertion = new FitnessBasedElitistReinsertion();

            var results = CompareMetaHeuristicsDifferentSizes(
               LargeSizes, 
                maxValue =>new FitnessStub(maxValue) { SupportsParallel = false }, 
                maxValue => new ChromosomeStub(maxValue, maxValue) , 
                OriginalMetaHeuristic, 
                ReducedMetaHeuristic, 
                crossover,
               populationSize, termination, reinsertion);


            var meanRatio = results.Sum(c => c.result2.Population.GenerationsNumber / (double) c.result1.Population.GenerationsNumber) / results.Count;

            Assert.Greater(meanRatio, 1.1);

        }

        private enum  KnownMetaheuristics
        {
            Default,
            WhaleOptmizerAlgorithm
        }



        //[Test()]
        public void GridSearch_WOA()
        {
            var repeatNb = 5;
            var testParams = new List<(KnownMetaheuristics kind,  double seconds, double helicoidScale, int nbGenerationsWOA, bool noMutation)>
            {
                (KnownMetaheuristics.Default,  1, 1, 200, true),
                (KnownMetaheuristics.WhaleOptmizerAlgorithm,  1.0, 1.0, 100,  true),
                (KnownMetaheuristics.WhaleOptmizerAlgorithm,  1.0, 1.0, 50,  true),
            };

            var sizes = new[] {/*50,*/ 100/*, 200 */}.ToList();

            // population parameters
            int populationSize = 100;

            //Termination
            var minFitness = double.MaxValue;
            int maxNbGenerations = 100000;
            int stagnationNb = 100000;

            //var reinsertion = new FitnessBasedElitistReinsertion();
            var reinsertion = new PureReinsertion();

            var crossover = new UniformCrossover();
            

            var maxCoordinate = 10;

            double GetGeneValueFunction(double d) => Math.Sign(d) * Math.Min(Math.Abs(d), maxCoordinate);
            IChromosome AdamChromosome(int i) => new EquationChromosome<double>(-maxCoordinate, maxCoordinate, i) {GetGeneValueFunction = GetGeneValueFunction};

            var knownFunctions = GetKnownFunctions();//.Skip(2).Take(1);

            var functionResults = new List<List<List<MeanEvolutionResult>>>();
            foreach (var functionToSolve in knownFunctions)
            {
                IFitness Fitness(int i) => new FunctionFitness<double>(functionToSolve);


                var sizeResults = new List<List<MeanEvolutionResult>>();
                foreach (var size in sizes)
                {

                    var testResults = new List<MeanEvolutionResult>();

                    
                    foreach (var (kind, duration, helicoidScale, nbGenerationsWoa, noMutation) in testParams)
                    {

                        TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(duration);
                        var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
                        IMetaHeuristic metaHeuristic;
                        if (kind == KnownMetaheuristics.WhaleOptmizerAlgorithm)
                        {
                           var woaMetaHeuristic = MetaHeuristicsFactory.WhaleOptimisationAlgorithm(false,
                               nbGenerationsWoa,
                               geneValue => geneValue, 
                               GetGeneValueFunction, helicoidScale: helicoidScale,
                               noMutation:noMutation);
                            metaHeuristic = woaMetaHeuristic;

                        }
                        else
                        {
                            metaHeuristic = new DefaultMetaHeuristic();
                        }

                        var meanResult = new MeanEvolutionResult();
                        for (int i = 0; i < repeatNb; i++)
                        {
                            var target = InitGa(metaHeuristic, Fitness(size), AdamChromosome(size), crossover, populationSize, termination, reinsertion);
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





        private IList<(EvolutionResult result1, EvolutionResult result2)> Compare_WOAReduced_Crossover_ChromosomeStub(ICrossover crossover, IEnumerable<int> sizes)
        {
            IMetaHeuristic StandardHeuristic(int i) => new DefaultMetaHeuristic();
            IMetaHeuristic MetaHeuristic(int i) => GetDefaultWhaleHeuristicForChromosomStub(true, 50, i);

            IFitness Fitness(int i) => new FitnessStub(i) {SupportsParallel = false};
            IChromosome AdamChromosome(int i) => new ChromosomeStub(i, i);


            //Population Size
            var populationSize = 100;

            //Termination
            var minFitness = 1;
            int maxNbGenerations = int.MaxValue;
            int stagnationNb = 100;
            TimeSpan maxTimeEvolving = TimeSpan.FromSeconds(5);
            var termination = GetTermination(minFitness, maxNbGenerations, stagnationNb, maxTimeEvolving);
            var reinsertion = new FitnessBasedElitistReinsertion();

            var results = CompareMetaHeuristicsDifferentSizes(
                sizes,
                Fitness,
                AdamChromosome,
                StandardHeuristic,
                MetaHeuristic,
                crossover, populationSize, termination, reinsertion);

            return results;

        }

       

        private IMetaHeuristic GetDefaultWhaleHeuristicForChromosomStub(bool reduced, int maxOperations, int maxValue)
        {
            double FromGene(int geneValue) => geneValue;
            Func<Double, int> ToGene =  ChromosomeStub.GeneFromDouble(maxValue);

            if (!reduced)
            {
                return MetaHeuristicsFactory.WhaleOptimisationAlgorithmWithParams(false, maxOperations, FromGene, ToGene);
            }

            return MetaHeuristicsFactory.WhaleOptimisationAlgorithm(false, maxOperations,  FromGene, ToGene);

        }



        #endregion



    }
}
