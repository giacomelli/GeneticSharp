using System;
using NUnit.Framework;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
	[TestFixture]
	public class IntegerChromosomeTest
	{
		[Test]
		public void ToFloatingPoint_NoArgs_Double()
		{
			RandomizationProvider.Current = Substitute.For<IRandomization> ();
			RandomizationProvider.Current.GetInt (0, 3).Returns (2);
			var target = new IntegerChromosome (0, 3);
			var actual = target.ToInteger();

			Assert.AreEqual (2, actual);
		}
	}
}

