namespace Weezlabs.Storgage.FilterBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for combine expressions
    /// </summary>
    public static class PredicateExtensions
    {
        /// <summary>
        /// Return result of AND operation between two expressions
        /// </summary>
        /// <typeparam name="T">Type of class for binary operation</typeparam>
        /// <param name="left">first expression</param>
        /// <param name="right">second expression</param>
        /// <returns>Result of AND operation for expressions</returns>
        public static Expression<Func<T, Boolean>> And<T>(this Expression<Func<T, Boolean>> left,
            Expression<Func<T, Boolean>> right)
        {
            Contract.Requires(right != null);
            return CombineLambdas(left, right, ExpressionType.AndAlso);
        }

        /// <summary>
        /// Return result of OR operation between two expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">first expression</param>
        /// <param name="right">second expression</param>
        /// <returns>Result of OR operation for expressions</returns>
        public static Expression<Func<T, Boolean>> Or<T>(this Expression<Func<T, Boolean>> left,
            Expression<Func<T, Boolean>> right)
        {
            Contract.Requires(right != null);
            return CombineLambdas(left, right, ExpressionType.OrElse);
        }

        #region private

        /// <summary>
        /// Return new expression combined from two expressions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">first expression</param>
        /// <param name="right">second expression</param>
        /// <param name="expressionType">Type of binary operation</param>
        /// <returns>result of binary expression between two expression</returns>
        private static Expression<Func<T, Boolean>> CombineLambdas<T>(this Expression<Func<T, Boolean>> left,
            Expression<Func<T, Boolean>> right, ExpressionType expressionType)
        {
            //Remove expressions created with Begin<T>()
            if (IsExpressionBodyConstant(left))
                return (right);
            
            ParameterExpression p = left.Parameters[0];
            SubstituteParameterVisitor visitor = new SubstituteParameterVisitor();
            visitor.Sub[right.Parameters[0]] = p;

            Expression body = Expression.MakeBinary(expressionType, left.Body, visitor.Visit(right.Body));

            return Expression.Lambda<Func<T, Boolean>>(body, p);
        }
        
        /// <summary>
        /// Check type of expression body
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left"></param>
        /// <returns>true if expression body is constant</returns>
        private static Boolean IsExpressionBodyConstant<T>(Expression<Func<T, Boolean>> left)
        {
            return left.Body.NodeType == ExpressionType.Constant;
        }
        
        internal class SubstituteParameterVisitor : ExpressionVisitor
        {
            public Dictionary<Expression, Expression> Sub = new Dictionary<Expression, Expression>();

            protected override Expression VisitParameter(ParameterExpression node)
            {
                Expression newValue;
                if (Sub.TryGetValue(node, out newValue))
                {
                    return newValue;
                }

                return node;
            }
        }

        #endregion
    }
}
