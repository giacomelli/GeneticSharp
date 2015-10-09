using System;
using GeneticSharp.Extensions.Mathematic;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Extensions.UnitTests.Mathematic
{
    [TestFixture()]
    [Category("Extensions")]
    public class EqualtionChromosomeTest
    {
        [Test()]
        public void Constructor_ExpectedResult_Exception()
        {
            ExceptionAssert.IsThrowing(
                new ArgumentOutOfRangeException("expectedResult", int.MaxValue, "EquationChromosome expected value must be lower"),
                () =>
                {
                    new EquationChromosome(int.MaxValue, 2);
                });
        }

        [Test()]
        public void CreateNew_ExpectedResultAndLenth_Created()
        {
            var target = new EquationChromosome(10, 2);
            var newCreated = target.CreateNew() as EquationChromosome;
            Assert.AreEqual(target.Length, newCreated.Length);
            Assert.AreEqual(target.ResultRange, newCreated.ResultRange);
        }
    }
}