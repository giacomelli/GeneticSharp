using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Primitives;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class MetaHeuristicExtensionsTest : MetaHeuristicTestBase
    {

        [Test]
        public void WithName_CorrectlySetsNameAndDescription()
        {
            var testHeuristic = new DefaultMetaHeuristic().WithName("testName", "testDescription");
            Assert.AreEqual("testName", testHeuristic.Name);
            Assert.AreEqual("testDescription", testHeuristic.Description);

        }


    }
}