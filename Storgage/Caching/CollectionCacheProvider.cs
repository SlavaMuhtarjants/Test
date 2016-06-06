using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Weezlabs.Storgage.Caching
{
    /// <summary>
    /// Provides caching functionality for collections
    /// </summary>
    public class CollectionCacheProvider : ICollectionCacheProvider
    {
        public CollectionCacheProvider(ICacheProvider cacheProvider)
        {
            Contract.Requires(cacheProvider != null);

            this.cacheProvider = cacheProvider;
        }

        /// <summary>
        /// Instance of generic cache provider
        /// </summary>
        ICacheProvider ICollectionCacheProvider.CacheProvider
        {
            get
            {
                return cacheProvider;
            }
        }

        /// <summary>
        /// Retrieve objects collection of the specified type
        /// </summary>
        /// <typeparam name="T">type of objects in collection</typeparam>
        /// <param name="key">key by which to retrieve the objects</param>
        /// <returns>objects collection of the specified type</returns>
        public IEnumerable<T> GetAll<T>(String key)
        {
            return ((ICollectionCacheProvider)this).CacheProvider.Retrieve<IEnumerable<T>>(key);
        }

        private readonly ICacheProvider cacheProvider;
    }
}