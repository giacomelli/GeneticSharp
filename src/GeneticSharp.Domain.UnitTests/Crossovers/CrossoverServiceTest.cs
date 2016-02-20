using System;
using GeneticSharp.Domain.Crossovers;
using NUnit.Framework;
using TestSharp;

namespace GeneticSharp.Domain.UnitTests.Crossovers
{
    [TestFixture()]
    [Category("Crossovers")]
    public class CrossoverServiceTest
    {
        [Test()]
        public void GetCrossoverTypes_NoArgs_AllAvailableCrossovers()
        {
            var actual = CrossoverService.GetCrossoverTypes();

            Assert.AreEqual(10, actual.Count);
            Assert.AreEqual(typeof(CutAndSpliceCrossover), actual[0]);
            Assert.AreEqual(typeof(CycleCrossover), actual[1]);
            Assert.AreEqual(typeof(OnePointCrossover), actual[2]);
            Assert.AreEqual(typeof(OrderBasedCrossover), actual[3]);
            Assert.AreEqual(typeof(OrderedCrossover), actual[4]);
            Assert.AreEqual(typeof(PartiallyMappedCrossover), actual[5]);
			Assert.AreEqual(typeof(PositionBasedCrossover), actual[6]);
            Assert.AreEqual(typeof(ThreeParentCrossover), actual[7]);
            Assert.AreEqual(typeof(TwoPointCrossover), actual[8]);
            Assert.AreEqual(typeof(UniformCrossover), actual[9]);
        }

        [Test()]
        public void GetCrossoverNames_NoArgs_AllAvailableCrossoversNames()
        {
            var actual = CrossoverService.GetCrossoverNames();

            Assert.AreEqual(10, actual.Count);
            Assert.AreEqual("Cut and Splice", actual[0]);
            Assert.AreEqual("Cycle (CX)", actual[1]);
            Assert.AreEqual("One-Point", actual[2]);
            Assert.AreEqual("Order-based (OX2)", actual[3]);
            Assert.AreEqual("Ordered (OX1)", actual[4]);
            Assert.AreEqual("Partially Mapped (PMX)", actual[5]);
			Assert.AreEqual("Position-based (POS)", actual[6]);
            Assert.AreEqual("Three Parent", actual[7]);
            Assert.AreEqual("Two-Point", actual[8]);
            Assert.AreEqual("Uniform", actual[9]);
        }

        [Test()]
        public void CreateCrossoverByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no ICrossover implementation with name 'Test'.", "name"), () =>
            {
                CrossoverService.CreateCrossoverByName("Test");
            });
        }

        [Test()]
        public void CreateCrossoverByName_ValidNameButInvalidConstructorArgs_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("A ICrossover's implementation with name 'One-Point' was found, but seems the constructor args were invalid.", "constructorArgs"), () =>
            {
                CrossoverService.CreateCrossoverByName("One-Point", 1, 2, 3);
            });
        }

        [Test()]
        public void CreateCrossoverByName_ValidName_CrossoverCreated()
        {
            ICrossover actual = CrossoverService.CreateCrossoverByName("One-Point", 1) as OnePointCrossover;
            Assert.IsNotNull(actual);

            actual = CrossoverService.CreateCrossoverByName("Ordered (OX1)") as OrderedCrossover;
            Assert.IsNotNull(actual);

            actual = CrossoverService.CreateCrossoverByName("Two-Point", 1, 2) as TwoPointCrossover;
            Assert.IsNotNull(actual);

            actual = CrossoverService.CreateCrossoverByName("Uniform", 1f) as UniformCrossover;
            Assert.IsNotNull(actual);

            actual = CrossoverService.CreateCrossoverByName("Partially Mapped (PMX)") as PartiallyMappedCrossover;
            Assert.IsNotNull(actual);

            actual = CrossoverService.CreateCrossoverByName("Three Parent") as ThreeParentCrossover;
            Assert.IsNotNull(actual);

            actual = CrossoverService.CreateCrossoverByName("Cycle (CX)") as CycleCrossover;
            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetCrossoverTypeByName_InvalidName_Exception()
        {
            ExceptionAssert.IsThrowing(new ArgumentException("There is no ICrossover implementation with name 'Test'.", "name"), () =>
            {
                CrossoverService.GetCrossoverTypeByName("Test");
            });
        }

        [Test()]
        public void GetCrossoverTypeByName_ValidName_CrossoverTpe()
        {
            var actual = CrossoverService.GetCrossoverTypeByName("One-Point");
            Assert.AreEqual(typeof(OnePointCrossover), actual);

            actual = CrossoverService.GetCrossoverTypeByName("Ordered (OX1)");
            Assert.AreEqual(typeof(OrderedCrossover), actual);

            actual = CrossoverService.GetCrossoverTypeByName("Two-Point");
            Assert.AreEqual(typeof(TwoPointCrossover), actual);

            actual = CrossoverService.GetCrossoverTypeByName("Uniform");
            Assert.AreEqual(typeof(UniformCrossover), actual);

            actual = CrossoverService.GetCrossoverTypeByName("Partially Mapped (PMX)");
            Assert.AreEqual(typeof(PartiallyMappedCrossover), actual);

            actual = CrossoverService.GetCrossoverTypeByName("Three Parent");
            Assert.AreEqual(typeof(ThreeParentCrossover), actual);

            actual = CrossoverService.GetCrossoverTypeByName("Cycle (CX)");
            Assert.AreEqual(typeof(CycleCrossover), actual);
        }
    }
}