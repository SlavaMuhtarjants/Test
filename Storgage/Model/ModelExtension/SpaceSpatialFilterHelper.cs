namespace Weezlabs.Storgage.Model.ModelExtension
{
    using System;
    using System.Data.Entity.Spatial;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public static class SpaceSpatialFilterHelper
    {
        /// <summary>
        /// Filters spaces by distance.
        /// </summary>
        /// <param name="spaces">Spaces queryable.</param>
        /// <param name="location">Location.</param>
        /// <param name="distance">Distance in miles.</param>
        /// <returns>Spaces queryable filtered by distance and ordered by distance.</returns>
        public static IQueryable<Space> FilterByDistance(this IQueryable<Space> spaces, String location, Double distance)
        {
            Contract.Requires(spaces != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(location));
            
            var other = DbGeography.PointFromText(location, DbGeography.DefaultCoordinateSystemId);
            return
                spaces.Where(x => x.Location.Distance(other) * Space.MilesInMeter <= distance)
                    .OrderBy(x => x.Location.Distance(other));
        }
    }
}
