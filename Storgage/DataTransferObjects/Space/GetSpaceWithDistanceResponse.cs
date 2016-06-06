namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;

    /// <summary>
    /// Contains information about response when we filter spaces by distance from geo point.
    /// </summary>
    public class GetSpaceWithDistanceResponse : GetSpaceResponse
    {
        /// <summary>
        /// Distance to space in miles.
        /// </summary>
        public Double DistanceToSpace { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetSpaceWithDistanceResponse()
        {
        }

        /// <summary>
        /// Creates dto object from model object.
        /// </summary>
        /// <param name="space">Model object.</param>
        /// <param name="distanceToSpace">Distance to space.</param>
        public GetSpaceWithDistanceResponse(Model.Space space, Double distanceToSpace)
            : base(space)
        {
            DistanceToSpace = distanceToSpace;
        }
    }
}