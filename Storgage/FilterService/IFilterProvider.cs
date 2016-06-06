namespace Weezlabs.Storgage.FilterService
{
    using System;
    using System.Collections.Generic;

    using DataTransferObjects.Filter;
    using DataTransferObjects.Space;
    using Model.Enums;

    /// <summary>
    /// Provides methods of user space filters management
    /// </summary>
    public interface IFilterProvider
    {
        /// <summary>
        /// Deletes user space filters from the database
        /// </summary>
        /// <param name="userID">filter owner identifier</param>
        /// <param name="filterIDs">collection of filter identifiers</param>
        void Delete(Guid userId, IEnumerable<Guid> filterIDs);

        /// <summary>
        /// Creates new user filter
        /// </summary>
        /// <param name="userID">user identifier</param>
        /// <param name="accessTypes">collection of access types</param>
        /// <param name="types">collection of space types</param>
        /// <param name="sizeTypes">collection of size types</param>
        /// <param name="minPrice">minimum price</param>
        /// <param name="maxPrice">maximum price</param>
        /// <param name="rentStartDate">date since that to search spaces for lease</param>
        /// <param name="boundingBox">right-angle area of the top left and bottom right georgraphic coordinates box</param>
        /// <param name="location">location (city, state)</param>
        /// <param name="zipCodeID">zip code identifier</param>
        /// <returns>Instance of SavedFilterInfo</returns>
        SavedFilterInfo Create(Guid userID, IEnumerable<SpaceAccessType> accessTypes, 
            IEnumerable<SpaceType> types, IEnumerable<SizeType> sizeTypes, 
            Decimal? minPrice, Decimal? maxPrice, DateTimeOffset rentStartDate, BoundingBox boundingBox,
            String location, Guid? zipCodeID);

        /// <summary>
        /// Retrieves user filter
        /// </summary>
        /// <param name="filterID">filter identifier</param>
        /// <param name="userID">user identifier</param>
        /// <returns>instance of SavedFilterInfo</returns>
        SavedFilterInfo Get(Guid filterID, Guid userID);
    }
}