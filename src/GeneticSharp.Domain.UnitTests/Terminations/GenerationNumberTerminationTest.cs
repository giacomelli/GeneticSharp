using System;
using NUnit.Framework;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
	[TestFixture()]
	public class GenerationNumberTerminationTest
	{
		[Test()]
		public void HasReached_GenerationNumberLowerThanCondition_False ()
		{
			var target = new GenerationNumberTermination (10);
			Assert.IsFalse(target.HasReached( new Generation (1, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
			Assert.IsFalse(target.HasReached( new Generation (9, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
		}

		[Test()]
		public void HasReached_GenerationNumberGreaterOrEqualThanCondition_True()
		{
			var target = new GenerationNumberTermination (10);
			Assert.IsTrue(target.HasReached(new Generation (10, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
			Assert.IsTrue(target.HasReached(new Generation (11, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
		}
	}
}

