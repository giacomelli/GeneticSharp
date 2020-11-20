using System;
using System.Linq;
using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics
{
    public static class ParameterReplacer
    {


        public static Expression<ParameterGenerator<TParamType>> ReduceLambda<TParamType>(LambdaExpression expression, IMetaHeuristicContext ctx)
        {
            do
            {
                var paramName = expression.Parameters.Last().Name;

                if (ctx.GetParameter(paramName) is IExpressionGeneratorParameter existingDef)
                {
                    var paramExpression = existingDef.GetExpression(ctx);
                    try
                    {

                        switch (expression.Parameters.Count)
                        {
                            case 3:
                                expression = Replace<ParameterGenerator<TParamType>>(expression, expression.Parameters.Last(), paramExpression.Body);
                                //expression = Replace<ParameterGenerator<TParamType>>(expression, expression.Parameters.Last(), paramExpression);

                                break;
                            case 4:
                                expression = Replace<ParameterGenerator<TParamType, double>>(expression, expression.Parameters.Last(), paramExpression.Body);
                                //expression = Replace<ParameterGenerator<TParamType, double>>(expression, expression.Parameters.Last(), paramExpression);
                                break;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    throw new ArgumentException($"Expression {expression} can't be reduced because {paramName} wasn't defined as a lambda expression", nameof(expression));
                }

            } while (typeof(ParameterGenerator<TParamType>).GetMethod("Invoke").GetParameters().Length < expression.Parameters.Count);

            return (Expression<ParameterGenerator<TParamType>>) expression;
        }

        public static Expression<ParameterGenerator<TParamType>> Reduce<TParamType, TArg1>(Expression<ParameterGenerator<TParamType, TArg1>> expression, IMetaHeuristicContext ctx)
        {
            var paramName = expression.Parameters.Last().Name;
            if (ctx.GetParameter(paramName) is ExpressionMetaHeuristicParameter<TParamType> existingDef)
            {
                var paramExpression = existingDef.GetDynamicGenerator(ctx);

                try
                {
                    //return Replace<ParameterGenerator<TParamType, TArg1>, ParameterGenerator<TParamType>>(expression, expression.Parameters.Last(), paramExpression);
                    return Replace<ParameterGenerator<TParamType, TArg1>, ParameterGenerator<TParamType>>(expression, expression.Parameters.Last(), paramExpression.Body);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
            throw new ArgumentException($"Expression {expression} can't be reduced because {paramName} wasn't defined as a lambda expression", nameof(expression));
        }

        public static Expression<TOutput> Replace<TOutput>
        (LambdaExpression expression, string sourceParameterName, Expression target)
        {
            var source = expression.Parameters.Single(p => p.Name == sourceParameterName);
            return new ParameterReplacerVisitor<TOutput>(source, target)
                .VisitAndConvert(expression);
        }


        public static Expression<TOutput> Replace<TInput, TOutput>(Expression<TInput> expression, string sourceParameterName, Expression target)
        {
            var source = expression.Parameters.Single(p => p.Name == sourceParameterName);
            return new ParameterReplacerVisitor<TOutput>(source, target)
                .VisitAndConvert(expression);
        }

        public static Expression<TOutput> Replace<TOutput>(LambdaExpression expression, ParameterExpression source, Expression target)
        {
            return new ParameterReplacerVisitor<TOutput>(source, target)
                .VisitAndConvert(expression);
        }

        // Produces an expression identical to 'expression'
        // except with 'source' parameter replaced with 'target' expression.     
        public static Expression<TOutput> Replace<TInput, TOutput>(Expression<TInput> expression, ParameterExpression source, Expression target)
        {
            return new ParameterReplacerVisitor<TOutput>(source, target)
                .VisitAndConvert(expression);
        }

        private class ParameterReplacerVisitor<TOutput> : ExpressionVisitor
        {
            private ParameterExpression _source;
            private Expression _target;

            public ParameterReplacerVisitor
                (ParameterExpression source, Expression target)
            {
                _source = source;
                _target = target;
            }

            internal Expression<TOutput> VisitAndConvert(LambdaExpression root)
            {
                return (Expression<TOutput>)VisitLambda(root);
            }

            internal Expression<TOutput> VisitAndConvert<T>(Expression<T> root)
            {
                return (Expression<TOutput>)VisitTopLambda(root);
            }


            protected Expression VisitLambda(LambdaExpression node)
            {
                // Leave all parameters alone except the one we want to replace.
                var parameters = node.Parameters
                    .Where(p => p != _source);

                try
                {
                    return Expression.Lambda<TOutput>(Visit(node.Body), parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                
            }


            protected  Expression VisitTopLambda<T>(Expression<T> node)
            {
                
                // Leave all parameters alone except the one we want to replace.
                var parameters = node.Parameters
                    .Where(p => p != _source);

                try
                {
                    return Expression.Lambda<TOutput>(Visit(node.Body), parameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }

            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                var toReturn = base.VisitLambda(node);
                if (toReturn is LambdaExpression lambda)
                {
                    return Expression.Quote(toReturn);
                }

                return toReturn;

            }

            protected override Expression VisitUnary(UnaryExpression node)
            {
                return base.VisitUnary(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                try
                {
                    // Replace the source with the target, visit other params as usual.
                    return node == _source ? _target : base.VisitParameter(node);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
        }
    }
}