﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using System.Security.Cryptography;
using GeneticSharp.Domain.LifeSpans;

namespace GeneticSharp.Domain.UnitTests
{
    [TestFixture()]
    [Category("GA")]
    public class GeneticAlgorithmTest
    {
        [SetUp]
        public void SetUp()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [TearDown]
        public void Cleanup()
        {
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void Constructor_NullPopulation_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new GeneticAlgorithm(null, null, null, null, null);
            });

            Assert.AreEqual("population", actual.ParamName);
        }

        [Test()]
        public void Constructor_NullFitness_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()), null, null, null, null);
            });

            Assert.AreEqual("fitness", actual.ParamName);
        }

        [Test()]
        public void Constructor_NullSelection_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()), Substitute.For<IFitness>(), null, null, null);
            });

            Assert.AreEqual("selection", actual.ParamName);
        }

        [Test()]
        public void Constructor_NullCrossover_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()),
                               Substitute.For<IFitness>(),
                               Substitute.For<ISelection>(), null, null);
            });

            Assert.AreEqual("crossover", actual.ParamName);
        }

        [Test()]
        public void Constructor_NullMutation_Exception()
        {
            var actual = Assert.Catch<ArgumentNullException>(() =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()),
                               Substitute.For<IFitness>(),
                               Substitute.For<ISelection>(),
                               Substitute.For<ICrossover>(), null);
            });

            Assert.AreEqual("mutation", actual.ParamName);
        }

        [Test()]
        public void Start_NotParallelManyGenerations_Optimization()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(50, 50, chromosome),
                new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Population.GenerationStrategy = new TrackingGenerationStrategy();

            Assert.IsInstanceOf<EliteSelection>(target.Selection);
            Assert.IsInstanceOf<OnePointCrossover>(target.Crossover);
            Assert.IsInstanceOf<UniformMutation>(target.Mutation);

            target.Termination = new GenerationNumberTermination(25);
            Assert.AreEqual(GeneticAlgorithmState.NotStarted, target.State);
            Assert.IsFalse(target.IsRunning);

            target.Start();

            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);

            Assert.AreEqual(25, target.Population.Generations.Count);

            var lastFitness = 0.0;

            foreach (var g in target.Population.Generations)
            {
                Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
                lastFitness = g.BestChromosome.Fitness.Value;
            }

            Assert.GreaterOrEqual(lastFitness, 0.8);
            Assert.AreEqual(lastFitness, target.BestChromosome.Fitness);
        }

        [Test()]
        public void Start_ParallelManyGenerations_Optimization()
        {
            var taskExecutor = new ParallelTaskExecutor();
            taskExecutor.MinThreads = 100;
            taskExecutor.MaxThreads = 100;
    
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(1);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();

            FlowAssert.IsAtLeastOneAttemptOk(20, () =>
            {
                var target = new GeneticAlgorithm(new Population(100, 150, chromosome),
                new FitnessStub() { SupportsParallel = true }, selection, crossover, mutation);
                target.TaskExecutor = taskExecutor;

                Assert.AreEqual(GeneticAlgorithmState.NotStarted, target.State);
                Assert.IsFalse(target.IsRunning, "Should not be running before start");

                target.Start();

                Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
                Assert.IsFalse(target.IsRunning, "Should not be running anymore");
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count >= 100, "Chromosomes shoud be equal or greater than 100");
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count <= 150, "Chromosomes shoud be equal or less than 150");
                Assert.IsNotNull(target.Population.BestChromosome);
                Assert.IsTrue(target.Population.BestChromosome.Fitness >= 0.9, $"Fitness should be equal or greater than 0.9, but is {target.Population.BestChromosome.Fitness}");
                Assert.IsTrue(target.Population.Generations.Count > 0, "Generations should be greater than 0");
            });

            FlowAssert.IsAtLeastOneAttemptOk(20, () =>
            {
                var target = new GeneticAlgorithm(new Population(100, 150, chromosome),
                new FitnessStub() { SupportsParallel = true }, selection, crossover, mutation);
                target.TaskExecutor = taskExecutor;
                target.Start();
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count >= 100);
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count <= 150);
                Assert.IsNotNull(target.Population.BestChromosome);
                Assert.IsTrue(target.Population.BestChromosome.Fitness >= 0.9);
                Assert.IsTrue(target.Population.Generations.Count > 0);
            });
        }

        [Test()]
        public void Start_ParallelManySlowFitness_Timeout()
        {
            var taskExecutor = new ParallelTaskExecutor();
            taskExecutor.MinThreads = 100;
            taskExecutor.MaxThreads = 100;
            taskExecutor.Timeout = TimeSpan.FromMilliseconds(1000);

            var selection = new RouletteWheelSelection();
            var crossover = new OnePointCrossover(1);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 150, chromosome),
                new FitnessStub() { SupportsParallel = true, ParallelSleep = 1500 }, selection, crossover, mutation);
            target.TaskExecutor = taskExecutor;

            Assert.Catch<TimeoutException>(() =>
            {
                target.Start();
            }, "The fitness evaluation reached the 00:00:01 timeout.");

            Assert.IsFalse(target.IsRunning);
            Assert.AreEqual(GeneticAlgorithmState.Stopped, target.State);
        }

        [Test()]
        public void Start_FitnessEvaluationFailed_FitnessException()
        {
            var selection = new RouletteWheelSelection();
            var crossover = new OnePointCrossover(1);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var fitness = new FuncFitness((c) => throw new Exception("TEST"));

            var target = new GeneticAlgorithm(new Population(100, 150, chromosome), fitness, selection, crossover, mutation);
          
            Assert.Catch<FitnessException>(target.Start);

            Assert.IsFalse(target.IsRunning);
            Assert.AreEqual(GeneticAlgorithmState.Stopped, target.State);
        }

        [Test()]
          public void Start_NotParallelManyGenerations_Fast()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 100, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Population.GenerationStrategy = new TrackingGenerationStrategy();
            target.Termination = new GenerationNumberTermination(100);
            target.TaskExecutor = new LinearTaskExecutor();
            target.Start();
            
            Assert.AreEqual(100, target.Population.Generations.Count);
            Assert.Less(target.TimeEvolving.TotalMilliseconds, 200);
        }

        [Test()]
        public void Start_ParallelManyGenerations_Fast()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 100, chromosome),
                    new FitnessStub() { SupportsParallel = true, ParallelSleep = 50}, selection, crossover, mutation);

            target.Population.GenerationStrategy = new TrackingGenerationStrategy();
            target.Termination = new GenerationNumberTermination(100);
            target.TaskExecutor = new ParallelTaskExecutor();
            target.Start();

            Assert.AreEqual(100, target.Population.Generations.Count);
            Assert.Less(target.TimeEvolving.TotalSeconds, 10);
        }

        [Test()]
        public void Start_ParallelManyGenerations_Faster()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var fitness = new FitnessStub() {SupportsParallel = true, ParallelSleep = 1};
            var generationStrategy = new TrackingGenerationStrategy();
            var termination = new GenerationNumberTermination(100);
            // we test linear first
            var target = new GeneticAlgorithm(new Population(10, 10, chromosome), fitness
                , selection, crossover, mutation);
            target.Population.GenerationStrategy = generationStrategy;
            target.Termination = termination;
            target.TaskExecutor = new LinearTaskExecutor();
            target.Start();
            Assert.AreEqual(100, target.Population.Generations.Count);
            var linearTime = target.TimeEvolving.TotalMilliseconds;
            //then parallel

            target = new GeneticAlgorithm(new Population(10, 10, chromosome), fitness
                , selection, crossover, mutation);
            target.Population.GenerationStrategy = generationStrategy;
            target.Termination = termination;
            target.TaskExecutor = new TplTaskExecutor();
            target.Start();
            Assert.AreEqual(100, target.Population.Generations.Count);

            var parallelTime = target.TimeEvolving.TotalMilliseconds;


            Assert.Less(parallelTime, linearTime);
        }

        [Test()]
        public void Start_ParallelGAs_Fast()
        {
            // GA 1     
            var selection1 = new EliteSelection();
            var crossover1 = new OnePointCrossover(2);
            var mutation1 = new UniformMutation();
            var chromosome1 = new ChromosomeStub();
            var ga1 = new GeneticAlgorithm(new Population(100, 199, chromosome1),
                    new FitnessStub() { SupportsParallel = false }, selection1, crossover1, mutation1);

            ga1.Population.GenerationStrategy = new TrackingGenerationStrategy();
            ga1.Termination = new GenerationNumberTermination(1000);

            // GA 2     
            var selection2 = new EliteSelection();
            var crossover2 = new OnePointCrossover(2);
            var mutation2 = new UniformMutation();
            var chromosome2 = new ChromosomeStub();
            var ga2 = new GeneticAlgorithm(new Population(100, 199, chromosome2),
                    new FitnessStub() { SupportsParallel = false }, selection2, crossover2, mutation2);

            ga2.Population.GenerationStrategy = new TrackingGenerationStrategy();
            ga2.Termination = new GenerationNumberTermination(1000);

            Parallel.Invoke(
                () => ga1.Start(),
                () => ga2.Start());


            Assert.AreEqual(1000, ga1.Population.Generations.Count);
            Assert.AreEqual(1000, ga2.Population.Generations.Count);
        }

        [Test()]
        public void Start_TplManyGenerations_Optimization()
        {
            var taskExecutor = new TplTaskExecutor();
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(1);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();

            FlowAssert.IsAtLeastOneAttemptOk(20, () =>
            {
                var target = new GeneticAlgorithm(new TplPopulation(100, 150, chromosome),
                new FitnessStub() { SupportsParallel = true }, selection, crossover, mutation);
                target.OperatorsStrategy = new TplOperatorsStrategy();
                target.TaskExecutor = taskExecutor;

                Assert.AreEqual(GeneticAlgorithmState.NotStarted, target.State);
                Assert.IsFalse(target.IsRunning);

                target.Start();

                Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
                Assert.IsFalse(target.IsRunning);
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count >= 100);
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count <= 150);
                Assert.IsNotNull(target.Population.BestChromosome);
                Assert.IsTrue(target.Population.BestChromosome.Fitness >= 0.9);
                Assert.IsTrue(target.Population.Generations.Count > 0);
            });

            FlowAssert.IsAtLeastOneAttemptOk(20, () =>
            {
                var target = new GeneticAlgorithm(new TplPopulation(100, 150, chromosome),
                new FitnessStub() { SupportsParallel = true }, selection, crossover, mutation);
                target.OperatorsStrategy = new TplOperatorsStrategy();
                target.TaskExecutor = taskExecutor;
                target.Start();
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count >= 100);
                Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count <= 150);
                Assert.IsNotNull(target.Population.BestChromosome);
                Assert.IsTrue(target.Population.BestChromosome.Fitness >= 0.9);
                Assert.IsTrue(target.Population.Generations.Count > 0);
            });
        }

        [Test()]
        public void Start_TplManySlowFitness_Timeout()
        {
            var taskExecutor = new TplTaskExecutor();
            taskExecutor.Timeout = TimeSpan.FromMilliseconds(1000);

            var selection = new RouletteWheelSelection();
            var crossover = new OnePointCrossover(1);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new TplPopulation(100, 150, chromosome),
                new FitnessStub() { SupportsParallel = true, ParallelSleep = 1500 }, selection, crossover, mutation);
            target.OperatorsStrategy = new TplOperatorsStrategy();
            target.TaskExecutor = taskExecutor;

            Assert.Catch<TimeoutException>(() =>
            {
                target.Start();
            }, "The fitness evaluation reached the 00:00:01 timeout.");

            Assert.IsFalse(target.IsRunning);
            Assert.AreEqual(GeneticAlgorithmState.Stopped, target.State);
        }

        [Test()]
        public void Start_TplManyGenerations_Faster()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new TplPopulation(100, 100, chromosome),
                    new FitnessStub() { SupportsParallel = true }, selection, crossover, mutation);
            target.OperatorsStrategy = new TplOperatorsStrategy();

            target.Population.GenerationStrategy = new TrackingGenerationStrategy();
            target.Termination = new GenerationNumberTermination(100);
            target.TaskExecutor = new TplTaskExecutor();
            target.Start();

            Assert.AreEqual(100, target.Population.Generations.Count);
            Assert.Greater(target.TimeEvolving.TotalMilliseconds, 1);
        }

        [Test()]
        public void Start_TplGAs_Fast()
        {
            // GA 1     
            var selection1 = new EliteSelection();
            var crossover1 = new OnePointCrossover(2);
            var mutation1 = new UniformMutation();
            var chromosome1 = new ChromosomeStub();
            var ga1 = new GeneticAlgorithm(new TplPopulation(100, 199, chromosome1),
                    new FitnessStub() { SupportsParallel = false }, selection1, crossover1, mutation1);
            ga1.OperatorsStrategy = new TplOperatorsStrategy();

            ga1.Population.GenerationStrategy = new TrackingGenerationStrategy();
            ga1.Termination = new GenerationNumberTermination(1000);

            // GA 2     
            var selection2 = new EliteSelection();
            var crossover2 = new OnePointCrossover(2);
            var mutation2 = new UniformMutation();
            var chromosome2 = new ChromosomeStub();
            var ga2 = new GeneticAlgorithm(new TplPopulation(100, 199, chromosome2),
                    new FitnessStub() { SupportsParallel = false }, selection2, crossover2, mutation2);
            ga2.OperatorsStrategy = new TplOperatorsStrategy();

            ga2.Population.GenerationStrategy = new TrackingGenerationStrategy();
            ga2.Termination = new GenerationNumberTermination(1000);

            Parallel.Invoke(
                () => ga1.Start(),
                () => ga2.Start());


            Assert.AreEqual(1000, ga1.Population.Generations.Count);
            Assert.AreEqual(1000, ga2.Population.Generations.Count);
        }

        [Test()]
        public void Start_TerminationReached_TerminationReachedEventRaised()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);
            target.Population.GenerationStrategy = new TrackingGenerationStrategy();
            target.Termination = new GenerationNumberTermination(1);

            var raised = false;
            target.TerminationReached += (e, a) =>
            {
                raised = true;
            };

            target.Start();

            Assert.IsTrue(raised);
        }

        [Test()]
        public void Start_ThreeParentCrossover_KeepsMinSizePopulation()
        {
            var selection = new EliteSelection();
            var crossover = new ThreeParentCrossover();
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Population.GenerationStrategy = new TrackingGenerationStrategy();
            target.Termination = new GenerationNumberTermination(100);

            target.Start();

            Assert.AreEqual(100, target.Population.Generations.Count);

            Assert.IsTrue(target.Population.Generations.All(g => g.Chromosomes.Count >= 100));
        }

        [Test()]
        public void Start_UsingAllConfigurationCombinationsAvailable_AllRun()
        {
            var selections = SelectionService.GetSelectionNames();
            var crossovers = CrossoverService.GetCrossoverNames();
            var mutations = MutationService.GetMutationNames().Where(m => !m.Equals("Flip Bit"));
            var reinsertions = ReinsertionService.GetReinsertionNames().Where(m => !m.Equals("LifespanReinsertion"));
            var chromosome = new OrderedChromosomeStub();

            foreach (var LifeSpan in new bool[] { true, false })
            {


                foreach (var s in selections)
                {
                    foreach (var c in crossovers)
                    {
                        foreach (var m in mutations)
                        {
                            foreach (var r in reinsertions)
                            {
                                var selection = SelectionService.CreateSelectionByName(s);
                                var crossover = CrossoverService.CreateCrossoverByName(c);
                                var mutation = MutationService.CreateMutationByName(m);
                                var reinsertion = ReinsertionService.CreateReinsertionByName(r);
                                if (LifeSpan)
                                    reinsertion = new LifespanReinsertionDecorator(reinsertion);

                                if (crossover.IsOrdered ^ mutation.IsOrdered)
                                {
                                    continue;
                                }

                                if (crossover.ParentsNumber > crossover.ChildrenNumber && !reinsertion.CanExpand)
                                {
                                    continue;
                                }

                                if (mutation is UniformMutation)
                                {
                                    mutation = new UniformMutation(1);
                                }

                                var target = new GeneticAlgorithm(
                                     new Population(50, 50, chromosome.Clone())
                                     {
                                         GenerationStrategy = new TrackingGenerationStrategy()
                                     },
                                     new FitnessStub() { SupportsParallel = false },
                                     selection,
                                     crossover,
                                     mutation);

                                target.Reinsertion = reinsertion;
                                target.Termination = new GenerationNumberTermination(25);
                                target.CrossoverProbability = reinsertion.CanExpand ? 0.75f : 1f;

                                try
                                {
                                    target.Start();
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception($"GA start failed using selection:{s}, crossover:{c}, mutation:{m} and reinsertion:{r}. Error: {ex.Message}", ex);
                                }

                                Assert.AreEqual(25, target.Population.Generations.Count);
                            }
                        }
                    }
                }
            }
        }

        [Test()]
        public void Start_ManyCalls_NewEvolutions()
        {
            RandomizationProvider.Current = new BasicRandomization();
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Population.GenerationStrategy = new TrackingGenerationStrategy();
            target.Termination = new GenerationNumberTermination(1000);

            Assert.AreEqual(GeneticAlgorithmState.NotStarted, target.State);
            Assert.IsFalse(target.IsRunning);

            target.Start();

            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);
            var lastTimeEvolving = target.TimeEvolving.Ticks;
            Assert.AreEqual(1000, target.Population.Generations.Count);
            Assert.Greater(target.TimeEvolving.TotalMilliseconds, 1);

            target.Start();

            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);
            Assert.AreEqual(1000, target.Population.Generations.Count);
            Assert.AreNotEqual(lastTimeEvolving, target.TimeEvolving.Ticks);

            target.Start();

            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);
            Assert.AreEqual(1000, target.Population.Generations.Count);
            Assert.AreNotEqual(lastTimeEvolving, target.TimeEvolving.Ticks);
        }

        [Test()]
        public void Start_ManyCallsTerminationChanged_NewEvolutions()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();

            FlowAssert.IsAtLeastOneAttemptOk(10, () =>
            {
                var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

                target.Population.GenerationStrategy = new TrackingGenerationStrategy();
                target.Termination = new GenerationNumberTermination(500);

                target.Start();
                var lastTimeEvolving = target.TimeEvolving.TotalMilliseconds;
                Assert.AreEqual(500, target.Population.Generations.Count);
                Assert.Greater(target.TimeEvolving.TotalMilliseconds, 1);
                Assert.Less(target.TimeEvolving.TotalMilliseconds, 1500, "Time evolving should be less than 1000ms");
                Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
                Assert.IsFalse(target.IsRunning);

                target.Termination = new GenerationNumberTermination(100);
                target.Start();
                Assert.AreEqual(100, target.Population.Generations.Count);
                Assert.Less(target.TimeEvolving.TotalMilliseconds, lastTimeEvolving, "Time evolving 50 generations should be less than 100-199 generations");
                lastTimeEvolving = target.TimeEvolving.TotalMilliseconds;
                Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
                Assert.IsFalse(target.IsRunning);

                target.Termination = new GenerationNumberTermination(25);
                target.Start();
                Assert.AreEqual(25, target.Population.Generations.Count);
                Assert.Less(target.TimeEvolving.TotalMilliseconds, lastTimeEvolving);
                Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
                Assert.IsFalse(target.IsRunning);
            });
        }

        [Test]
        public void Start_FloatingPoingChromosome_Evolved()
        {
            var chromosome = new FloatingPointChromosome(
                new double[] { 0, 0, 0, 0 },
                new double[] { 1000, 1000, 1000, 1000 },
                new int[] { 10, 10, 10, 10 },
                new int[] { 0, 0, 0, 0 });

            var population = new Population(25, 25, chromosome);

            var fitness = new FuncFitness((c) =>
            {
                var f = c as FloatingPointChromosome;

                var values = f.ToFloatingPoints();
                var x1 = values[0];
                var y1 = values[1];
                var x2 = values[2];
                var y2 = values[3];

                // Euclidean distance: https://en.wikipedia.org/wiki/Euclidean_distance
                return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            });

            var selection = new EliteSelection();
            var crossover = new UniformCrossover();
            var mutation = new FlipBitMutation();
            var termination = new FitnessStagnationTermination(1000);

            var ga = new GeneticAlgorithm(
                population,
                fitness,
                selection,
                crossover,
                mutation);

            ga.Termination = termination;
            ga.Start();

            var bc = ga.BestChromosome as FloatingPointChromosome;
            var points = bc.ToFloatingPoints();
            Assert.AreEqual(4, points.Length);

            Assert.AreEqual(1414.2135623730951, bc.Fitness);
            Assert.GreaterOrEqual(ga.GenerationsNumber, 100);
        }

        [Test()]
        public void Stop_NotStarted_Exception()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            Assert.Catch<InvalidOperationException>(() =>
            {
                target.Stop();
            }, "Attempt to stop a genetic algorithm which was not yet started.");
        }

        [Test()]
        public void Stop_Started_Stopped()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Termination = new GenerationNumberTermination(10000);

            Parallel.Invoke(
            () => target.Start(),
            () =>
            {
                Thread.Sleep(10);
                Assert.AreEqual(GeneticAlgorithmState.Started, target.State);
                Assert.IsTrue(target.IsRunning);
                target.Stop();
                Thread.Sleep(30);

                Assert.AreEqual(GeneticAlgorithmState.Stopped, target.State);
                Assert.IsFalse(target.IsRunning);
            });

            Assert.Less(target.Population.Generations.Count, 10000);
            Assert.Less(target.TimeEvolving.TotalMilliseconds, 1000);
        }

        [Test()]
        public void Resume_NotStarted_Exception()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            Assert.Catch<InvalidOperationException>(() =>
            {
                target.Resume();
            }, "Attempt to resume a genetic algorithm which was not yet started.");
        }

        [Test()]
        public void Resume_Stopped_Resumed()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Termination = new TimeEvolvingTermination(TimeSpan.FromMilliseconds(10000));
            target.TaskExecutor = new ParallelTaskExecutor();

            var stoppedCount = 0;
            void handleStopped(object sender, EventArgs args)
            {
                Assert.AreEqual(GeneticAlgorithmState.Stopped, target.State);
                Assert.IsFalse(target.IsRunning);
                stoppedCount++;
            };

            // Listening stopped event.
            target.Stopped += handleStopped;

            Parallel.Invoke(
            () => target.Start(),
            () =>
            {
                Thread.Sleep(500);
                target.Stop();
            });

            Thread.Sleep(2000);

            Parallel.Invoke(
                () => target.Resume(),
                () =>
                {
                    Thread.Sleep(2000);
                    Assert.AreEqual(GeneticAlgorithmState.Resumed, target.State);
                    Assert.IsTrue(target.IsRunning);
                });

            Assert.AreEqual(1, stoppedCount);

            // Not listening Stopped event.
            target.Stopped -= handleStopped;
            
            Parallel.Invoke(
            () => target.Start(),
            () =>
            {
                Thread.Sleep(500);
                target.Stop();
            });

            Assert.AreEqual(1, stoppedCount);
        }

        [Test()]
        public void Resume_TerminationReachedAndTerminationNotChanged_Exception()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Termination = new GenerationNumberTermination(10);
            target.Start();
            Assert.AreEqual(10, target.Population.Generations.Count);
            var timeEvolving = target.TimeEvolving.Ticks;
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);

            Assert.Catch<InvalidOperationException>(() =>
            {
                target.Resume();
            }, "Attempt to resume a genetic algorithm with a termination (GenerationNumberTermination (HasReached: True)) already reached. Please, specify a new termination or extend the current one.");
        }

        [Test()]
        public void Resume_TerminationReachedAndTerminationExtend_Resumed()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Population.GenerationStrategy = new TrackingGenerationStrategy();
            target.Termination = new GenerationNumberTermination(100);
            target.Start();
            Assert.AreEqual(100, target.Population.Generations.Count);
            var timeEvolving = target.TimeEvolving.Ticks;
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);

            target.Termination = new GenerationNumberTermination(200);
            target.Resume();
            Assert.AreEqual(target.Population.Generations.Count, 200);
            Assert.Less(timeEvolving, target.TimeEvolving.Ticks);
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);

            target.Termination = new GenerationNumberTermination(300);
            target.Resume();
            Assert.AreEqual(target.Population.Generations.Count, 300);
            Assert.Less(timeEvolving, target.TimeEvolving.Ticks);
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);
        }
        
        [Test]
        public void Resume_PopulationFromAnotherGeneticAlgorithm_Resumed()
        {
            // Arrange
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var population = new Population(100, 199, chromosome)
            {
                GenerationStrategy = new TrackingGenerationStrategy()
            };
            var fitnessStub = new FitnessStub { SupportsParallel = false };
            
            // initialize a GA and run it for 100 generations
            var initialGeneticAlgorithm = new GeneticAlgorithm(population, fitnessStub, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(100)
            };
            initialGeneticAlgorithm.Start();

            // initialize a new GA with the same population to run it for 100 generations more
            var target = new GeneticAlgorithm(population, fitnessStub, selection, crossover, mutation)
            {
                Termination = new GenerationNumberTermination(200)
            };

            // Act
            target.Resume();
            
            // Assert
            Assert.AreEqual(200, target.Population.Generations.Count);
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);
        }

    }
}

