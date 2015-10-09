using GeneticSharp.Extensions.Mathematic;
using NUnit.Framework;

namespace GeneticSharp.Extensions.UnitTests.Mathematic
{
    [TestFixture()]
    [Category("Extensions")]
    public class FunctionBuilderChromosomeTest
    {
        [Test()]
        public void BuildAvailableOperations_ParametersCount_AvailableOperations()
        {
            var actual = FunctionBuilderChromosome.BuildAvailableOperations(4);
            Assert.AreEqual(10, actual.Count);

            Assert.AreEqual("", actual[0]);
            Assert.AreEqual("+", actual[1]);
            Assert.AreEqual("-", actual[2]);
            Assert.AreEqual("/", actual[3]);
            Assert.AreEqual("*", actual[4]);
            Assert.AreEqual("__INT__", actual[5]);
            Assert.AreEqual("A", actual[6]);
            Assert.AreEqual("B", actual[7]);
            Assert.AreEqual("C", actual[8]);
            Assert.AreEqual("D", actual[9]);
        }
    }
}