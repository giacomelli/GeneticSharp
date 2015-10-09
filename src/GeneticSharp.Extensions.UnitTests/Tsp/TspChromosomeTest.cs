using System;
using GeneticSharp.Extensions.Tsp;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Extensions.UnitTests.Tsp
{
    [TestFixture()]
    [Category("Extensions")]
    public class TspChromosomeTest
    {
        [Test()]
        public void Constructor_OneCity_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("The minimum length for a chromosome is 2 genes."), () =>
            {
                new TspChromosome(1);
            });
        }

        [Test()]
        public void GenerateGene_FitnessLowerThanZero_Zero()
        {
            var target = new TspChromosome(10);
            var cityIndex = Convert.ToDouble(target.GenerateGene(0).Value);
            Assert.IsTrue(cityIndex >= 0 && cityIndex < 10);
        }

        [Test()]
        public void Clone_NoArgs_Cloned()
        {
            var target = new TspChromosome(10);
            target.Distance = 123;

            var actual = target.Clone() as TspChromosome;
            Assert.IsFalse(Object.ReferenceEquals(target, actual));
            Assert.AreEqual(123, actual.Distance);
        }
    }
}

