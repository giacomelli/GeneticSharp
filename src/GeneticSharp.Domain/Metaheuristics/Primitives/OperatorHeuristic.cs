using GeneticSharp.Domain.Metaheuristics.Parameters;

namespace GeneticSharp.Domain.Metaheuristics.Primitives
{
    public abstract class OperatorHeuristic<TOperator> : ContainerMetaHeuristic
    {


        public IMetaHeuristicParameterGenerator<TOperator> DynamicParameter { get; set; }

        public TOperator StaticOperator { get; set; }

        

        protected TOperator GetOperator(IEvolutionContext ctx)
        {
            if (StaticOperator != null)
            {
                return StaticOperator;
            }

            var toReturn = DynamicParameter.GetGenerator(ctx)(this, ctx);
            if (DynamicParameter.Scope == ParamScope.Constant)
            {
                StaticOperator = toReturn;
            }

            return toReturn;

        }

    }
}