using System;
using System.Linq;
using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics
{
    public static class ParameterReplacer
    {

        private static int _ParameterGeneratorLength = typeof(ParameterGenerator<>).GetMethod("Invoke").GetParameters().Length;

        public static Expression<ParameterGenerator<TParamType>> ReduceLambdaParameterGenerator<TParamType>(LambdaExpression expression, IMetaHeuristicContext ctx)
        {
            do
            {
                var lastParam = expression.Parameters.Last();
                var paramDef = ctx.GetParameterDefinition(lastParam.Name);
                if (paramDef == null)
                {
                    throw new ArgumentException($"Expression {expression} can't be reduced because {lastParam.Name} is unknown", nameof(expression));
                }
                if (paramDef is IExpressionGeneratorParameter existingDef)
                {
                    var paramExpression = existingDef.GetExpression(ctx, lastParam.Name);


                    expression = Replace(expression, expression.Parameters.Last(), paramExpression.Body);

                }
                else
                {
                    throw new ArgumentException($"Expression {expression} can't be reduced because {lastParam.Name} wasn't defined as a lambda expression", nameof(expression));
                }

            } while (_ParameterGeneratorLength < expression.Parameters.Count);

            return  expression.CastDelegate<ParameterGenerator<TParamType>>();
        }

        

        public static LambdaExpression Replace(LambdaExpression expression, ParameterExpression source, Expression target)
        {

            return expression.ReplacePar(source.Name, target);

        }

        public static Expression<TOutput> Replace<TOutput>(LambdaExpression expression, ParameterExpression source, Expression target)
        {
            var replaced = Replace(expression, source, target);
           return replaced.CastDelegate<TOutput>();
        }

        public static Expression<TOutput> CastDelegate<TOutput>(this LambdaExpression expression)
        {
            return Expression.Lambda<TOutput>(expression.Body, expression.Parameters);
        }

    }
}