namespace Weezlabs.Storgage.DataLayer.Filters
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using Weezlabs.Storgage.Model;

    using EntityFramework.Extensions;

    /// <summary>
    /// Repository for user filters managing
    /// </summary>
    public class FilterRepository : BaseRepository<Filter, Guid>, IFilterReadonlyRepository, IFilterRepository
    {
        /// <summary>
        /// Initialise the repository with the database context
        /// </summary>
        /// <param name="context">Database context.</param>
        public FilterRepository(DbContext context)
            : base(context)
        {
        }

        protected override void InternalDelete(Filter itemToDelete)
        {
            itemToDelete.FilterRootDictionaries.ToList().ForEach
                (frd => { Context.Entry(frd).State = EntityState.Deleted; });

            base.InternalDelete(itemToDelete);
        }

        protected override void InternalDeleteRange(IQueryable<Filter> itemsToDelete)
        {
            itemsToDelete.SelectMany(f => f.FilterRootDictionaries).Delete();
            base.InternalDeleteRange(itemsToDelete);
        }
    }
}