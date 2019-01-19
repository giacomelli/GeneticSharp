using System;
using GeneticSharp.Domain.Crossovers;
using NUnit.Framework;

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

            Assert.AreEqual(12, actual.Count);
            var index = -1;
            Assert.AreEqual(typeof(AlternatingPositionCrossover), actual[++index]);
            Assert.AreEqual(typeof(CutAndSpliceCrossover), actual[++index]);
            Assert.AreEqual(typeof(CycleCrossover), actual[++index]);
            Assert.AreEqual(typeof(OnePointCrossover), actual[++index]);
            Assert.AreEqual(typeof(OrderBasedCrossover), actual[++index]);
            Assert.AreEqual(typeof(OrderedCrossover), actual[++index]);
            Assert.AreEqual(typeof(PartiallyMappedCrossover), actual[++index]);
			Assert.AreEqual(typeof(PositionBasedCrossover), actual[++index]);
            Assert.AreEqual(typeof(ThreeParentCrossover), actual[++index]);
            Assert.AreEqual(typeof(TwoPointCrossover), actual[++index]);
            Assert.AreEqual(typeof(UniformCrossover), actual[++index]);
            Assert.AreEqual(typeof(VotingRecombinationCrossover), actual[++index]);
        }

        [Test()]
        public void GetCrossoverNames_NoArgs_AllAvailableCrossoversNames()
        {
            var actual = CrossoverService.GetCrossoverNames();

            Assert.AreEqual(12, actual.Count);
            var index = -1;
            Assert.AreEqual("Alternating-position (AP)", actual[++index]);
            Assert.AreEqual("Cut and Splice", actual[++index]);
            Assert.AreEqual("Cycle (CX)", actual[++index]);
            Assert.AreEqual("One-Point", actual[++index]);
            Assert.AreEqual("Order-based (OX2)", actual[++index]);
            Assert.AreEqual("Ordered (OX1)", actual[++index]);
            Assert.AreEqual("Partially Mapped (PMX)", actual[++index]);
			Assert.AreEqual("Position-based (POS)", actual[++index]);
            Assert.AreEqual("Three Parent", actual[++index]);
            Assert.AreEqual("Two-Point", actual[++index]);
            Assert.AreEqual("Uniform", actual[++index]);
            Assert.AreEqual("Voting Recombination (VR)", actual[++index]);
        }

        [Test()]
        public void CreateCrossoverByName_InvalidName_Exception()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                CrossoverService.CreateCrossoverByName("Test");
            }, "There is no ICrossover implementation with name 'Test'.");
        }

        [Test()]
        public void CreateCrossoverByName_ValidNameButInvalidConstructorArgs_Exception()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                CrossoverService.CreateCrossoverByName("One-Point", 1, 2, 3);
            }, "A ICrossover's implementation with name 'One-Point' was found, but seems the constructor args were invalid");
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
            Assert.Catch<ArgumentException>(() =>
            {
                CrossoverService.GetCrossoverTypeByName("Test");
            }, "There is no ICrossover implementation with name 'Test'.");
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