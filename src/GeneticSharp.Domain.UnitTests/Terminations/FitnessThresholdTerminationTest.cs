using System;
using NUnit.Framework;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
	[TestFixture()]
	public class FitnessThresholdTerminationTest
	{
		[Test()]
		public void HasReached_BestChromosomeHasLowerFitness_False ()
		{
			var target = new FitnessThresholdTermination (0.5);
			var generation = new Generation (10, new List<IChromosome> () {
				new ChromosomeStub() { Fitness = 0.4 },
				new ChromosomeStub() { Fitness = 0.499 }
			});

			generation.End (2);
			Assert.IsFalse(target.HasReached(generation));
		}

		[Test()]
		public void HasReached_BestChromosomeHasGreaterOrEqualFitness_True()
		{
			var target = new FitnessThresholdTermination (0.8);
			var generation = new Generation (10, new List<IChromosome> () {
				new ChromosomeStub() { Fitness = 0.4 },
				new ChromosomeStub() { Fitness = 0.8 }
			});

			generation.End (2);

			Assert.IsTrue(target.HasReached(generation));
		}
	}
}

