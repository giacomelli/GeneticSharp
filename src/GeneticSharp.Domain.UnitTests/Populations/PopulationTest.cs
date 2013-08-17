using System;
using System.Diagnostics;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using HelperSharp;
using NUnit.Framework;
using Rhino.Mocks;
using TestSharp; 
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Terminations;

namespace GeneticSharp.Domain.UnitTests.Populations
{
	[TestFixture()]
	public class PopulationTest
	{
		[TearDown]
		public void Cleanup()
		{
			RandomizationProvider.Current = new BasicRandomization();
		}

		[Test()]
		public void Constructor_InvalidMinSize_Exception ()
		{
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes."), () =>
            {
				new Population (-1, 1, null, null, null, null, null);
			});

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes."), () =>
            {
				new Population (0, 1, null, null, null, null, null);
			});

            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("minSize", "The minimum size for a population is 2 chromosomes."), () =>
            {
				new Population (1, 1, null, null, null, null, null);
			});
		}

        [Test()]
        public void Constructor_InvalidMaxSize_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentOutOfRangeException("maxSize", "The maximum size for a population should be equal or greater than minimum size."), () =>
            {
                new Population(2, 1, null, null, null, null, null);
            });           
        }

        [Test()]
        public void Constructor_NullAdamChromosome_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentNullException("adamChromosome"), () =>
            {
                new Population(2, 2, null, null, null, null, null);
            });
        }

		[Test()]
		public void Constructor_NullFitness_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("fitness"), () => {
				new Population (2, 2, MockRepository.GenerateMock<IChromosome>(), null, null, null, null);
			});		
		}

		[Test()]
		public void Constructor_NullSelection_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("selection"), () => {
                new Population(2, 2, MockRepository.GenerateMock<IChromosome>(), MockRepository.GenerateMock<IFitness>(), null, null, null);
			});		
		}

		[Test()]
		public void Constructor_NullCrossover_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("crossover"), () => {
                new Population(2, 2, 
                                MockRepository.GenerateMock<IChromosome>(),  
				                MockRepository.GenerateMock<IFitness>(), 
				                MockRepository.GenerateMock<ISelection>(), null, null);
			});		
		}

		[Test()]
		public void Constructor_NullMutation_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("mutation"), () => {
                new Population(2, 2, 
                                MockRepository.GenerateMock<IChromosome>(),  
				                MockRepository.GenerateMock<IFitness>(), 
				                MockRepository.GenerateMock<ISelection>(), 
				                MockRepository.GenerateMock<ICrossover>(), null);
			});		
		}

        [Test()]
        public void RunGeneration_InvalidFitnessEvaluateResult_Exception()
        {
            var selection = new RouletteWheelSelection();
            var crossover = new OnePointCrossover(1);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var fitness = MockRepository.GenerateMock<IFitness>();
            fitness.Expect(f => f.Evaluate(null)).IgnoreArguments().Return(1.1);

            var target = new Population(
                20,
                20,
                chromosome,
                fitness, selection, crossover, mutation);

            ExceptionAssert.IsThrowing(new FitnessException(fitness, "The {0}.Evaluate returns a fitness with value 1.1. The fitness value should be between 0.0 and 1.0.".With(fitness.GetType())), () => {
                target.RunGeneration();
            });

            fitness = MockRepository.GenerateMock<IFitness>();
            fitness.Expect(f => f.Evaluate(null)).IgnoreArguments().Return(-0.1);

            target = new Population(
                20,
                20,
                chromosome,
                fitness, selection, crossover, mutation);

			ExceptionAssert.IsThrowing(new FitnessException(fitness, "The {0}.Evaluate returns a fitness with value -0.1. The fitness value should be between 0.0 and 1.0.".With(fitness.GetType())), () =>
            {
                target.RunGeneration();
            });
        }


		[Test()]
		public void RunGeneration_NotParallelManyGenerations_Optimization ()
		{
			var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
			var target = new Population (
				50, 
                50,
                chromosome,
				new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

			Assert.AreEqual (50, target.MinSize);
			Assert.IsInstanceOf<EliteSelection>(target.Selection);
            Assert.IsInstanceOf<OnePointCrossover>(target.Crossover);
            Assert.IsInstanceOf<UniformMutation>(target.Mutation);

			target.Termination = new GenerationNumberTermination (25);
			target.RunGenerations ();
			Assert.AreEqual(25, target.Generations.Count);

			var lastFitness = 0.0;

			foreach (var g in target.Generations) {
				Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
				lastFitness = g.BestChromosome.Fitness.Value;
			}

			Assert.GreaterOrEqual (lastFitness, 0.9);
		}

		[Test()]
		public void RunGeneration_ParallelManyGenerations_Optimization ()
		{
			var selection = new EliteSelection();
			var crossover = new OnePointCrossover(1);
			var mutation = new UniformMutation();
			var chromosome = new ChromosomeStub();
			var target = new Population (
				100, 
                150,
				chromosome,
				new FitnessStub() { SupportsParallel = true }, selection, crossover, mutation);

			Assert.AreEqual (100, target.MinSize);
			Assert.IsInstanceOf<EliteSelection>(target.Selection);
			Assert.IsInstanceOf<OnePointCrossover>(target.Crossover);
			Assert.IsInstanceOf<UniformMutation>(target.Mutation);



			FlowAssert.IsAtLeastOneAttemptOk (8, () => {
				target.RunGeneration();
                Assert.IsTrue(target.CurrentGeneration.Chromosomes.Count > 100);
                Assert.IsTrue(target.CurrentGeneration.Chromosomes.Count <= 150);
				Assert.IsNotNull(target.BestChromosome);
				Assert.IsTrue (target.BestChromosome.Fitness >= 0.9);
			});

			FlowAssert.IsAtLeastOneAttemptOk (8, () => {
				target.RunGeneration();
				Assert.IsTrue(target.CurrentGeneration.Chromosomes.Count > 100);
				Assert.IsTrue(target.CurrentGeneration.Chromosomes.Count <= 150);
				Assert.IsNotNull(target.BestChromosome);
				Assert.IsTrue (target.BestChromosome.Fitness >= 0.9);
			});

			Assert.IsTrue (target.Generations.Count > 0);
		}

		[Test()]
		public void RunGeneration_ParallelManySlowFitness_Timeout ()
		{
			var selection = new RouletteWheelSelection();
			var crossover = new OnePointCrossover(1);
			var mutation = new UniformMutation();
			var chromosome = new ChromosomeStub();
			var target = new Population (
				100, 
				150,
				chromosome,
				new FitnessStub() { SupportsParallel = true, ParallelSleep = 1500 }, selection, crossover, mutation);

			ExceptionAssert.IsThrowing (new TimeoutException("The RunGeneration reach the 1000 milliseconds timeout."), () => {
				target.RunGeneration (1000);
			});
		}

        [Test()]
        public void RunGeneration_NotParallelManyGenerations_Fast()
        {
            var selection = new EliteSelection();
            var crossover = new OnePointCrossover(2);
            var mutation = new UniformMutation();
            var chromosome = new ChromosomeStub();
            var target = new Population(
                100,
                100,
                chromosome,
                new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

			target.Termination = new GenerationNumberTermination (100);

			TimeAssert.LessThan (200, () => {
				target.RunGenerations();
			});
           
            Assert.AreEqual(100, target.Generations.Count);        
        }
	}
}

