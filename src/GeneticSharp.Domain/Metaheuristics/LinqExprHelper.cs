﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GeneticSharp.Domain.Metaheuristics
{
    /// <summary>
    /// Imported from <see href="https://github.com/jarekczek/LinqExprHelper">Github LinqExprHelper library</see>
    /// </summary>
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