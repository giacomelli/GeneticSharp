using GeneticSharp.Domain.Metaheuristics;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture()]
    [Category("MetaHeuristics")]
    class MetaHeuristicContextTest : MetaHeuristicTestBase
    {

        [Test()]
        public void IndividualContext_HoldsCorrectInformation()
        {

            var generationContext = new EvolutionContext();
            generationContext.GetOrAdd(("test", 1, EvolutionStage.All, null, 0), () => 2);
            var indContext = generationContext.GetIndividual(3);
            Assert.AreEqual(3, indContext.Index);
            Assert.AreEqual(2, indContext.GetOrAdd(("test", 1, EvolutionStage.All, null, 0), ()=>3) );

        }


    }
}