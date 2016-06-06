namespace Weezlabs.Storgage.FilterBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;

    using DataLayer.Dictionaries;
    using DataTransferObjects.Filter;
    using Model.Enums;
    using SpecificFilters;    
    /// <summary>
    /// Clss for build expression by filter
    /// </summary>
    public class FilterBuilder : ICommonFilterBuilder
    {
        private FilterInfo filter;

        private readonly IDictionaryProvider dictionaryProvider;

        /// <summary>
        /// Constructor with initizlizing filter info
        /// </summary>
        /// <param name="filterParam"></param>
        /// <param name="dictionaryProvider"></param>
        public FilterBuilder(FilterInfo filterParam, IDictionaryProvider dictionaryProvider)
        {
            Contract.Requires(filterParam != null);
            Contract.Requires(dictionaryProvider != null);
            this.filter = filterParam;
            this.dictionaryProvider = dictionaryProvider;
        }

        /// <summary>
        /// Set filter
        /// </summary>
        /// <param name="filterParam">Filter object</param>
        public void SetFilter(FilterInfo filterParam)
        {
            Contract.Requires(filterParam != null);
            filter = filterParam;
        }

        /// <summary>
        /// Return uncompiled expression for where section
        /// </summary>
        /// <returns>Expression if need filtering and null if we sholdn't filtering</returns>
        public Expression<Func<T, Boolean>> BuildFilter<T>()
        {
            var expressionsList = new List<Expression<Func<T, Boolean>>>();
            
            // filter by rate
            if (filter.MinRate != null || filter.MaxRate != null)
            {
                var rateRangeParams = new Object[] { filter.MinRate, filter.MaxRate };
                Expression<Func<T, Boolean>> exp = new RateRangeFilterBuilder(rateRangeParams).BuildFilter<T>();
                expressionsList.Add(exp);
            }

            // filter by rent start date
            if (filter.AvailableSince.HasValue)
            {
                Object[] availableSinceParams = new Object[] { filter.AvailableSince };
                Expression<Func<T, Boolean>> exp = new DateRangeFilterBuilder(availableSinceParams).BuildFilter<T>();
                expressionsList.Add(exp);
            }

            // filter by bounding box
            if (filter.BBox != null)
            {
                Object[] boundingBoxParams = new Object[] { filter.BBox };
                Expression<Func<T, Boolean>> exp = new BoundingBoxFilterBuilder(boundingBoxParams).BuildFilter<T>();
                expressionsList.Add(exp);
            }

            // filter by access type
            if (filter.AccessTypes != null && filter.AccessTypes.Any())
            {
                IEnumerable<SpaceAccessType> allAccessTypes =
                    Enum.GetValues(typeof (SpaceAccessType)).Cast<SpaceAccessType>();

                // build expression if filter contains not all available access type values
                if (allAccessTypes.Except(filter.AccessTypes).Any())
                {
                    Object[] accessTypesParams = filter.AccessTypes.Cast<Object>().ToArray();
                    Expression<Func<T, Boolean>> exp = new AccessTypeFilterBuilder(accessTypesParams, dictionaryProvider).BuildFilter<T>();
                    expressionsList.Add(exp);
                }
            }

            // filter by space type
            if (filter.SpaceTypes != null && filter.SpaceTypes.Any())
            {
                IEnumerable<SpaceType> allSpaceTypes =
                    Enum.GetValues(typeof(SpaceType)).Cast<SpaceType>();

                // build expression if filter contains not all available space type values
                if (allSpaceTypes.Except(filter.SpaceTypes).Any())
                {
                    Object[] spaceTypesParams = filter.SpaceTypes.Cast<Object>().ToArray();
                    Expression<Func<T, Boolean>> exp = new SpaceTypeFilterBuilder(spaceTypesParams, dictionaryProvider).BuildFilter<T>();
                    expressionsList.Add(exp);
                }
            }

            // filter by size
            if (filter.Sizes != null && filter.Sizes.Any())
            {
                IEnumerable<SizeType> allSizes =
                    Enum.GetValues(typeof(SizeType)).Cast<SizeType>();

                // build expression if filter contains not all available sizes of storgages
                if (allSizes.Except(filter.Sizes).Any())
                {
                    Object[] sizesParams = filter.Sizes.Cast<Object>().ToArray();
                    Expression<Func<T, Boolean>> exp = new SizeTypeFilterBuilder(sizesParams, dictionaryProvider).BuildFilter<T>();
                    expressionsList.Add(exp);
                }
            }
          
            
            if (expressionsList.Any())
            {
                Expression<Func<T, Boolean>> finalExpression = expressionsList.First();
                expressionsList.RemoveAt(0);

                while (expressionsList.Any())
                {
                    Expression<Func<T, Boolean>> exp1 = expressionsList.First();
                    expressionsList.RemoveAt(0);
                    
                    finalExpression = finalExpression.And(exp1);
                }
                
                return finalExpression;
            }

            return null;
        }
    }
}