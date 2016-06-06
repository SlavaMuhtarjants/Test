namespace Weezlabs.Storgage.FilterBuilder
{
    using System;

    /// <summary>
    /// Preferences for filter builder
    /// </summary>
    public class FilterPreferences
    {
        /// <summary>
        /// Name of property for filtering storgages by access type
        /// </summary>
        public const String AccessTypeProperty = "SpaceAccessTypeId";

        /// <summary>
        /// Name of property for filtering storgages by size type
        /// </summary>
        public const String SizeTypeProperty = "SizeTypeId";

        /// <summary>
        /// Name of property for filtering storgages by space type
        /// </summary>
        public const String SpaceTypeProperty = "SpaceTypeId";

        /// <summary>
        /// Name of latitude property for filtering storgages by bounding box
        /// </summary>
        public const String BoundingBoxLatitudeProperty = "Latitude";

        /// <summary>
        /// Name of longitude property for filtering storgages by bounding box
        /// </summary>
        public const String BoundingBoxLongitudeProperty = "Longitude";

        /// <summary>
        /// Name of property for filtering storgages by available date range (available since)
        /// </summary>
        public const String DateRangeSinceProperty = "AvailableSince";
        
        /// <summary>
        /// Name of property for filtering storgages by distance
        /// </summary>
        public const String DistanceProperty = "DistanceToSpace";

        /// <summary>
        /// Name of property for filtering storgages by rate
        /// </summary>
        public const String RateProperty = "Rate";

        /// <summary>
        /// Name of zip code property.
        /// </summary>
        public const String ZipProperty = "ZipCode";
    }
}
