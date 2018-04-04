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
		public void ToInteger_PositiveValue_Integer()
		{
			RandomizationProvider.Current = Substitute.For<IRandomization> ();
			RandomizationProvider.Current.GetInt (0, 3).Returns (2);

            var target = new IntegerChromosome (0, 3);
            Assert.AreEqual("00000000000000000000000000000010", target.ToString());

			var actual = target.ToInteger();
			Assert.AreEqual (2, actual);            
		}

        [Test]
        public void ToInteger_NegativeValue_Integer()
        {
            RandomizationProvider.Current = Substitute.For<IRandomization>();
            RandomizationProvider.Current.GetInt(0, 3).Returns(-2);

            var target = new IntegerChromosome(0, 3);
            Assert.AreEqual("11111111111111111111111111111110", target.ToString());

            var actual = target.ToInteger();
            Assert.AreEqual(-2, actual);
        }
    }
}

