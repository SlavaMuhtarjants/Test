namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;
    using System.Data.Entity.Spatial;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Contains information about point location of space.
    /// </summary>
    public class GeoPoint
    {
        private DbGeography point;

        /// <summary>
        /// Longitude of space.
        /// </summary>
        public Double Latitude { get; set; }

        /// <summary>
        /// Latitude of space.
        /// </summary>
        public Double Longitude { get; set; }

        /// <summary>
        /// Geo point
        /// </summary>
        private DbGeography Point
        {
            get
            {
                if (point != null)
                {
                    return point;
                }
                point =
                    DbGeography.FromText(String.Format("POINT({0} {1})",
                        Longitude.ToString(CultureInfo.InvariantCulture),
                        Latitude.ToString(CultureInfo.InvariantCulture)));
                return point;
            }
        }

        //we can't do this property public
        public DbGeography GetPoint()
        {
            return Point;
        }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GeoPoint()
        {
        }

        /// <summary>
        /// Create Geo point from DbGeometry spatial.
        /// </summary>
        /// <param name="spatial">DbGeography spatial.</param>
        public GeoPoint(DbGeography spatial)
        {
            Contract.Requires(spatial != null);
            point = spatial;            
            Latitude = spatial.Latitude.Value;
            Longitude = spatial.Longitude.Value;
        }

        /// <summary>
        /// Geotoint to wellknown format
        /// </summary>
        /// <returns>Point in wellknown format</returns>
        public override String ToString()
        {
            return Point.WellKnownValue.WellKnownText;
        }
    }
}