namespace Weezlabs.Storgage.SpaceService
{
    using System;
    using System.Collections.Generic;

    using DataTransferObjects.Space;
    using DataTransferObjects.Filter;

    /// <summary>
    /// Space searcher.
    /// </summary>
    public interface ISpaceSearcher
    {
        /// <summary>
        /// Search spaces by distance.
        /// </summary>
        /// <param name="filter">Income filters.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Searched spaces by distance.</returns>
        IEnumerable<GetSpaceWithDistanceResponse> SearchByDistance(FilterInfo filter, int offset, int limit);

        /// <summary>
        /// Search spaces by bounding box.
        /// </summary>
        /// <param name="filter">Income filters.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Searched spaces by distance.</returns>
        IEnumerable<GetSpaceResponse> SearchByBBox(FilterInfo filter, int offset, int limit);

        /// <summary>
        /// Search spaces.
        /// </summary>
        /// <param name="filter">Income filters.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Searched spaces.</returns>
        IEnumerable<GetSpaceResponse> Search(FilterInfo filter, int offset, int limit);

        /// <summary>
        /// Build a report of space count per each user filter
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <returns>table of matches between user filters and space count</returns>
        IEnumerable<GetSpacesReportByFilters> CalculateSpacesByFilters(Guid userId);

        /// <summary>
        /// Build a report of space count per each user who has a filter
        /// </summary>
        /// <param name="offset">offset to pass</param>
        /// <param name="pageSize">page size</param>
        /// <returns>table of matches between user and space count per all user filters</returns>
        Dictionary<Guid, Int32> CalculateSpacesByUsers(Int32 offset, Int32 pageSize);
    }
}
