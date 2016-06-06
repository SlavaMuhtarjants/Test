namespace Weezlabs.Storgage.Model
{
    using System;    
    using System.Data.Entity.Spatial;
    using System.Diagnostics.Contracts;

    using Contracts;

    /// <summary>
    /// Space extension.
    /// </summary>
    public partial class Space : IMarkableAsRemoved        
	{
        public const Double MilesInMeter = 0.000621371;

        public Boolean WasRemoved
        {
            get { return this.IsDeleted; }
            set { this.IsDeleted = value; }
        }

        /// <summary>
        /// Calculates distance between 2 points.
        /// </summary>
        /// <param name="spacePoint">Location in well known format</param>
        /// <param name="point">Other point in well known format.</param>
        /// <returns>Distance in miles.</returns>
        public static Double GetDistance(String spacePoint, String point)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(spacePoint));
            Contract.Requires(!String.IsNullOrWhiteSpace(point));

            var other = DbGeography.PointFromText(point, DbGeography.DefaultCoordinateSystemId);
            var location = DbGeography.PointFromText(spacePoint, DbGeography.DefaultCoordinateSystemId);
            var distance = location.Distance(other);

            var result = distance.HasValue ? distance.Value * MilesInMeter : 0.0;
            return result;
        }

        /// <summary>
        /// Checks that user has access to this space.
        /// </summary>
        /// <param name="actorId">User identifier.</param>
        /// <returns>True if user has access.</returns>
        public Boolean HasUserAccess(Guid actorId)
        {
            var result = UserId == actorId;
            return result;
        }
    }
}
