using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class MetaHeuristicParameterTest : MetaHeuristicTestBase
    {

        [Test]
        public void GetOrAddMethod_RespectsScope()
        {


            var param = new MetaHeuristicParameter<int>
            {
                Generator = (heuristic, ctx) => ctx.Index + 1, 
                Scope = ParamScope.Generation
            };


            var generationContext = new EvolutionContext();
            generationContext.GetIndividual(0);

            var int1 = param.GetOrAdd(null, generationContext, "test");
            Assert.AreEqual(1, int1);

            generationContext.Index = 2;
            var int2 = param.GetOrAdd(null, generationContext, "test");
            //scope is generation, the result should be cached and unchanged
            Assert.AreEqual(int1, int2);


        }


    }
}