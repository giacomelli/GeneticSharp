using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GeneticSharp.Domain.Metaheuristics.Parameters
{
    
    /// <summary>
    /// The Expression Metaheuristic parameter extends the default delegate-based default parameter class with supporting Lambda Expression. 
    /// </summary>
    /// <typeparam name="TParamType"></typeparam>
    public class ExpressionMetaHeuristicParameter<TParamType> : MetaHeuristicParameter<TParamType>,
        IExpressionGeneratorParameter
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

        public static MethodInfo GetMethod
        {
            get
            {
                if (!_getOrAddMethods.TryGetValue(typeof(TParamType), out var toReturn))
                {
                    var methods =
                        typeof(MetaHeuristicParameter<TParamType>).GetMethods();

                    toReturn = methods.First(m => m.Name == nameof(Get) && !m.IsGenericMethod);
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


            LambdaExpression cachedExpression = Expression.Lambda(Expression.Call(Expression.Constant(this),
                    GetMethod, unCached.Parameters[0],
                    unCached.Parameters[1], Expression.Constant(paramName)), unCached.Parameters[0],
                unCached.Parameters[1]);
            return cachedExpression;
        }
    }

    public class
        ExpressionMetaHeuristicParameter<TParamType, TArg1> : ExpressionMetaHeuristicParameterWithArgs<TParamType>
    {
        protected override LambdaExpression GetExpressionWithArgs()
        {
            return DynamicGeneratorWithArg;
        }

        public Expression<ParameterGenerator<TParamType, TArg1>> DynamicGeneratorWithArg { get; set; }
    }

    public class
        ExpressionMetaHeuristicParameter<TParamType, TArg1, TArg2> : ExpressionMetaHeuristicParameterWithArgs<TParamType
        >
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