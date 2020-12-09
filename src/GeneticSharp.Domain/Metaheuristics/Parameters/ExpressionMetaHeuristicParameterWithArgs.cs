using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics.Parameters
{
    public abstract class ExpressionMetaHeuristicParameterWithArgs<TParamType> : ExpressionMetaHeuristicParameter<TParamType>
    {

        public override Expression<ParameterGenerator<TParamType>> GetDynamicGenerator(IEvolutionContext ctx)
        {
            if (DynamicGenerator == null)
            {
                var expWithArgs = GetExpressionWithArgs();
                DynamicGenerator = ParameterReplacer.ReduceLambdaParameterGenerator<TParamType>(expWithArgs, ctx);
            }
            return base.GetDynamicGenerator(ctx);
        }

        protected abstract LambdaExpression GetExpressionWithArgs();

    }
}