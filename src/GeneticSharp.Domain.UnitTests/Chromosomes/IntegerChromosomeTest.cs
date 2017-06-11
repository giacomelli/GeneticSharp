using System;
using NUnit.Framework;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
	[TestFixture]
	public class IntegerChromosomeTest
	{
		[Test]
		public void ToFloatingPoint_NoArgs_Double()
		{
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization> ();
			RandomizationProvider.Current.Expect (r => r.GetInt (0, 3)).Return (2);
			var target = new IntegerChromosome (0, 3);
			var actual = target.ToInteger();

			Assert.AreEqual (2, actual);
		}
	}
}

