using System;
using NUnit.Framework;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
	[TestFixture]
	public class ScalableFloatingPointChromosomeTest
    {

        [Test]
        public void ToFloatingPoints_MaxValues()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetInt(0, 2).Returns(1);
            var target = new ScalableFloatingPointChromosome(new double[] { -5, 0, 5 }, new double[] { 0, 5, 10 }, new int[] { 3, 4, 5 });
            var actual = target.ToFloatingPoints();

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(0, actual[0]);
            Assert.AreEqual(5, actual[1]);
            Assert.AreEqual(10, actual[2]);
        }

        [Test]
        public void ToFloatingPoints_MinValues()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetInt(0, 0).Returns(0);
            var target = new ScalableFloatingPointChromosome(new double[] { -5, 0, 5 }, new double[] { 0, 5, 10 }, new int[] { 3, 4, 5 });
            var actual = target.ToFloatingPoints();

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(-5, actual[0]);
            Assert.AreEqual(0, actual[1]);
            Assert.AreEqual(5, actual[2]);
        }

        [Test]
        public void ToFloatingPoints_ExplicitBits()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetInt(0, 0).Returns(0);
            var target = new ScalableFloatingPointChromosome(new double[] { -5, 0, 5 }, new double[] { 0, 5, 10 }, new int[] { 3, 4, 5 }, new int[] { 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 1 });
            var actual = target.ToFloatingPoints();

            Assert.AreEqual(3, actual.Length);
            Assert.AreEqual(-4.2857142857142856, actual[0]);
            Assert.AreEqual(0.33333333333333331, actual[1]);
            Assert.AreEqual(5.161290322580645, actual[2]);
        }
    }
}

