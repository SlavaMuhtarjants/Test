namespace Weezlabs.Storgage.DataLayer.Filters
{
    using System;
    using System.Data.Entity;

    using Weezlabs.Storgage.Model;

    /// <summary>
    /// Repository for user filters managing
    /// </summary>
    public class FilterDictionaryRepository : BaseRepository<FilterRootDictionary, Guid>, IFilterDictionaryReadonlyRepository
    {
        /// <summary>
        /// Initialise the repository with the database context
        /// </summary>
        /// <param name="context">Database context.</param>
        public FilterDictionaryRepository(DbContext context)
            : base(context)
        {
        }
    }
}