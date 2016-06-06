namespace Weezlabs.Storgage.FilterService
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Spatial;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using DataLayer;
    using DataLayer.Dictionaries;  
    using DataLayer.Filters;
    using DataTransferObjects.Filter;
    using DataTransferObjects.Space;
    using Model;   
    using Model.Exceptions;
    using Model.ModelExtension;

    /// <summary>
    /// Provides methods of user space filters management
    /// </summary>
    public class FilterProvider : IFilterProvider
    {
        private readonly IDictionaryProvider dictionaryProvider;
        private readonly IFilterRepository filterRepository;
        private readonly IUnitOfWork unitOfWork;

        public FilterProvider(IFilterRepository filterRepository, 
            IUnitOfWork unitOfWork, IDictionaryProvider dictionaryProvider)
        {
            this.dictionaryProvider = dictionaryProvider;
            this.filterRepository = filterRepository;
            this.unitOfWork = unitOfWork;
        }

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
        public SavedFilterInfo Create(Guid userID,
            IEnumerable<Weezlabs.Storgage.Model.Enums.SpaceAccessType> accessTypes,
            IEnumerable<Weezlabs.Storgage.Model.Enums.SpaceType> types,
            IEnumerable<Weezlabs.Storgage.Model.Enums.SizeType> sizeTypes,
            Decimal? minPrice, Decimal? maxPrice, DateTimeOffset rentStartDate, BoundingBox boundingBox,
            String location, Guid? zipCodeID)
        {
            Contract.Requires(accessTypes != null);
            Contract.Requires(types != null);
            Contract.Requires(sizeTypes != null);
            Contract.Requires(boundingBox != null);
            Contract.Requires(rentStartDate > DateTime.UtcNow);
            Contract.Requires(location != null);

            // obtain identifiers of the specified filter dictionary attributes: type, access type and size type
            //
            IEnumerable<Guid> sizeTypeIDs = dictionaryProvider.SizeTypes.Join(sizeTypes, t => t.ToEnum(), et => et, (t, et) => t.Id);
            IEnumerable<Guid> typeIDs = dictionaryProvider.SpaceTypes.Join(types, t => t.ToEnum(), et => et, (t, et) => t.Id);
            IEnumerable<Guid> accessTypeIDs = dictionaryProvider.SpaceAccessTypes.Join(accessTypes, t => t.ToEnum(), et => et, (t, et) => t.Id);

            DbGeography polygon = DbGeography.PolygonFromText(boundingBox.ToString(), DbGeography.DefaultCoordinateSystemId);
            Filter filter = new Filter
            {
                UserId = userID,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                RentStartDate = rentStartDate,
                BoundingBox = polygon,
                CheckAccessType = accessTypeIDs.Count() > 0,
                CheckSizeType = sizeTypeIDs.Count() > 0,
                CheckType = typeIDs.Count() > 0,
                Location = location,
                ZipCodeId = zipCodeID,
                TopLatitude = boundingBox.TopLeftPoint.Latitude,
                BottomLatitude = boundingBox.BottomRightPoint.Latitude,
                LeftLongitude = boundingBox.TopLeftPoint.Longitude,
                RightLongitude = boundingBox.BottomRightPoint.Longitude
            };
            ICollection<FilterRootDictionary> attributeValues = sizeTypeIDs.Concat(typeIDs).Concat(accessTypeIDs).
                Select(id => new FilterRootDictionary
                {
                    Filter = filter,
                    RootDictionaryId = id
                }).ToArray();
            filter.FilterRootDictionaries = attributeValues;
            filterRepository.Add(filter);

            unitOfWork.CommitChanges();

            filter = filterRepository.GetAll().Where(f => f.Id == filter.Id).AttachIncludes().SingleOrDefault();

            return new SavedFilterInfo(filter);
        }

        /// <summary>
        /// Deletes user space filters from the database
        /// </summary>
        /// <param name="userID">filter owner identifier</param>
        /// <param name="filterIDs">collection of filter identifiers</param>
        public void Delete(Guid userId, IEnumerable<Guid> filterIDs)
        {
            IQueryable<Filter> dbFilters = filterRepository.GetAll().Where(f => filterIDs.Contains(f.Id));
            Filter[] filters = dbFilters.ToArray();

            if (filters.All(f => f.UserId == userId))
            {
                if (filters.Count() > 0)
                {
                    filterRepository.DeleteRange(dbFilters);
                }
            }
            else
            {
                throw new AccessDeniedException();
            }
        }

        /// <summary>
        /// Retrieves user filter
        /// </summary>
        /// <param name="filterID">filter identifier</param>
        /// <param name="userID">user identifier</param>
        /// <returns>instance of SavedFilterInfo</returns>
        public SavedFilterInfo Get(Guid filterID, Guid userID)
        {
            Filter filter = filterRepository.GetAll().Where
                (f => f.Id == filterID).AttachIncludes().Take(1).SingleOrDefault();
            filter = Get(filter, userID, filterID);

            return new SavedFilterInfo(filter);
        }

        private Filter Get(Filter filter, Guid userID, Guid filterID)
        {
            if (filter == null)
            {
                throw new NotFoundException(String.Format(Weezlabs.Storgage.Resources.Messages.FilterNotFound, filterID));
            }

            if (filter.UserId != userID)
            {
                throw new AccessDeniedException();
            }

            return filter;
        }
    }
}