using System;
using GeneticSharp.Domain.Chromosomes;
using NUnit.Framework;
using Rhino.Mocks;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [TestFixture]
    public class ChromosomeBaseTest
    {
        [Test]
        public void CompareTo_Others_DiffResults()
        {
            var target = MockRepository.GenerateMock<ChromosomeBase>();
            target.Fitness = 0.5;

            var other = MockRepository.GenerateMock<ChromosomeBase>();
            other.Fitness = 0.5;

            Assert.AreEqual(-1, target.CompareTo(null));
            Assert.AreEqual(0, target.CompareTo(other));            
            
            other.Fitness = 0.4;
            Assert.AreEqual(1, target.CompareTo(other));

            other.Fitness = 0.6;
            Assert.AreEqual(-1, target.CompareTo(other));
        }
    }
}