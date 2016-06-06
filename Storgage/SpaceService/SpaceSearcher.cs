namespace Weezlabs.Storgage.SpaceService
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using DataLayer.Filters;
    using DataLayer.Spaces;
    using DataTransferObjects.Filter;
    using DataTransferObjects.Space;
    using FilterBuilder;
    using Model;
    using Model.ModelExtension;

    /// <summary>
    /// Spaces searcher.
    /// </summary>
    public class SpaceSearcher : ISpaceSearcher
    {
        private readonly IFilterDictionaryReadonlyRepository filterDictionaryRepository;
        private readonly IFilterReadonlyRepository filterRepository;
        private readonly ISpaceReadonlyRepository spaceRepository;
        private readonly ICommonFilterBuilder filterBuilder;

        /// <summary>
        /// Create instance of spaces searcher.
        /// </summary>
        /// <param name="filterRepository">Filter repository</param>
        /// <param name="spaceRepository">Space repository</param>
        /// <param name="filterBuilder">Filter builder</param>
        public SpaceSearcher(ISpaceReadonlyRepository spaceRepository, 
            IFilterDictionaryReadonlyRepository filterDictionaryRepository, 
            ICommonFilterBuilder filterBuilder,
            IFilterReadonlyRepository filterRepository)
        {
            Contract.Requires(filterDictionaryRepository != null);
            Contract.Requires(filterRepository != null);
            Contract.Requires(spaceRepository != null);
            Contract.Requires(filterBuilder != null);

            this.filterDictionaryRepository = filterDictionaryRepository;
            this.filterRepository = filterRepository;
            this.spaceRepository = spaceRepository;
            this.filterBuilder = filterBuilder;
        }

        /// <summary>
        /// Searchers spaces.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Spaces ordered by distance from point.</returns>
        public IEnumerable<GetSpaceWithDistanceResponse> SearchByDistance(FilterInfo filter, Int32 offset, Int32 limit)
        {
            Contract.Requires(filter != null);
            Contract.Requires(filter.MaxDistance.HasValue);

            IQueryable<Space> spaces = GetSpaces(filter)
                .FilterByDistance(filter.Location.ToString(), filter.MaxDistance.Value)
                .Paging(offset, limit);
           
            IEnumerable<GetSpaceWithDistanceResponse> result = spaces
                .ToList()
                .Select(x => new GetSpaceWithDistanceResponse(x, Space.GetDistance(x.Location.WellKnownValue.WellKnownText, filter.Location.ToString())))
                .OrderBy(x => x.DistanceToSpace);
                        
            return result;
        }

        /// <summary>
        /// Searchers spaces by bounding box.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Spaces.</returns>
        public IEnumerable<GetSpaceResponse> SearchByBBox(FilterInfo filter, Int32 offset, Int32 limit)
        {
            Contract.Requires(filter != null);
            Contract.Requires(filter.BBox != null);

            IQueryable<Space> spaces = GetSpaces(filter).
                OrderByDescending(x => x.Rate).ThenBy(x => x.Id).Paging(offset, limit);
            IEnumerable<GetSpaceResponse> result = spaces.ToList().Select(x => new GetSpaceResponse(x));

            return result;
        }

        /// <summary>
        /// Searchers spaces.
        /// </summary>
        /// <param name="filter">Filter.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Spaces.</returns>
        public IEnumerable<GetSpaceResponse> Search(FilterInfo filter, Int32 offset, Int32 limit)
        {
            Contract.Requires(filter != null);

            IQueryable<Space> spaces = GetSpaces(filter).Paging(offset, limit);
            IEnumerable<GetSpaceResponse> result = spaces.ToList().Select(x => new GetSpaceResponse(x)).ToList();

            return result;
        }

        /// <summary>
        /// Build a report of space count per each user filter
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <returns>table of matches between user filters and space count</returns>
        public IEnumerable<GetSpacesReportByFilters> CalculateSpacesByFilters(Guid userId)
        {
            IQueryable<Filter> filtersQuery = filterRepository.GetAll().Where(f => f.UserId == userId);
            IEnumerable<SavedFilterInfo> filters = filtersQuery.AttachIncludes().ToArray().
                Select(f => new SavedFilterInfo(f)).ToArray();

            IQueryable<FilterSpaceQuery> filterDictionaryQuery = GetFilterSpaceQuery(filtersQuery);
            var report = filterDictionaryQuery.Select(obj => new
                {
                    FilterId = obj.Filter.Id,
                    SpaceId = obj.Space.Id
                }).Distinct().GroupBy(obj => obj.FilterId, obj => obj.SpaceId).
                Select(gr => new
                {
                    Key = gr.Key,
                    Count = gr.Count()
                }).ToDictionary(obj => obj.Key, obj => obj.Count);

            return filters.Select(f => new GetSpacesReportByFilters
            {
                Filter = f,
                SpacesCount = report.ContainsKey(f.Id) ? report[f.Id] : 0
            });
        }

        /// <summary>
        /// Build a report of space count per each user who has a filter
        /// </summary>
        /// <param name="offset">offset to pass</param>
        /// <param name="pageSize">page size</param>
        /// <returns>table of matches between user and space count per all user filters</returns>
        public Dictionary<Guid, Int32> CalculateSpacesByUsers(Int32 offset, Int32 pageSize)
        {
            DateTime lastDay = DateTime.UtcNow.AddDays(-1);
            IQueryable<Guid> userIDs = filterRepository.GetAll().Select(f => f.UserId).Distinct().OrderBy(id => id).Paging(offset, pageSize);
            IQueryable<Filter> filtersQuery = filterRepository.GetAll().Join(userIDs,
                f => f.UserId, id => id, (f, userId) => f);
            IQueryable<FilterSpaceQuery> filterDictionaryQuery = GetFilterSpaceQuery(filtersQuery, s => s.LastModified > lastDay);

            Dictionary<Guid, Int32> report = filterDictionaryQuery.Select(obj => new
                {
                    UserId = obj.Filter.UserId,
                    SpaceId = obj.Space.Id
                }).Distinct().GroupBy(obj => obj.UserId, obj => obj.SpaceId).Select(gr => new
                {
                    Key = gr.Key,
                    Count = gr.Count()
                }).ToDictionary(obj => obj.Key, obj => obj.Count);

            return report;
        }

        private IQueryable<FilterSpaceQuery> GetFilterSpaceQuery
            (IQueryable<Filter> filtersQuery, Expression<Func<Space, Boolean>> predicate = null)
        {
            predicate = predicate ?? (s => true);
            // Make 3 Left Join between Space, Filter and FilterRootDictionary tables to
            // take into accout 3 relationships: size types, access types, types
            //
            var filterDictionaryQuery = spaceRepository.GetAll().Where(predicate).Join(filtersQuery, s => true, f => true, (s, f) => new
            {
                Space = s,
                Filter = f
            }).GroupJoin(filterDictionaryRepository.GetAll(), fs => new
            {
                FilterId = fs.Filter.Id,
                AccessTypeID = fs.Space.SpaceAccessTypeId
            }, frd => new
            {
                FilterId = frd.FilterId,
                AccessTypeID = frd.RootDictionaryId
            }, (fs, frdCollection) => new
            {
                Space = fs.Space,
                Filter = fs.Filter,
                Dictionaries = frdCollection
            }).SelectMany(obj => obj.Dictionaries.DefaultIfEmpty(), (obj, d) => new
            {
                Space = obj.Space,
                Filter = obj.Filter,
                AccessTypeDictionary = d
            }).GroupJoin(filterDictionaryRepository.GetAll(), fs => new
            {
                FilterId = fs.Filter.Id,
                SizeTypeId = fs.Space.SizeTypeId
            }, frd => new
            {
                FilterId = frd.FilterId,
                SizeTypeId = frd.RootDictionaryId
            }, (fs, frdCollection) => new
            {
                Space = fs.Space,
                Filter = fs.Filter,
                AccessTypeDictionary = fs.AccessTypeDictionary,
                Dictionaries = frdCollection
            }).SelectMany(obj => obj.Dictionaries.DefaultIfEmpty(), (obj, d) => new
            {
                Space = obj.Space,
                Filter = obj.Filter,
                AccessTypeDictionary = obj.AccessTypeDictionary,
                SizeTypeDictionary = d
            }).GroupJoin(filterDictionaryRepository.GetAll(), fs => new
            {
                FilterId = fs.Filter.Id,
                TypeId = fs.Space.SpaceTypeId
            }, frd => new
            {
                FilterId = frd.FilterId,
                TypeId = frd.RootDictionaryId
            }, (fs, frdCollection) => new
            {
                Space = fs.Space,
                Filter = fs.Filter,
                AccessTypeDictionary = fs.AccessTypeDictionary,
                SizeTypeDictionary = fs.SizeTypeDictionary,
                Dictionaries = frdCollection
            }).SelectMany(obj => obj.Dictionaries.DefaultIfEmpty(), (obj, d) => new
            {
                Space = obj.Space,
                Filter = obj.Filter,
                AccessTypeDictionary = obj.AccessTypeDictionary,
                SizeTypeDictionary = obj.SizeTypeDictionary,
                TypeDictionary = d
            });

            // Search spaces by price range condition, geo coordinates inclusion and AvailableSince conditions
            //
            IQueryable<FilterSpaceQuery> filterSpaceQuery = filterDictionaryQuery.Where
                (obj => !obj.Space.IsDeleted && obj.Space.IsListed &&
                (obj.Space.Latitude >= obj.Filter.BottomLatitude && obj.Space.Latitude <= obj.Filter.TopLatitude &&
                    obj.Space.Longitude >= obj.Filter.LeftLongitude && obj.Space.Longitude <= obj.Filter.RightLongitude) &&
                (!obj.Space.AvailableSince.HasValue || obj.Space.AvailableSince < obj.Filter.RentStartDate) &&
                (!obj.Filter.MaxPrice.HasValue || obj.Space.Rate <= obj.Filter.MaxPrice) &&
                    (!obj.Filter.MinPrice.HasValue || obj.Space.Rate >= obj.Filter.MinPrice) &&
                (!obj.Filter.CheckType || obj.Space.SpaceTypeId == obj.TypeDictionary.RootDictionaryId) &&
                (!obj.Filter.CheckSizeType || obj.Space.SizeTypeId == obj.SizeTypeDictionary.RootDictionaryId) &&
                (!obj.Filter.CheckAccessType || obj.Space.SpaceAccessTypeId == obj.AccessTypeDictionary.RootDictionaryId)).
            Select(obj => new FilterSpaceQuery
            {
                Filter = obj.Filter,
                Space = obj.Space
            });

            return filterSpaceQuery;
        }

        private IQueryable<Space> GetSpaces(FilterInfo filter)
        {
            IQueryable<Space> spaces = spaceRepository.GetAll().
                Where(x => x.IsListed && !x.IsDeleted).AttachIncludes().OrderBy(x => x.Rate);

            filterBuilder.SetFilter(filter);
            Expression expression = filterBuilder.BuildFilter<Space>();
            
            if (expression != null)
            {
                spaces = spaces.Where((Expression<Func<Space, Boolean>>)expression);
            }


            return spaces;
        }
    }
}
