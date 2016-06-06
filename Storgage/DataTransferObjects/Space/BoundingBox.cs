namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    /// <summary>
    /// Contains data avout bounding box.
    /// </summary>
    public class BoundingBox
    {
        [Required]
        /// <summary>
        /// Top left geo point of bbox.
        /// </summary>
        public GeoPoint TopLeftPoint { get; set; }

        [Required]
        /// <summary>
        /// Bottom right geo point of bbox.
        /// </summary>
        public GeoPoint BottomRightPoint { get; set; }

        /// <summary>
        /// Returns bounding box in WKT.
        /// </summary>
        /// <returns>Bounding box in WKT.</returns>
        public override string ToString()
        {
            return string.Format("POLYGON(({0} {1}, {0} {3}, {2} {3}, {2} {1}, {0} {1}))",
                TopLeftPoint.Longitude.ToString(CultureInfo.InvariantCulture),
                TopLeftPoint.Latitude.ToString(CultureInfo.InvariantCulture),
                BottomRightPoint.Longitude.ToString(CultureInfo.InvariantCulture),
                BottomRightPoint.Latitude.ToString(CultureInfo.InvariantCulture));
        }
    }    
}