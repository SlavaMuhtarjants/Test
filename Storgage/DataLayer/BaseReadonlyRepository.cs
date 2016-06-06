namespace Weezlabs.Storgage.DataLayer
{
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Base class for readonly repository.
    /// </summary>
    /// <typeparam name="TItem">Type of enitities</typeparam>
    /// <typeparam name="TKey">Type of key.</typeparam>
    public abstract class BaseReadonlyRepository<TItem, TKey> : IReadonlyRepository<TItem, TKey> where TItem: class
    { 
        /// <summary>
        /// Database context.
        /// </summary>
        protected DbContext Context { get { return context; } }
        private readonly DbContext context;

        /// <summary>
        /// Database set.
        /// </summary>
        protected DbSet<TItem> DbSet { get { return dbSet; } }
        private readonly DbSet<TItem> dbSet;

        /// <summary>
        /// Creates instance of readonly repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        protected BaseReadonlyRepository(DbContext context)
            : this(context, null)
        {            
        }

        /// <summary>
        /// Creates instance of readonly repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="dbSet">Database set.</param>
        protected BaseReadonlyRepository(DbContext context, DbSet<TItem> dbSet)
        {
            Contract.Requires(context != null);
            this.context = context;
            if (dbSet != null)
            {
                this.dbSet = dbSet;
            }
            else
            {
                this.dbSet = context.Set<TItem>();
            }
        }

        /// <summary>
        /// Returns all entities.
        /// </summary>
        /// <returns>Queryable object with all entities.</returns>
        public IQueryable<TItem> GetAll()
        {
            return DoGetAll();
        }

        /// <summary>
        /// Returns all entities. Method for overriding.
        /// </summary>
        /// <returns>Queryable object with all entities.</returns>
        protected virtual IQueryable<TItem> DoGetAll()
        {
            return DbSet;
        }

        /// <summary>
        /// Returns one entity by its key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Item or key.</returns>
        public TItem GetById(TKey id)
        {
            return DoGetById(id);
        }

        /// <summary>
        /// Returns one entity by its key. Method for overriding.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Item or key.</returns>
        protected virtual TItem DoGetById(TKey id)
        {
            return DbSet.Find(id);
        }
    }
}
