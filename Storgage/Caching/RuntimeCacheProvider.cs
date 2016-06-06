using System;
using System.Runtime.Caching;

namespace Weezlabs.Storgage.Caching
{
    /// <summary>
    /// Cache provider for .NET application based on System.Runtime.Caching.MemoryCache class
    /// </summary>
    public class RuntimeCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Retrieve object from cache by the key
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="key">key by which to retrieve an object</param>
        /// <returns>instance of type T</returns>
        public T Retrieve<T>(String key)
        {
            Object obj = MemoryCache.Default.Get(key);

            if (obj is T)
            {
                return (T)obj;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Determine if cache contains the specified key
        /// </summary>
        /// <param name="key">key by which to retrieve the object</param>
        /// <returns>boolean value, indicating whether the specified key exists</returns>
        public Boolean HasKey(String key)
        {
            return MemoryCache.Default.Contains(key);
        }

        /// <summary>
        /// Remove object from the cache
        /// </summary>
        /// <param name="key">key by which to remove the object</param>
        /// <returns>boolean value, which equals to false if the key doesn't exist</returns>
        public Boolean Remove(String key)
        {
            if (HasKey(key))
            {
                MemoryCache.Default.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Insert a new object or update the existing object in the cache
        /// </summary>
        /// <param name="key">key by which to retrieve the object</param>
        /// <param name="value">updated value</param>
        public void Set(String key, Object value)
        {
            Set(key, value, _defaultTimeDurationInCache);
        }

        /// <summary>
        /// Insert a new object or update the existing object in the cache
        /// </summary>
        /// <param name="key">key by which to retrieve the object</param>
        /// <param name="value">updated value</param>
        /// <param name="timeDuration">time duration of storing in the cache</param>
        public void Set(String key, Object value, TimeSpan timeDuration)
        {
            MemoryCache.Default.Set(key, value, DateTime.UtcNow.Add(timeDuration));
        }

        private TimeSpan _defaultTimeDurationInCache = new TimeSpan(1, 0, 0);   // 1 hour
    }
}