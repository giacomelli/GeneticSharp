using System;
using NUnit.Framework;
using GeneticSharp.Domain.Randomizations;
using TestSharp;
using GeneticSharp.Domain.Populations;
using Rhino.Mocks;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Terminations;
using HelperSharp;

namespace GeneticSharp.Domain.UnitTests
{
	[TestFixture()]
	public class GeneticAlgorithmTest
	{
		[TearDown]
		public void Cleanup()
		{
			RandomizationProvider.Current = new BasicRandomization();
		}

		[Test()]
		public void Constructor_NullPopulation_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("population"), () => {
				new GeneticAlgorithm (null, null, null, null, null);
			});		
		}

		[Test()]
		public void Constructor_NullFitness_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("fitness"), () => {
				new GeneticAlgorithm (new Population(2, 2, new ChromosomeStub()), null, null, null, null);
			});		
		}

		[Test()]
		public void Constructor_NullSelection_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("selection"), () => {
				new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()), MockRepository.GenerateMock<IFitness>(), null, null, null);
			});		
		}

		[Test()]
		public void Constructor_NullCrossover_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("crossover"), () => {
				new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()),  
				               MockRepository.GenerateMock<IFitness>(), 
				               MockRepository.GenerateMock<ISelection>(), null, null);
			});		
		}

		[Test()]
		public void Constructor_NullMutation_Exception ()
		{
			ExceptionAssert.IsThrowing (new ArgumentNullException ("mutation"), () => {
				new GeneticAlgorithm(new Population(2, 2, new ChromosomeStub()),
				               MockRepository.GenerateMock<IFitness>(), 
				               MockRepository.GenerateMock<ISelection>(), 
				               MockRepository.GenerateMock<ICrossover>(), null);
			});		
		}

		[Test()]
		public void Evolve_InvalidFitnessEvaluateResult_Exception()
		{
			var selection = new RouletteWheelSelection();
			var crossover = new OnePointCrossover(1);
			var mutation = new UniformMutation();
			var chromosome = new ChromosomeStub();
			var fitness = MockRepository.GenerateMock<IFitness>();
			fitness.Expect(f => f.Evaluate(null)).IgnoreArguments().Return(1.1);

			var target = new GeneticAlgorithm(
				new Population(20, 20, chromosome),
				fitness, selection, crossover, mutation);

			ExceptionAssert.IsThrowing(new FitnessException(fitness, "The {0}.Evaluate returns a fitness with value 1.1. The fitness value should be between 0.0 and 1.0.".With(fitness.GetType())), () => {
				target.Evolve();
			});

			fitness = MockRepository.GenerateMock<IFitness>();
			fitness.Expect(f => f.Evaluate(null)).IgnoreArguments().Return(-0.1);

			target = new GeneticAlgorithm(
				new Population(20, 20, chromosome),
				fitness, selection, crossover, mutation);

			ExceptionAssert.IsThrowing(new FitnessException(fitness, "The {0}.Evaluate returns a fitness with value -0.1. The fitness value should be between 0.0 and 1.0.".With(fitness.GetType())), () =>
			                           {
				target.Evolve();
			});
		}


		[Test()]
		public void Evolve_NotParallelManyGenerations_Optimization ()
		{
			var selection = new EliteSelection();
			var crossover = new OnePointCrossover(2);
			var mutation = new UniformMutation();
			var chromosome = new ChromosomeStub();
			var target = new GeneticAlgorithm( new Population (50, 50, chromosome),
				new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

			Assert.IsInstanceOf<EliteSelection>(target.Selection);
			Assert.IsInstanceOf<OnePointCrossover>(target.Crossover);
			Assert.IsInstanceOf<UniformMutation>(target.Mutation);

			target.Termination = new GenerationNumberTermination (25);
			target.Evolve ();
			Assert.AreEqual(25, target.Population.Generations.Count);

			var lastFitness = 0.0;

			foreach (var g in target.Population.Generations) {
				Assert.GreaterOrEqual(g.BestChromosome.Fitness.Value, lastFitness);
				lastFitness = g.BestChromosome.Fitness.Value;
			}

			Assert.GreaterOrEqual (lastFitness, 0.9);
		}

		[Test()]
		public void Evolve_ParallelManyGenerations_Optimization ()
		{
			var selection = new EliteSelection();
			var crossover = new OnePointCrossover(1);
			var mutation = new UniformMutation();
			var chromosome = new ChromosomeStub();
			var target = new GeneticAlgorithm(new Population (100, 150, chromosome),
				new FitnessStub() { SupportsParallel = true }, selection, crossover, mutation);

			Assert.IsInstanceOf<EliteSelection>(target.Selection);
			Assert.IsInstanceOf<OnePointCrossover>(target.Crossover);
			Assert.IsInstanceOf<UniformMutation>(target.Mutation);



			FlowAssert.IsAtLeastOneAttemptOk (8, () => {
				target.Evolve();
				Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count > 100);
				Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count <= 150);
				Assert.IsNotNull(target.Population.BestChromosome);
				Assert.IsTrue (target.Population.BestChromosome.Fitness >= 0.9);
			});

			FlowAssert.IsAtLeastOneAttemptOk (8, () => {
				target.Evolve();
				Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count > 100);
				Assert.IsTrue(target.Population.CurrentGeneration.Chromosomes.Count <= 150);
				Assert.IsNotNull(target.Population.BestChromosome);
				Assert.IsTrue (target.Population.BestChromosome.Fitness >= 0.9);
			});

			Assert.IsTrue (target.Population.Generations.Count > 0);
		}

		[Test()]
		public void Evolve_ParallelManySlowFitness_Timeout ()
		{
			var selection = new RouletteWheelSelection();
			var crossover = new OnePointCrossover(1);
			var mutation = new UniformMutation();
			var chromosome = new ChromosomeStub();
			var target = new GeneticAlgorithm(new Population (100, 150, chromosome),
				new FitnessStub() { SupportsParallel = true, ParallelSleep = 1500 }, selection, crossover, mutation);

			ExceptionAssert.IsThrowing (new TimeoutException("The RunGeneration reach the 1000 milliseconds timeout."), () => {
				target.Evolve (1000);
			});
		}

		[Test()]
		public void Evolve_NotParallelManyGenerations_Fast()
		{
			var selection = new EliteSelection();
			var crossover = new OnePointCrossover(2);
			var mutation = new UniformMutation();
			var chromosome = new ChromosomeStub();
			var target = new GeneticAlgorithm(new Population (100, 199, chromosome),
					new FitnessStub() { SupportsParallel = false }, selection, crossover, mutation);

			target.Termination = new GenerationNumberTermination (100);

			TimeAssert.LessThan (200, () => {
				target.Evolve();
			});

			Assert.AreEqual(100, target.Population.Generations.Count);        
			Assert.Greater (target.TimeEvolving.TotalMilliseconds, 1);
		}
	}
}

