using System;
using System.Runtime.Caching;

namespace SMO.Cache
{
    public static class CachingProvider
    {
        static ObjectCache cache = MemoryCache.Default;
        public static void AddItem(string key, object value)
        {
            RemoveItem(key);
            CacheItem item = new CacheItem(key, value);
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.MaxValue;
            cache.Add(item, policy);
        }

        public static void RemoveItem(string key)
        {
            if (cache.Contains(key))
            {
                cache.Remove(key);
            }
        }

        public static object GetItem(string key)
        {
            if (cache.Contains(key))
            {
                return cache[key];
            }
            return null;
        }
    }
}
