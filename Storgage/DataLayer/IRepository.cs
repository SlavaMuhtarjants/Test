namespace Weezlabs.Storgage.DataLayer
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Interface for repository.
    /// </summary>
    /// <typeparam name="TItem">Type of entity.</typeparam>
    /// <typeparam name="TKey">Type of key.</typeparam>
    public interface IRepository<TItem, TKey> : IReadonlyRepository<TItem, TKey> where TItem : class
    {
        /// <summary>
        /// Adds new entity to repository.
        /// </summary>
        /// <param name="itemToAdd">New entity.</param>       
        void Add(TItem itemToAdd);

        /// <summary>
        /// Adds items.
        /// </summary>
        /// <param name="itemsToAdd">Items to add.</param>
        void AddRange(IEnumerable<TItem> itemsToAdd);

        /// <summary>
        /// Updates new entity in repository.
        /// </summary>
        /// <param name="itemToUpdate">Entity to update.</param>
        void Update(TItem itemToUpdate);        

        /// <summary>
        /// Deletes entity by its key.
        /// </summary>
        /// <param name="itemToDelete">Item to delete.</param>              
        void Delete(TItem itemToDelete);

        /// <summary>
        /// Deletes entities by keys.
        /// </summary>
        /// <param name="itemsToDelete">IQueryable of items to delete.</param>
        void DeleteRange(IQueryable<TItem> itemsToDelete);
    }
}
