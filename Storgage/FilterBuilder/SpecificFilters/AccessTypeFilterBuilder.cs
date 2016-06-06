namespace Weezlabs.Storgage.FilterBuilder.SpecificFilters
{
    using System;
    using System.Linq;   
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    using DataLayer.Dictionaries;
    using Enums;    

    /// <summary>
    /// Filter builder for checking storgages by access type
    /// </summary>
    public class AccessTypeFilterBuilder : BaseSpecificFilter
    {        

        /// <summary>
        /// Constructor with initializing filter params
        /// </summary>
        /// <param name="filter">array of filter params</param>
        public AccessTypeFilterBuilder(Object[] filter, IDictionaryProvider dictionaryProvider)            
        {
            Contract.Requires(filter != null);
            Contract.Requires(dictionaryProvider != null);

            foreach (Object filterParam in filter)
            {
                Object curFilter = filterParam;
                if (curFilter is Model.Enums.SpaceAccessType)
                {
                    Parameters.Add(new FilterModel()
                    {
                        PropertyName = FilterPreferences.AccessTypeProperty,
                        Operation = OperationsEnum.Equals,
                        Value =  dictionaryProvider.SpaceAccessTypes
                            .Single(x => x.Synonym == ((Model.Enums.SpaceAccessType) curFilter).ToString())
                            .Id
                    });
                }
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
