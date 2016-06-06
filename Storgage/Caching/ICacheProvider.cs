using System;

namespace Weezlabs.Storgage.Caching
{
    /// <summary>
    /// Contains typical methods for work with cache storage
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Retrieve object from cache by the key
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="key">key by which to retrieve an object</param>
        /// <returns>instance of type T</returns>
        T Retrieve<T>(String key);

        /// <summary>
        /// Determine if cache contains the specified key
        /// </summary>
        /// <param name="key">key by which to retrieve the object</param>
        /// <returns>boolean value, indicating whether the specified key exists</returns>
        Boolean HasKey(String key);

        /// <summary>
        /// Remove object from the cache
        /// </summary>
        /// <param name="key">key by which to remove the object</param>
        /// <returns>boolean value, which equals to false if the key doesn't exist</returns>
        Boolean Remove(String key);

        /// <summary>
        /// Insert a new object or update the existing object in the cache
        /// </summary>
        /// <param name="key">key by which to retrieve the object</param>
        /// <param name="value">updated value</param>
        void Set(String key, Object value);

        /// <summary>
        /// Insert a new object or update the existing object in the cache
        /// and set the expiration date
        /// </summary>
        /// <param name="key">key by which to retrieve the object</param>
        /// <param name="value">updated value</param>
        /// <param name="timeDuration">time duration of storing in the cache</param>
        void Set(String key, Object value, TimeSpan timeDuration);
    }
}