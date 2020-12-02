using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics
{
    public abstract class OperatorHeuristic<TOperator> : ContainerMetaHeuristic
    {


        public IMetaHeuristicParameterGenerator<TOperator> DynamicParameter { get; set; }

        public TOperator StaticOperator { get; set; }

        

        protected TOperator GetOperator(IMetaHeuristicContext ctx)
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

            //if (DynamicOperator == null)
            //{
            //    DynamicOperator = ParameterReplacer.ReduceLambdaParameterGenerator<TOperator>(DynamicOperatorWithArgs, ctx).Compile();
            //}

            //if (DynamicParameter.Scope != ParameterScope.None)
            //{
            //    var toReturn =  DynamicParameter.GetOrAdd<TOperator>(this, ctx, "DynamicOperator");
            //    if (DynamicParameter.Scope == ParameterScope.Constant )
            //    {
            //        StaticOperator = toReturn;
            //    }
            //}

            //return DynamicOperator(this, ctx);
        }

    }
}