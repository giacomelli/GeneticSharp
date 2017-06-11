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
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization>();
			RandomizationProvider.Current.Expect(r => r.GetDouble(0.5, 2.5)).Return(1.1);
			var target = new FloatingPointChromosome(0.5, 2.5, 2);
			var actual = target.ToFloatingPoint();

			Assert.AreEqual(1.1, actual);
		}

		[Test]
		public void ToFloatingPoints_NoArgs_Double()
		{
			RandomizationProvider.Current = MockRepository.GenerateMock<IRandomization>();
			RandomizationProvider.Current.Expect(r => r.GetDouble(0, 10)).Return(1);
			RandomizationProvider.Current.Expect(r => r.GetDouble(1, 11)).Return(2);
			RandomizationProvider.Current.Expect(r => r.GetDouble(2, 12)).Return(3);
			var target = new FloatingPointChromosome(new double[] { 0, 1, 2 }, new double[] { 10, 11, 12 }, new int[] { 8, 8, 8 }, new int[] { 0, 0, 0 });
			var actual = target.ToFloatingPoints();

			Assert.AreEqual(3, actual.Length);
			Assert.AreEqual(1, actual[0]);
			Assert.AreEqual(2, actual[1]);
			Assert.AreEqual(3, actual[2]);
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

