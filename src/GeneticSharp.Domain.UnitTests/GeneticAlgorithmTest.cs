using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Reinsertions;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Infrastructure.Threading;
using HelperSharp;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp;

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
            ExceptionAssert.IsThrowing(new ArgumentNullException("population"), () =>
            {
                new GeneticAlgorithm(null, null, null, null, null);
            });
        }

        [Test()]
        public void Constructor_NullFitness_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("fitness"), () =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()), null, null, null, null);
            });
        }

        [Test()]
        public void Constructor_NullSelection_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("selection"), () =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()), MockRepository.GenerateMock<IFitness>(), null, null, null);
            });
        }

        [Test()]
        public void Constructor_NullCrossover_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("crossover"), () =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()),
                               MockRepository.GenerateMock<IFitness>(),
                               MockRepository.GenerateMock<ISelection>(), null, null);
            });
        }

        [Test()]
        public void Constructor_NullMutation_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("mutation"), () =>
            {
                new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()),
                               MockRepository.GenerateMock<IFitness>(),
                               MockRepository.GenerateMock<ISelection>(),
                               MockRepository.GenerateMock<ICrossover>(), null);
            });
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
            var taskExecutor = new SmartThreadPoolTaskExecutor();
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
            var taskExecutor = new SmartThreadPoolTaskExecutor();
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

            ExceptionAssert.IsThrowing(new TimeoutException("The fitness evaluation rech the 00:00:01 timeout."), () =>
            {
                target.Start();
            });

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
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Termination = new GenerationNumberTermination(100);

            TimeAssert.LessThan(30000, () =>
            {
                target.Start();
            });

            Assert.AreEqual(100, target.Population.Generations.Count);
            Assert.Greater(target.TimeEvolving.TotalMilliseconds, 1);
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

            ga1.Termination = new GenerationNumberTermination(1000);

            // GA 2     
            var selection2 = new EliteSelection();
            var crossover2 = new OnePointCrossover(2);
            var mutation2 = new UniformMutation();
            var chromosome2 = new ChromosomeStub();
            var ga2 = new GeneticAlgorithm(new Population(100, 199, chromosome2),
                    new FitnessStub() { SupportsParallel = false }, selection2, crossover2, mutation2);

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
            var mutations = MutationService.GetMutationNames();
            var reinsertions = ReinsertionService.GetReinsertionNames();
            var chromosome = new OrderedChromosomeStub();

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

                            target.Start();
                            Assert.AreEqual(25, target.Population.Generations.Count);
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
            var target = new GeneticAlgorithm(new Population(100, 199, chromosome),
                    new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

            target.Termination = new GenerationNumberTermination(100);

            target.Start();
            var lastTimeEvolving = target.TimeEvolving.TotalMilliseconds;
            Assert.AreEqual(100, target.Population.Generations.Count);
            Assert.Greater(target.TimeEvolving.TotalMilliseconds, 1);
            Assert.Less(target.TimeEvolving.TotalMilliseconds, 1000);
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);

            target.Termination = new GenerationNumberTermination(50);
            target.Start();
            Assert.AreEqual(50, target.Population.Generations.Count);
            Assert.Less(target.TimeEvolving.TotalMilliseconds, lastTimeEvolving);
            lastTimeEvolving = target.TimeEvolving.TotalMilliseconds;
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);

            target.Termination = new GenerationNumberTermination(25);
            target.Start();
            Assert.AreEqual(25, target.Population.Generations.Count);
            Assert.Less(target.TimeEvolving.TotalMilliseconds, lastTimeEvolving);
            Assert.AreEqual(GeneticAlgorithmState.TerminationReached, target.State);
            Assert.IsFalse(target.IsRunning);
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

            ExceptionAssert.IsThrowing(new InvalidOperationException("Attempt to stop a genetic algorithm which was not yet started."), () =>
            {
                target.Stop();
            });
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
            Assert.Greater(target.TimeEvolving.TotalMilliseconds, 8.8);
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

            ExceptionAssert.IsThrowing(new InvalidOperationException("Attempt to resume a genetic algorithm which was not yet started."), () =>
            {
                target.Resume();
            });
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
            target.TaskExecutor = new SmartThreadPoolTaskExecutor();

            var stoppedCount = 0;
            target.Stopped += (e, a) =>
            {
                Assert.AreEqual(GeneticAlgorithmState.Stopped, target.State);
                Assert.IsFalse(target.IsRunning);
                stoppedCount++;
            };

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

            ExceptionAssert.IsThrowing(new InvalidOperationException("Attempt to resume a genetic algorithm with a termination (GenerationNumberTermination (HasReached: True)) already reached. Please, specify a new termination or extend the current one."), () =>
            {
                target.Resume();
            });
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

            target.Termination = new GenerationNumberTermination(100);
            target.Start();
            Assert.AreEqual(target.Population.Generations.Count, 100);
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

    }
}

