using System;
using NUnit.Framework;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
	[TestFixture]
	public class FloatingPointChromosomeTest
	{
		[Test]
		public void ToFloatingPoint_PositiveValue_Double()
		{
			RandomizationProvider.Current = Substitute.For<IRandomization>();
			RandomizationProvider.Current.GetDouble(0.5, 2.5).Returns(1.1);

            var target = new FloatingPointChromosome(0.5, 2.5, 64, 2);
            Assert.AreEqual("0000000000000000000000000000000000000000000000000000000001101110", target.ToString());

            var actual = target.ToFloatingPoint();
			Assert.AreEqual(1.1, actual);
		}

        [Test]
        public void ToFloatingPoint_NegativeValue_Double()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetDouble(-2.5, 0.5).Returns(-1.1);

            var target = new FloatingPointChromosome(-2.5, 0.5, 64, 2);
            Assert.AreEqual("1111111111111111111111111111111111111111111111111111111110010010", target.ToString());

            var actual = target.ToFloatingPoint();
            Assert.AreEqual(-1.1, actual);
        }

        [Test]
		public void ToFloatingPoints_NoArgs_Double()
		{
			RandomizationProvider.Current = Substitute.For<IRandomization>();
			RandomizationProvider.Current.GetDouble(0, 10).Returns(1);
			RandomizationProvider.Current.GetDouble(1, 11).Returns(2);
			RandomizationProvider.Current.GetDouble(2, 12).Returns(3);
			var target = new FloatingPointChromosome(new double[] { 0, 1, 2 }, new double[] { 10, 11, 12 }, new int[] { 8, 8, 8 }, new int[] { 0, 0, 0 });
			var actual = target.ToFloatingPoints();

			Assert.AreEqual(3, actual.Length);
			Assert.AreEqual(1, actual[0]);
			Assert.AreEqual(2, actual[1]);
			Assert.AreEqual(3, actual[2]);
		}

        [Test]
        public void ToFloatingPoint_LowerMinValue_MinValue()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetDouble(0.5, 2.5).Returns(0.4);

            var target = new FloatingPointChromosome(0.5, 2.5, 64, 2);
            var actual = target.ToFloatingPoint();
            Assert.AreEqual(0.5, actual);
        }

        [Test]
        public void ToFloatingPoint_GreaterMaxValue_MaxValue()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetDouble(0.5, 2.5).Returns(2.6);

            var target = new FloatingPointChromosome(0.5, 2.5, 64, 2);
            var actual = target.ToFloatingPoint();
            Assert.AreEqual(2.5, actual);
        }

        [Test]
		public void Constructor_FromZeroToZero_Double()
		{
			RandomizationProvider.Current = Substitute.For<IRandomization>();
			RandomizationProvider.Current.GetDouble(0, 0).Returns(0);
			var target = new FloatingPointChromosome(0, 0, 2);
			var actual = target.ToFloatingPoint();

			Assert.AreEqual(0, actual);
		}

        [Test]
        public void FlipGene_Index_ValueFlip()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetDouble(0, 0).Returns(0);
            var target = new FloatingPointChromosome(0, 0, 2);

            target.FlipGene(0);
            Assert.AreEqual("10000000000000000000000000000000", target.ToString());

            target.FlipGene(1);
            Assert.AreEqual("11000000000000000000000000000000", target.ToString());

            target.FlipGene(31);
            Assert.AreEqual("11000000000000000000000000000001", target.ToString());

            target.FlipGene(30);
            Assert.AreEqual("11000000000000000000000000000011", target.ToString());
        }
    }
}

