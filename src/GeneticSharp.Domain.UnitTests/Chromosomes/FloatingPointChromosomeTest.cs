using System;
using NUnit.Framework;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
	[TestFixture]
	public class FloatingPointChromosomeTest
	{
		[Test]
		public void ToFloatingPoint_NoArgs_Double()
		{
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization> ();
			RandomizationProvider.Current.Expect (r => r.GetDouble (0.5, 2.5)).Return (1.1);
			var target = new FloatingPointChromosome (0.5, 2.5, 2);
			var actual = target.ToFloatingPoint ();

			Assert.AreEqual (1.1, actual);
		}

		[Test]
		public void Constructor_FromZeroToZero_Double()
		{
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization>();
			RandomizationProvider.Current.Expect(r => r.GetDouble(0, 0)).Return(0);
			var target = new FloatingPointChromosome(0, 0, 2);
			var actual = target.ToFloatingPoint();

			Assert.AreEqual(0, actual);
		}
	}
}

