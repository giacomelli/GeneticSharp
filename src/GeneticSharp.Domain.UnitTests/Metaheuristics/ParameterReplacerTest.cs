using GeneticSharp.Domain.Metaheuristics;
using GeneticSharp.Domain.Metaheuristics.Parameters;
using NUnit.Framework;

namespace GeneticSharp.Domain.UnitTests.MetaHeuristics
{
    [TestFixture]
    [Category("MetaHeuristics")]
    class ParameterReplacerTest : MetaHeuristicTestBase
    {

        [Test]
        public void ExpressionWithSubExpresssionParameter_WithPropertContext_IsProperlyReducedToSingleExpression()
        {
            var paramNameAsString = "paramName";
            var ctxParam = new ExpressionMetaHeuristicParameter<int>()
                .WithName(paramNameAsString, "param description");
            ctxParam.DynamicGenerator = (heuristic, context) => context.Index + 1;

            var idx = 3;
            IEvolutionContext ctx = new EvolutionContext();
            ctx = ctx.GetIndividual(idx);
            ctx.RegisterParameter(paramNameAsString, ctxParam);

            var childParameter = new ExpressionMetaHeuristicParameter<int, int>
            {
                DynamicGeneratorWithArg = (heuristic, context, paramName) => paramName - 1
            };

            var reducedExpression =
                ParameterReplacer.ReduceLambdaParameterGenerator<int>(childParameter.DynamicGeneratorWithArg, ctx);

            Assert.AreEqual("(heuristic, context) => ((context.Index + 1) - 1)", reducedExpression.ToString());

        }


    }
}