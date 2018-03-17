using System;
using GeneticSharp.Extensions.Mathematic;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Mathematic
{
    [TestFixture()]
    [Category("Extensions")]
    public class EqualtionChromosomeTest
    {
        [Test()]
        public void Constructor_ExpectedResult_Exception()
        {
            var actual = Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                new EquationChromosome(int.MaxValue, 2);
            }, "EquationChromosome expected value must be lower");

            Assert.AreEqual("expectedResult", actual.ParamName);
            Assert.AreEqual(actual.ActualValue, int.MaxValue);
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