using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics
{
    public abstract class OperatorHeuristic<TOperator> : ContainerMetaHeuristic
    {

        public OperatorHeuristic()
        {
            
        }

        public OperatorHeuristic(ParameterGenerator<TOperator> dynamicOperator)
        {
            DynamicOperator = dynamicOperator;
        }

        public OperatorHeuristic(TOperator staticOperator)
        {
            StaticOperator = staticOperator;
        }

        public TOperator StaticOperator { get; set; }

        public ParameterGenerator<TOperator> DynamicOperator { get; set; }

        public LambdaExpression DynamicOperatorWithArgs { get; set; }

        

        protected TOperator GetOperator(IMetaHeuristicContext ctx)
        {
            if (StaticOperator != null)
            {
                return StaticOperator;
            }

            if (DynamicOperator == null)
            {
                DynamicOperator = ParameterReplacer.ReduceLambda<TOperator>(DynamicOperatorWithArgs, ctx).Compile();
            }
            return DynamicOperator(this, ctx);
        }

    }
}