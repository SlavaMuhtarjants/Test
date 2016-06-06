namespace Weezlabs.Storgage.FilterBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Enums;
    
    /// <summary>
    /// Class for building lambdas
    /// </summary>
    public class ExpressionBuilder
    {
        private static readonly MethodInfo containsMethod = typeof(String).GetMethod("Contains");

        private static readonly MethodInfo startsWithMethod =
            typeof (String).GetMethod("StartsWith", new Type[] {typeof (String)});

        private static readonly MethodInfo endsWithMethod =
            typeof (String).GetMethod("EndsWith", new Type[] {typeof (String)});
        
        /// <summary>
        /// Return expression with all filters 
        /// </summary>
        /// <typeparam name="T">Name of filtered class</typeparam>
        /// <param name="filters">List of filters</param>
        /// <param name="binaryType">Type of binary operations for all filters</param>
        /// <returns>Expression with filters</returns>
        public static Expression<Func<T,Boolean>> GetExpression<T>(IList<FilterModel> filters, BinaryOperationsEnum binaryType)
        {
            if (filters.Count == 0)
            {
                return null;
            }

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
            {
                exp = GetExpression<T>(param, filters[0]);
            }
            else if (filters.Count == 2)
            {
                exp = GetExpression<T>(param, filters[0], filters[1], binaryType);
            }
            else
            {
                while (filters.Count > 0)
                {
                    FilterModel f1 = filters[0];
                    FilterModel f2 = filters[1];

                    exp = exp == null
                        ? GetExpression<T>(param, filters[0], filters[1], binaryType)
                        : (binaryType == BinaryOperationsEnum.OrElse
                            ? Expression.OrElse(exp,
                                GetExpression<T>(param, filters[0], filters[1], binaryType))
                            : Expression.AndAlso(exp,
                                GetExpression<T>(param, filters[0], filters[1])));

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = binaryType == BinaryOperationsEnum.OrElse
                            ? Expression.OrElse(exp, GetExpression<T>(param, filters[0]))
                            : Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, Boolean>>(exp, param);
        }

        /// <summary>
        /// Check type for Nullable
        /// </summary>
        /// <param name="t">Type for check</param>
        /// <returns>True if T is nullable type</returns>
        static Boolean IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Return expression with filter
        /// </summary>
        /// <typeparam name="T">Filtered type</typeparam>
        /// <param name="param">Expression param (left expression)</param>
        /// <param name="filter">Filter</param>
        /// <returns>Expression with filter</returns>
        private static Expression GetExpression<T>(ParameterExpression param, FilterModel filter)
        {
            Expression member = Expression.Property(param, filter.PropertyName);
            Expression constant = Expression.Constant(filter.Value);
            
            if (IsNullableType(member.Type) && !IsNullableType(constant.Type))
            {
                constant = Expression.Convert(constant, member.Type);
            }else if (!IsNullableType(member.Type) && IsNullableType(constant.Type))
            {
                member = Expression.Convert(member, constant.Type);
            }

            switch (filter.Operation)
            {
                case OperationsEnum.Equals:
                    return Expression.Equal(member, constant);

                case OperationsEnum.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case OperationsEnum.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case OperationsEnum.LessThan:
                    return Expression.LessThan(member, constant);

                case OperationsEnum.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);

                case OperationsEnum.Contains:
                    return Expression.Call(member, containsMethod, constant);

                case OperationsEnum.StartsWith:
                    return Expression.Call(member, startsWithMethod, constant);

                case OperationsEnum.EndsWith:
                    return Expression.Call(member, endsWithMethod, constant);
            }

            return null;
        }

        /// <summary>
        /// Get result of binary expression between two expressions 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param">Expression param (left expression)</param>
        /// <param name="filter1">First filter</param>
        /// <param name="filter2">Second filter</param>
        /// <param name="binaryType">Type of binary operation</param>
        /// <returns>Result of binary expression</returns>
        private static BinaryExpression GetExpression<T>
        (ParameterExpression param, FilterModel filter1, FilterModel filter2, BinaryOperationsEnum binaryType = BinaryOperationsEnum.AndElse)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            return binaryType == BinaryOperationsEnum.OrElse
                ? Expression.OrElse(bin1, bin2)
                : Expression.AndAlso(bin1, bin2);
        }
    }
}
