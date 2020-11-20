using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics
{

    public delegate TParamType ParameterGenerator<out TParamType>(IMetaHeuristic h, IMetaHeuristicContext ctx);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1, TArg2 arg2);

    public abstract class MetaHeuristicParameter
    {
        public ParameterScope Scope { get; set; }


        public abstract object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx);


    }

    public class MetaHeuristicParameter<TParamType> : MetaHeuristicParameter
    {
        public override object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx)
        {
            return GetGenerator(ctx)(h, ctx);
        }

        public virtual ParameterGenerator<TParamType> GetGenerator(IMetaHeuristicContext ctx)
        {
            return Generator;
        }


        public ParameterGenerator<TParamType> Generator { get; set; }

    }

    public interface IExpressionGeneratorParameter
    {

        LambdaExpression GetExpression(IMetaHeuristicContext ctx);

    }


    public class ExpressionMetaHeuristicParameter<TParamType> : MetaHeuristicParameter<TParamType>, IExpressionGeneratorParameter
    {


        public override ParameterGenerator<TParamType> GetGenerator(IMetaHeuristicContext ctx)
        {
            if (Generator == null)
            {
                Generator = GetDynamicGenerator(ctx).Compile();
            }

            return base.GetGenerator(ctx);
        }


        public virtual Expression<ParameterGenerator<TParamType>> GetDynamicGenerator(IMetaHeuristicContext ctx)
        {
            return DynamicGenerator;
        }


        public Expression<ParameterGenerator<TParamType>> DynamicGenerator { get; set; }

        public LambdaExpression GetExpression(IMetaHeuristicContext ctx)
        {
            return GetDynamicGenerator(ctx);
        }
    }




    public class ExpressionMetaHeuristicParameter<TParamType, TArg1> : ExpressionMetaHeuristicParameter<TParamType>
    {
       
        public override Expression<ParameterGenerator<TParamType>> GetDynamicGenerator(IMetaHeuristicContext ctx)
        {
            if (DynamicGenerator == null)
            {
                DynamicGenerator = ParameterReplacer.Reduce(DynamicGeneratorWithArg, ctx);
            }
            return base.GetDynamicGenerator(ctx);
        }

        public Expression<ParameterGenerator<TParamType, TArg1>> DynamicGeneratorWithArg { get; set; }

    }
}