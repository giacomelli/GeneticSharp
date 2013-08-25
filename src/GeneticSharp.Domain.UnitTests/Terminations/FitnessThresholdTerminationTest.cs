using System;
using NUnit.Framework;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Selections;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
	[TestFixture()]
	public class FitnessThresholdTerminationTest
	{
		[Test()]
		public void HasReached_BestChromosomeHasLowerFitness_False ()
		{
			var repository = new MockRepository ();
			var ga = repository.StrictMock<IGeneticAlgorithm> ();

			using (repository.Ordered()) {
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub() { Fitness = 0.4});
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub() { Fitness = 0.499});
			}
			repository.ReplayAll ();

			var target = new FitnessThresholdTermination (0.5);
			Assert.IsFalse(target.HasReached(ga));
			Assert.IsFalse(target.HasReached(ga));
		}

		[Test()]
		public void HasReached_BestChromosomeHasGreaterOrEqualFitness_True()
		{
			var repository = new MockRepository ();
			var ga = repository.StrictMock<IGeneticAlgorithm> ();

			using (repository.Ordered()) {
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub() { Fitness = 0.4});
				ga.Expect (g => g.BestChromosome).Return (new ChromosomeStub() { Fitness = 0.8});
			}
			repository.ReplayAll ();

			var target = new FitnessThresholdTermination (0.8);
		
			Assert.IsFalse(target.HasReached(ga));
			Assert.IsTrue(target.HasReached(ga));
		}
	}
}

