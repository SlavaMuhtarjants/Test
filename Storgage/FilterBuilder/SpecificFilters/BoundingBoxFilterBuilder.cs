namespace Weezlabs.Storgage.FilterBuilder.SpecificFilters
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using DataTransferObjects.Space;
    using Enums;

    /// <summary>
    /// Filter builder for checking real properties by bounding box
    /// </summary>
    public class BoundingBoxFilterBuilder : BaseSpecificFilter
    {
        /// <summary>
        /// Constructor with initializing filter params
        /// </summary>
        /// <param name="filter">array of filter params</param>
        public BoundingBoxFilterBuilder(Object[] filter)
        {
            Contract.Requires(filter != null);

            if (filter.Count() == 1 && filter.ElementAt(0) is BoundingBox)
            {
                BoundingBox boundingBox = (BoundingBox)filter.ElementAt(0);

                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.BoundingBoxLatitudeProperty,
                    Operation = OperationsEnum.GreaterThanOrEqual,
                    Value = boundingBox.BottomRightPoint.Latitude
                });
                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.BoundingBoxLatitudeProperty,
                    Operation = OperationsEnum.LessThanOrEqual,
                    Value = boundingBox.TopLeftPoint.Latitude
                });
                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.BoundingBoxLongitudeProperty,
                    Operation = OperationsEnum.GreaterThanOrEqual,
                    Value = boundingBox.TopLeftPoint.Longitude
                });
                Parameters.Add(new FilterModel()
                {
                    PropertyName = FilterPreferences.BoundingBoxLongitudeProperty,
                    Operation = OperationsEnum.LessThanOrEqual,
                    Value = boundingBox.BottomRightPoint.Longitude
                });
            }
        }
    }
}