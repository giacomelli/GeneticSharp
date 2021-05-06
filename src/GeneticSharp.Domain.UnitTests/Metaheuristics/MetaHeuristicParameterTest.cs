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
                Generator = (heuristic, ctx) => ctx.OriginalIndex + 1, 
                Scope = ParamScope.Generation
            };


            IEvolutionContext generationContext = new EvolutionContext();
           var indContext = generationContext.GetIndividual(0);

            var int1 = param.Get(null, indContext, "test");
            Assert.AreEqual(1, int1);

            indContext = generationContext.GetIndividual(2);
            var int2 = param.Get(null, generationContext, "test");
            //scope is generation, the result should be cached and unchanged
            Assert.AreEqual(int1, int2);


        }


    }
}