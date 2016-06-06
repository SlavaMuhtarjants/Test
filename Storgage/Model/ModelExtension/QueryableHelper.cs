namespace Weezlabs.Storgage.Model.ModelExtension
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Queryable helper.
    /// </summary>
    public static class QueryableHelper
    {
        /// <summary>
        /// Pagings queryable.
        /// </summary>
        /// <typeparam name="T">Type of queryable.</typeparam>
        /// <param name="queryable">Queryable.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns></returns>
        public static IQueryable<T> Paging<T>(this IQueryable<T> queryable, Int32 offset, Int32 limit)
        {
            Contract.Requires(queryable != null);

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(null, Resources.Messages.InvalidOffset);
            }

            if (limit < 0)
            {
                throw new ArgumentOutOfRangeException(null, Resources.Messages.InvalidLimit);
            }

            queryable = (offset > 0) ? queryable.Skip(offset): queryable;
            queryable = (limit > 0) ? queryable.Take(limit) : queryable;

            return queryable;
        }
    }
}
