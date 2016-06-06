namespace Weezlabs.Storgage.FilterBuilder.SpecificFilters
{
    using System;
    using System.Diagnostics.Contracts;

    using Enums;

    /// <summary>
    /// Filter builder for checking storgages by rate
    /// </summary>
    public class RateRangeFilterBuilder : BaseSpecificFilter
    {        
        /// <summary>
        /// Constructor with initializing filter params
        /// </summary>
        /// <param name="filter">array of filter params</param>
        public RateRangeFilterBuilder(Object[] filter) 
        {
            Contract.Requires(filter != null);

            if (filter[0] is Decimal)
            {
                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.RateProperty,
                    Operation = OperationsEnum.GreaterThanOrEqual,
                    Value = (Decimal) filter[0]
                });
            }
            if (filter[1] is Decimal)
            {
                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.RateProperty,
                    Operation = OperationsEnum.LessThanOrEqual,
                    Value = (Decimal) filter[1]
                });
            }
        }        
    }
}
