using System;
using System.Collections.Generic;
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
                    try
                    {
                       
                        expression = Replace(expression, expression.Parameters.Last(), paramExpression.Body);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    throw new ArgumentException($"Expression {expression} can't be reduced because {lastParam.Name} wasn't defined as a lambda expression", nameof(expression));
                }

            } while (_ParameterGeneratorLength < expression.Parameters.Count);

            return  expression.CastDelegate<ParameterGenerator<TParamType>>();
        }


        
        //public static Expression<ParameterGenerator<TParamType>> Reduce<TParamType, TArg1>(Expression<ParameterGenerator<TParamType, TArg1>> expression, IMetaHeuristicContext ctx)
        //{
        //    var paramName = expression.Parameters.Last().Name;
        //    if (ctx.GetParameter(paramName) is ExpressionMetaHeuristicParameter<TParamType> existingDef)
        //    {
        //        var paramExpression = existingDef.GetDynamicGenerator(ctx);

        //        try
        //        {
        //            //return Replace<ParameterGenerator<TParamType, TArg1>, ParameterGenerator<TParamType>>(expression, expression.Parameters.Last(), paramExpression);
        //            return Replace<ParameterGenerator<TParamType>>(expression, expression.Parameters.Last(), paramExpression.Body);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e);
        //            throw;
        //        }

        //    }
        //    throw new ArgumentException($"Expression {expression} can't be reduced because {paramName} wasn't defined as a lambda expression", nameof(expression));
        //}

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


        //public static Expression<TOutput> Replace<TOutput>
        //(LambdaExpression expression, string sourceParameterName, Expression target)
        //{
        //    var source = expression.Parameters.Single(p => p.Name == sourceParameterName);
        //    return new ParameterReplacerVisitor<TOutput>(source, target)
        //        .VisitAndConvert(expression);
        //}


        //public static Expression<TOutput> Replace<TInput, TOutput>(Expression<TInput> expression, string sourceParameterName, Expression target)
        //{
        //    var source = expression.Parameters.Single(p => p.Name == sourceParameterName);
        //    return new ParameterReplacerVisitor<TOutput>(source, target)
        //        .VisitAndConvert(expression);
        //}

        //public static Expression<TOutput> Replace<TOutput>(LambdaExpression expression, ParameterExpression source, Expression target)
        //{
        //    return new ParameterReplacerVisitor<TOutput>(source, target)
        //        .VisitAndConvert(expression);

        //}

        //// Produces an expression identical to 'expression'
        //// except with 'source' parameter replaced with 'target' expression.     
        //public static Expression<TOutput> Replace<TInput, TOutput>(Expression<TInput> expression, ParameterExpression source, Expression target)
        //{
        //    return new ParameterReplacerVisitor<TOutput>(source, target)
        //        .VisitAndConvert(expression);
        //}

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


            private LambdaExpression _ParentNode;

            private Expression VisitLambda(LambdaExpression node)
            {
                // Leave all parameters alone except the one we want to replace.
                var parameters = node.Parameters
                    .Where(p => p != _source);

                try
                {
                    _ParentNode = node;
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

            //protected override Expression VisitLambda<T>(Expression<T> node)
            //{

            //    try
            //    {
            //        if (!_Quoted.Contains(node))
            //        {
            //            _Quoted.Add(node);
            //            var quoted = Expression.Quote(node);
            //            var toReturn = VisitUnary(quoted);
            //            return toReturn;

            //        }
            //        else
            //        {
            //            var toReturn = base.VisitLambda(node);
            //            return toReturn;
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }

                
            //}

            //private HashSet<LambdaExpression> _Quoted = new HashSet<LambdaExpression>();

            //protected override Expression VisitUnary(UnaryExpression node)
            //{
            //    try
            //    {
            //        //return base.VisitUnary(node);
            //        if (node.NodeType == ExpressionType.Quote)
            //        {
            //            _Quoted.Add((LambdaExpression) node.Operand);
            //            //return base.Visit(node.Operand);
            //            var toReturn = base.VisitUnary(node);
            //            //_Quotes.Pop();
            //            return toReturn;
            //        }

            //        return base.VisitUnary(node);
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }
               
            //}

            protected override Expression VisitParameter(ParameterExpression node)
            {
                try
                {
                    // Replace the source with the target, visit other params as usual.
                    if (node == _source)
                    {
                        return _target;
                    }
                    else
                    {
                        //var replaceParam = _ParentNode.Parameters.FirstOrDefault(n => n.Name == node.Name);
                        //if (replaceParam != null)
                        //{
                        //    return base.VisitParameter(replaceParam);
                        //}
                        return base.VisitParameter(node);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
        }


    }



    public static class LinqExprHelper
    {
        private class ReplVisitor : ExpressionVisitor
        {
            protected Expression searchedExpr;
            protected Expression replaceExpr;

            public void PrepareReplace(ParameterExpression src, Expression dst)
            {
                searchedExpr = src;
                replaceExpr = dst;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == searchedExpr)
                    return replaceExpr;
                else
                    return base.VisitParameter(node);
            }
        }

        private class ParNameUnifyingVisitor : ExpressionVisitor
        {
            private Dictionary<string, ParameterExpression> dict;

            public Expression Process(LambdaExpression node)
            {
                dict = new Dictionary<string, ParameterExpression>();
                Expression rv = base.Visit(node);
                dict = null;
                return rv;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                ParameterExpression replPar; ;
                if (dict.TryGetValue(node.Name, out replPar))
                    return replPar;
                else
                {
                    dict[node.Name] = node;
                    return base.VisitParameter(node);
                }
            }

            public override Expression Visit(Expression node)
            {
                if (dict == null)
                    throw new InvalidOperationException("Use Process method instead.");
                return base.Visit(node);
            }
        }

        #region NewExpr methods
        /** Helper methods to force type resolution by C# compiler,
         *  enables use of syntax: var e = NewExpr((int x) => x + 1)
         */
        public static Expression<Func<T, R>> NewExpr<T, R>(Expression<Func<T, R>> expr)
        {
            return expr;
        }
#pragma warning disable CS1591
        public static Expression<Func<T1, T2, R>> NewExpr<T1, T2, R>(Expression<Func<T1, T2, R>> expr)
        {
            return expr;
        }
        public static Expression<Func<T1, T2, T3, R>>
            NewExpr<T1, T2, T3, R>
            (Expression<Func<T1, T2, T3, R>> expr)
        {
            return expr;
        }
#pragma warning disable CS1591
        #endregion

        /** Processes the expression to make parameters identical if they
         *  have the same name, even if they come from different scopes.
         *  The scope of the first found parameter is used.
         *  <para>It's crucial that also parameters of the LambdaExpression
         *  are processed, because they have to be used in the expression body.
         *  However we are not sure, which one of the parameters with the same
         *  name is used. It may happen, that it's a different one than
         *  the original parameter on the left side of the LambdaExpression.
         *  Alternatively the implementation could first process
         *  expr.Parameters and fill the map with them, to give them higher
         *  priority.</para>
         */
        public static LambdaExpression UnifyParametersByName(this LambdaExpression expr)
        {
            var vis = new ParNameUnifyingVisitor();
            return (LambdaExpression)vis.Process(expr);
        }

        /** Replaces a parameter with name <paramref name="parName"/> with the given expression.
         *  Used to simplify syntax of complex expressions, and compose
         *  expressions from reusable inline lambdas.
         *  See <seealso cref="LinqExprHelperTests.CombineExprByName" /> for sample usage.
         */
        public static LambdaExpression ReplacePar(this LambdaExpression expr,
            string parName, Expression replacementExpr)
        {
            var parToRepl = expr.Parameters.Where(p => p.Name.Equals(parName)).First();
            var newPars = expr.Parameters.Where(p => !p.Name.Equals(parName)).ToArray();
            var vis = new ReplVisitor();
            vis.PrepareReplace(parToRepl, replacementExpr);
            var newExprBody = vis.Visit(expr.Body);
            return Expression.Lambda(newExprBody, newPars).UnifyParametersByName();
        }
    }




}