namespace Weezlabs.Storgage.FilterBuilder.SpecificFilters
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Enums;

    public abstract class BaseSpecificFilter : IFilterBuilder
    {
        /// <summary>
        /// List of parameters for filter
        /// </summary>
        private readonly List<FilterModel> parameters = new List<FilterModel>();
        protected List<FilterModel> Parameters { get { return parameters; } }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected BaseSpecificFilter()
        {
        }

        /// <summary>
        /// Return builded filter by parameters
        /// </summary>
        /// <typeparam name="T">Type of filtered elements</typeparam>
        /// <returns>expression for filtering data</returns>
        public virtual Expression<Func<T, Boolean>> BuildFilter<T>()
        {
            return ExpressionBuilder.GetExpression<T>(parameters,
                BinaryOperationsEnum.AndElse);
        }        
    }
}
