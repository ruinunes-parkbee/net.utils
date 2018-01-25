using System;
using System.Runtime.Caching;

namespace CacheIt
{
    public interface ICache
    {
        void Insert<T>(string key, T value, TimeSpan expiration);
        T Get<T>(string key);
        void Clear(string key);
    }

    public class MemoryCache : ICache
    {
        private readonly System.Runtime.Caching.MemoryCache _cache;

        public MemoryCache(string cacheName)
        {
            _cache = new System.Runtime.Caching.MemoryCache(cacheName);
        }

        public void Insert<T>(string key, T value, TimeSpan expiration)
        {
            var cacheItemPolicy = new CacheItemPolicy
                                  {
                                      AbsoluteExpiration = DateTime.Now + expiration
                                  };
            _cache.Add(key, value, cacheItemPolicy);
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        public void Clear(string key)
        {
            if (_cache.Contains(key))
            {
                _cache.Remove(key);
            }
        }
    }
}
