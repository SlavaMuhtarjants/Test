using System;
using System.Collections.Generic;

namespace Weezlabs.Storgage.Caching
{
    /// <summary>
    /// Provides caching functionality for collections
    /// </summary>
    public interface ICollectionCacheProvider
    {
        /// <summary>
        /// Instance of generic cache provider
        /// </summary>
        ICacheProvider CacheProvider { get; }

        /// <summary>
        /// Retrieve objects collection of the specified type
        /// </summary>
        /// <typeparam name="T">type of objects in collection</typeparam>
        /// <param name="key">key by which to retrieve the objects</param>
        /// <returns>objects collection of the specified type</returns>
        IEnumerable<T> GetAll<T>(String key);
    }
}