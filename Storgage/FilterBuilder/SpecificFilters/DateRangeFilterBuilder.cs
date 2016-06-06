namespace Weezlabs.Storgage.FilterBuilder.SpecificFilters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    using Enums;

    /// <summary>
    /// Filter builder for checking storgages by available date
    /// </summary>
    public class DateRangeFilterBuilder : BaseSpecificFilter
    {        
        /// <summary>
        /// Constructor with initializing filter params
        /// </summary>
        /// <param name="filter">array of filter params</param>
        public DateRangeFilterBuilder(Object[] filter)
        {
            Contract.Requires(filter != null);

            if (filter[0] is DateTimeOffset)
            {
                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.DateRangeSinceProperty,
                    Operation = OperationsEnum.LessThan,
                    Value = (DateTimeOffset)filter[0]
                });
                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.DateRangeSinceProperty,
                    Operation = OperationsEnum.Equals,
                    Value = null
                });
            }
        }

        /// <summary>
        /// Return builded filter by parameters
        /// </summary>
        /// <typeparam name="T">Type of filtered elements</typeparam>
        /// <returns>expression for filtering data</returns>
        public override Expression<Func<T, Boolean>> BuildFilter<T>()
        {
            return ExpressionBuilder.GetExpression<T>(Parameters,
                BinaryOperationsEnum.OrElse);
        }
    }
}