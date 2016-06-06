namespace Weezlabs.Storgage.DataLayer
{
    using System.Linq;

    /// <summary>
    /// Intrerface for readonly repository.
    /// </summary>
    /// <typeparam name="TItem">Type of enitities</typeparam>
    /// <typeparam name="TKey">Type of key.</typeparam>
    public interface IReadonlyRepository<TItem, TKey> where TItem : class
    {
        /// <summary>
        /// Returns one item by its identifier.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Item or null.</returns>
        TItem GetById(TKey id);

        /// <summary>
        /// Returns all items.
        /// </summary>
        /// <returns>Queryable of items.</returns>
        IQueryable<TItem> GetAll();
    }
}
