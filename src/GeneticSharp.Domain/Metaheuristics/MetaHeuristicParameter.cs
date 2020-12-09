using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GeneticSharp.Domain.Metaheuristics.Primitives;

namespace GeneticSharp.Domain.Metaheuristics
{

    public delegate TParamType ParameterGenerator<out TParamType>(IMetaHeuristic h, IEvolutionContext ctx);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1, TArg2 arg2);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2, in TArg3>(IMetaHeuristic h, IEvolutionContext ctx, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public interface IMetaHeuristicParameter
    {
         ParamScope Scope { get; set; }

        TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IEvolutionContext ctx, string key);
        
        object ComputeParameter(IMetaHeuristic h, IEvolutionContext ctx);


    }

    public interface IMetaHeuristicParameterGenerator<TParamType>: IMetaHeuristicParameter
    {

        ParameterGenerator<TParamType> GetGenerator(IEvolutionContext ctx);

    }


    public interface IExpressionGeneratorParameter: IMetaHeuristicParameter
    {

        LambdaExpression GetExpression(IEvolutionContext ctx, string paramName);

    }


    public class MetaHeuristicParameter<TParamType> : NamedEntity, IMetaHeuristicParameterGenerator<TParamType>
    {
       

        public ParamScope Scope { get; set; }


        private (string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) GetScopeMask((string key, int generation, EvolutionStage stage, IMetaHeuristic heuristic, int individual) input)
        {
            if ((Scope & ParamScope.Generation) != ParamScope.Generation)
            {
                input.generation = 0;

            }
            if ((Scope & ParamScope.Stage) != ParamScope.Stage)
            {
                input.stage = EvolutionStage.All;
            }
            if ((Scope & ParamScope.MetaHeuristic) != ParamScope.MetaHeuristic)
            {
                input.heuristic = null;
            }
            if ((Scope & ParamScope.Individual) != ParamScope.Individual)
            {
                input.individual = 0;
            }

            return input;
        }

        public TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IEvolutionContext ctx, string key)
        {

            //var newKey = this.GetKey(h, ctx, key);
            var maskedTuple = GetScopeMask((key, ctx.Population?.GenerationsNumber ?? 0, ctx.CurrentStage, h, ctx.Index));

            var toReturn = (TItemType)ctx.GetOrAdd(maskedTuple, () => ComputeParameter(h, ctx));
            return toReturn;
        }



       public  object ComputeParameter(IMetaHeuristic h, IEvolutionContext ctx)
        {
            var toReturn = GetGenerator(ctx)(h, ctx);
            return toReturn;
        }

        public virtual ParameterGenerator<TParamType> GetGenerator(IEvolutionContext ctx)
        {
            return Generator;
        }


        public ParameterGenerator<TParamType> Generator { get; set; }

        public TParamType GetOrAdd(IMetaHeuristic h, IEvolutionContext ctx, string key)
        {
          var toReturn =  GetOrAdd<TParamType>(h, ctx, key);
          return toReturn;
        }
    }

   


    public class ExpressionMetaHeuristicParameter<TParamType> : MetaHeuristicParameter<TParamType>, IExpressionGeneratorParameter
    {

        private static readonly Dictionary<Type, MethodInfo> _getOrAddMethods = new Dictionary<Type, MethodInfo>();

        public override ParameterGenerator<TParamType> GetGenerator(IEvolutionContext ctx)
        {
            if (Generator == null)
            {
                Generator = GetDynamicGenerator(ctx).Compile();
            }

            return base.GetGenerator(ctx);
        }


        public virtual Expression<ParameterGenerator<TParamType>> GetDynamicGenerator(IEvolutionContext ctx)
        {
            return DynamicGenerator;
        }


        public Expression<ParameterGenerator<TParamType>> DynamicGenerator { get; set; }

        public static MethodInfo GetOrAddMethod
        {
            get
            {
                if (!_getOrAddMethods.TryGetValue(typeof(TParamType), out var toReturn))
                {
                    var methods =
                        typeof(MetaHeuristicParameter<TParamType>).GetMethods();

                    toReturn = methods.First(m => m.Name == nameof(GetOrAdd) && !m.IsGenericMethod);
                    lock (_getOrAddMethods)
                    {
                        _getOrAddMethods[typeof(TParamType)] = toReturn;
                    }
                }
                return toReturn;
            }
        }

        public LambdaExpression GetExpression(IEvolutionContext ctx, string paramName)
        {

            if (Scope == ParamScope.None)
            {
                return GetDynamicGenerator(ctx);

            }

            var unCached = GetDynamicGenerator(ctx);


            LambdaExpression cachedExpression = Expression.Lambda(Expression.Call(Expression.Constant(this), GetOrAddMethod, unCached.Parameters[0],
                    unCached.Parameters[1], Expression.Constant(paramName)), unCached.Parameters[0],
                unCached.Parameters[1]);
            return cachedExpression;

        }
    }

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


    public class ExpressionMetaHeuristicParameter<TParamType, TArg1> : ExpressionMetaHeuristicParameterWithArgs<TParamType>
    {
        protected override LambdaExpression GetExpressionWithArgs()
        {
            return DynamicGeneratorWithArg;
        }

        public Expression<ParameterGenerator<TParamType, TArg1>> DynamicGeneratorWithArg { get; set; }

    }


    public class ExpressionMetaHeuristicParameter<TParamType, TArg1, TArg2> : ExpressionMetaHeuristicParameterWithArgs<TParamType>
    {
        protected override LambdaExpression GetExpressionWithArgs()
        {
            return DynamicGeneratorWithArgs;
        }

        public Expression<ParameterGenerator<TParamType, TArg1, TArg2>> DynamicGeneratorWithArgs { get; set; }

    }

    public class ExpressionMetaHeuristicParameter<TParamType, TArg1, TArg2, TArg3> : ExpressionMetaHeuristicParameterWithArgs<TParamType>
    {
        protected override LambdaExpression GetExpressionWithArgs()
        {
            return DynamicGeneratorWithArgs;
        }

        public Expression<ParameterGenerator<TParamType, TArg1, TArg2, TArg3>> DynamicGeneratorWithArgs { get; set; }

    }


}