using System;
using NUnit.Framework;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
	[TestFixture()]
	public class GenerationNumberTerminationTest
	{
		[Test()]
		public void HasReached_GenerationNumberLowerThanCondition_False ()
		{
			var repository = new MockRepository ();
			var ga = repository.StrictMock<IGeneticAlgorithm> ();

			using (repository.Ordered()) {
				ga.Expect (g => g.GenerationsNumber).Return (1);
				ga.Expect (g => g.GenerationsNumber).Return (2);
				ga.Expect (g => g.GenerationsNumber).Return (3);
				ga.Expect (g => g.GenerationsNumber).Return (4);
				ga.Expect (g => g.GenerationsNumber).Return (5);
				ga.Expect (g => g.GenerationsNumber).Return (6);
				ga.Expect (g => g.GenerationsNumber).Return (7);
				ga.Expect (g => g.GenerationsNumber).Return (8);
				ga.Expect (g => g.GenerationsNumber).Return (0);
			}
			repository.ReplayAll ();

			var target = new GenerationNumberTermination (10);
			Assert.IsFalse(target.HasReached(ga));

			for (int i = 0; i < 8; i++) {
				Assert.IsFalse (target.HasReached (ga));    
			}

		}

		[Test()]
		public void HasReached_GenerationNumberGreaterOrEqualThanCondition_True()
		{
			var repository = new MockRepository ();
			var ga = repository.StrictMock<IGeneticAlgorithm> ();

			using (repository.Ordered()) {
				ga.Expect (g => g.GenerationsNumber).Return (10);
				ga.Expect (g => g.GenerationsNumber).Return (11);
			}
			repository.ReplayAll ();

			var target = new GenerationNumberTermination (10);
			Assert.IsTrue (target.HasReached (ga));
			Assert.IsTrue (target.HasReached (ga));
		}
	}
}

