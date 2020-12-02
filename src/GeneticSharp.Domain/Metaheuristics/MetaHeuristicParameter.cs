using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GeneticSharp.Domain.Populations;

namespace GeneticSharp.Domain.Metaheuristics
{

    public delegate TParamType ParameterGenerator<out TParamType>(IMetaHeuristic h, IMetaHeuristicContext ctx);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1, TArg2 arg2);
    public delegate TParamType ParameterGenerator<out TParamType, in TArg1, in TArg2, in TArg3>(IMetaHeuristic h, IMetaHeuristicContext ctx, TArg1 arg1, TArg2 arg2, TArg3 arg3);

    public interface IMetaHeuristicParameter
    {
         ParamScope Scope { get; set; }

        TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IMetaHeuristicContext ctx, string key);
        
        object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx);


    }

    public interface IMetaHeuristicParameterGenerator<TParamType>: IMetaHeuristicParameter
    {

        ParameterGenerator<TParamType> GetGenerator(IMetaHeuristicContext ctx);

    }


    public interface IExpressionGeneratorParameter: IMetaHeuristicParameter
    {

        //string GetFormatKey();

        LambdaExpression GetExpression(IMetaHeuristicContext ctx, string paramName);

    }


    public class MetaHeuristicParameter<TParamType> : NamedEntity, IMetaHeuristicParameterGenerator<TParamType>
    {

       

        public ParamScope Scope { get; set; }

        //private static readonly Dictionary<ParameterScope, string> _formatKeys = new Dictionary<ParameterScope, string>();


        //public static string GetFormatKey(ParameterScope mScope)
        //{
        //        var sb = new StringBuilder("{0}");
        //        if ((mScope & ParameterScope.Generation) == ParameterScope.Generation)
        //        {
        //            sb.Append("G{1}");

        //        }
        //        if ((mScope & ParameterScope.Stage) == ParameterScope.Stage)
        //        {
        //            sb.Append("S{2}");
        //        }
        //        if ((mScope & ParameterScope.MetaHeuristic) == ParameterScope.MetaHeuristic)
        //        {
        //            sb.Append("H{3}");
        //        }
        //        if ((mScope & ParameterScope.Individual) == ParameterScope.Individual)
        //        {
        //            sb.Append("I{4}");
        //        }

        //        return sb.ToString();
        //}


        //public string GetFormatKey()
        //{
           
        //    _formatKeys.TryGetValue(Scope, out var toReturn);
        //    if (string.IsNullOrEmpty(toReturn))
        //    {
        //        toReturn = GetFormatKey(Scope);
        //        lock (_formatKeys)
        //        {
        //            _formatKeys[Scope] = toReturn;
        //        }
        //    }

        //    return toReturn;
        //}

        //private string _formatKey;

        //public string GetKey(IMetaHeuristic h, IMetaHeuristicContext ctx, string key)
        //{
        //    if (_formatKey == null)
        //    {
        //        _formatKey = GetFormatKey();
        //    }

        //    return GetKey(h, ctx, _formatKey, key);

        //}

        //public static string GetKey(IMetaHeuristic h, IMetaHeuristicContext ctx, string formatKey, string key)
        //{
        //    return string.Format(formatKey,
        //        key,
        //        ctx.Population.GenerationsNumber.ToStringLookup(),
        //       ((int) ctx.CurrentStage).ToStringLookup(),
        //        h.Guid,
        //        ctx.Index.ToStringLookup());
        //}

        private (string key, int generation, MetaHeuristicsStage stage, IMetaHeuristic heuristic, int individual) GetScopeMask((string key, int generation, MetaHeuristicsStage stage, IMetaHeuristic heuristic, int individual) input)
        {
            if ((Scope & ParamScope.Generation) != ParamScope.Generation)
            {
                input.generation = 0;

            }
            if ((Scope & ParamScope.Stage) != ParamScope.Stage)
            {
                input.stage = MetaHeuristicsStage.All;
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

        public TItemType GetOrAdd<TItemType>(IMetaHeuristic h, IMetaHeuristicContext ctx, string key)
        {

            //var newKey = this.GetKey(h, ctx, key);
            var maskedTuple = GetScopeMask((key, ctx.Population.GenerationsNumber, ctx.CurrentStage, h, ctx.Index));

            var toReturn = (TItemType)ctx.GetOrAdd(maskedTuple, () => (object)ComputeParameter(h, ctx));
            return toReturn;
        }



       public  object ComputeParameter(IMetaHeuristic h, IMetaHeuristicContext ctx)
        {
            var toReturn = GetGenerator(ctx)(h, ctx);
            return toReturn;
        }

        public virtual ParameterGenerator<TParamType> GetGenerator(IMetaHeuristicContext ctx)
        {
            return Generator;
        }


        public ParameterGenerator<TParamType> Generator { get; set; }

        public TParamType GetOrAdd(IMetaHeuristic h, IMetaHeuristicContext ctx, string key)
        {
          var toReturn =  GetOrAdd<TParamType>(h, ctx, key);
          return toReturn;
        }
    }

   


    public class ExpressionMetaHeuristicParameter<TParamType> : MetaHeuristicParameter<TParamType>, IExpressionGeneratorParameter
    {

        private static Dictionary<Type, MethodInfo> _getOrAddMethods = new Dictionary<Type, MethodInfo>();

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

        public static MethodInfo GetOrAddMethod
        {
            get
            {
                if (!_getOrAddMethods.TryGetValue(typeof(TParamType), out var toReturn))
                {
                    var methods =
                        typeof(MetaHeuristicParameter<TParamType>).GetMethods();

                    toReturn = methods.First(m => m.Name == nameof(MetaHeuristicParameter<TParamType>.GetOrAdd) && !m.IsGenericMethod);
                    lock (_getOrAddMethods)
                    {
                        _getOrAddMethods[typeof(TParamType)] = toReturn;
                    }
                }
                return toReturn;
            }
        }

        public LambdaExpression GetExpression(IMetaHeuristicContext ctx, string paramName)
        {

            if (Scope == ParamScope.None)
            {
                return GetDynamicGenerator(ctx);

            }
            else
            {
                var unCached = GetDynamicGenerator(ctx);


                LambdaExpression cachedExpression = Expression.Lambda(Expression.Call(Expression.Constant(this), GetOrAddMethod, unCached.Parameters[0],
                        unCached.Parameters[1], Expression.Constant(paramName)), unCached.Parameters[0],
                    unCached.Parameters[1]);
                return cachedExpression;


            }

        }
    }

    public abstract class ExpressionMetaHeuristicParameterWithArgs<TParamType> : ExpressionMetaHeuristicParameter<TParamType>
    {

        public override Expression<ParameterGenerator<TParamType>> GetDynamicGenerator(IMetaHeuristicContext ctx)
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

    static class ToStringExtensions
    {
        // Lookup table.
        private static string[] _cache = Enumerable.Range(0, 1000).Select(i => i.ToString(CultureInfo.InvariantCulture))
            .ToArray();
        

        // Lookup table last index.
        private static int _top = _cache.Length;

        public static string ToStringLookup(this int value)
        {
            // See if the integer is in range of the lookup table.
            // ... If it is present, return the string literal element.
            if (value >= 0 &&
                value < _top)
            {
                return _cache[value];
            }
            // Fall back to ToString method.
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }


}