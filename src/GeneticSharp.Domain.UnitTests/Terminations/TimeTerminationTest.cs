using System;
using NUnit.Framework;
using GeneticSharp.Domain.Terminations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Chromosomes;
using System.Collections.Generic;
using System.Threading;

namespace GeneticSharp.Domain.UnitTests.Terminations
{
	[TestFixture()]
	public class TimeTerminationTest
	{
		[Test()]
		public void HasReached_TimeLowerThanMaxTime_False ()
		{
			var target = new TimeTermination (TimeSpan.FromSeconds(1));
			Assert.IsFalse(target.HasReached( new Generation (1, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
			Assert.IsFalse(target.HasReached( new Generation (9, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
		}

		[Test()]
		public void HasReached_TimeGreaterOrEqualMaxTime_True()
		{
			var target = new TimeTermination (TimeSpan.FromSeconds(1));
			Assert.IsFalse(target.HasReached(new Generation (1, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
			Thread.Sleep (1000);
			Assert.IsTrue(target.HasReached(new Generation (9, new List<IChromosome>() { new ChromosomeStub(), new ChromosomeStub() })));
		}
	}
}

