namespace Weezlabs.Storgage.DataLayer
{
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Model.Contracts;

    using EntityFramework.Extensions;

    /// <summary>
    /// Base class for repository.
    /// </summary>
    /// <typeparam name="TItem">Type of entity.</typeparam>
    /// <typeparam name="TKey">Type of key.</typeparam>
    public abstract class BaseRepository<TItem, TKey> : BaseReadonlyRepository<TItem, TKey>, IRepository<TItem, TKey> where TItem : class
    {
        /// <summary>
        /// Creates instance of base repository.
        /// </summary>
        /// <param name="context">Database context.</param>
        protected BaseRepository(DbContext context)
            : this(context, null)
        {
        }

        /// <summary>
        /// Creates instance of base repository.
        /// </summary>
        /// <param name="context">Db context.</param>
        /// <param name="dbSet">Db set.</param>
        protected BaseRepository(DbContext context, DbSet<TItem> dbSet)
            : base(context, dbSet)
        { }
        
        /// <summary>
        /// Adds new item to repository.
        /// </summary>
        /// <param name="itemToAdd">New item.</param>
        public void Add(TItem itemToAdd)
        {
            DoAdd(itemToAdd);
        }
        
        /// <summary>
        /// Adds new item to repository. Method for overriding.
        /// </summary>
        /// <param name="itemToAdd">New item.</param>
        protected virtual void DoAdd(TItem itemToAdd)
        {
            var dbEntityEntry = Context.Entry(itemToAdd);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(itemToAdd);
            }
        }

        /// <summary>
        /// Adds items.
        /// </summary>
        /// <param name="itemsToAdd">Items to add.</param>
        public void AddRange(IEnumerable<TItem> itemsToAdd)
        {
            DoAddRange(itemsToAdd);
        }

        /// <summary>
        /// Adds items.
        /// </summary>
        /// <param name="itemsToAdd">Items to add.</param>
        protected virtual void DoAddRange(IEnumerable<TItem> itemsToAdd)
        {
            DbSet.AddRange(itemsToAdd);

        }

        /// <summary>
        /// Updates item in repository.
        /// </summary>
        /// <param name="itemToUpdate">Item to update.</param>
        public void Update(TItem itemToUpdate)
        {
            DoUpdate(itemToUpdate);
        }

        /// <summary>
        /// Updates item in repository. Method for overriding.
        /// </summary>
        /// <param name="itemToUpdate">Item to update.</param>
        protected virtual void DoUpdate(TItem itemToUpdate)
        {
            var dbEntityEntry = Context.Entry(itemToUpdate);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(itemToUpdate);
            }
            dbEntityEntry.State = EntityState.Modified;

        }
        
        /// <summary>
        /// Deletes item from repository by identifier.
        /// </summary>
        /// <param name="itemToDelete">Item to delete.</param>        
        public void Delete(TItem itemToDelete)
        {
            if (itemToDelete is IMarkableAsRemoved)
            {
                var markableAsRemoved = (IMarkableAsRemoved)itemToDelete;
                markableAsRemoved.WasRemoved = true;
                Update(itemToDelete);
            }
            else
            {
                InternalDelete(itemToDelete);
            }          
        }

        public void DeleteRange(IQueryable<TItem> itemsToDelete)
        {
            InternalDeleteRange(itemsToDelete);
        }

        protected virtual void InternalDelete(TItem itemToDelete)
        {
            var dbEntityEntry = Context.Entry(itemToDelete);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(itemToDelete);
                DbSet.Remove(itemToDelete);
            }
        }

        protected virtual void InternalDeleteRange(IQueryable<TItem> itemsToDelete)
        {
            itemsToDelete.Delete();
        }
    }
}
