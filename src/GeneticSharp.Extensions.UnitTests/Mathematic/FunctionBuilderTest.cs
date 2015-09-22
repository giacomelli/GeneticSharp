using System;
using NUnit.Framework;
using TestSharp;
using GeneticSharp.Extensions.Mathematic;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Terminations;

namespace GeneticSharp.Extensions.UnitTests.Mathematic
{
	[TestFixture()]
	[Category("Extensions")]
	public class FunctionBuilderTest
	{
		[Test()]
		public void Evolve_ManyGenerations_Fast()
		{
			var selection = new EliteSelection();
			var crossover = new ThreeParentCrossover();
			var mutation = new UniformMutation(true);

			var fitness = new FunctionBuilderFitness(
				new FunctionBuilderInput(
					new double[] { 1, 2, 3},
					6)
				,
				new FunctionBuilderInput(
					new double[] { 2, 3, 4},
					24)
			);
			var chromosome = new FunctionBuilderChromosome(fitness.AvailableOperations, 5);

			var population = new Population(100, 200, chromosome);

			var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
			ga.Termination = new FitnessThresholdTermination (0);
			ga.Start();
			var bestChromosome = ga.BestChromosome as FunctionBuilderChromosome;
			Assert.AreEqual (0.0, bestChromosome.Fitness.Value);
			var actual = fitness.GetFunctionResult (
				             bestChromosome.BuildFunction (), 
				             new FunctionBuilderInput (new double[] { 3, 4, 5 }, 60)
				);

			Assert.AreEqual (60.0, actual);
		}
	}
}