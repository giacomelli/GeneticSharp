using System;
using System.Collections.Generic;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture]
    [Category("Crossovers")]
    public class CrossoverBaseTest
    {
        [Test]
        public void Cross_Null_Exception()
        {
            var target = Substitute.ForPartsOf<CrossoverBase>(2, 2);

            Assert.Catch<ArgumentNullException>(() =>
            {
                target.Cross(null);
            });
        }

        [Test]
        public void Cross_InvalidNumberOfParents_Exception()
        {
            var target = Substitute.ForPartsOf<CrossoverBase>(2, 2);

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.Cross(new List<IChromosome>());
            }, "The number of parents should be the same of ParentsNumber.");

            Assert.Catch <ArgumentOutOfRangeException>(() =>
            {
                target.Cross(new List<IChromosome>() { Substitute.For<IChromosome>() });
            }, "The number of parents should be the same of ParentsNumber.");
        }
    }
}