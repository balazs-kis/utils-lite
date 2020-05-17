using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UtilsLite.Caching
{
    public class MemoryCache : ICacheProvider, IAsyncCacheProvider
    {
        private readonly ConcurrentDictionary<string, object> _cache;

        public MemoryCache()
        {
            _cache = new ConcurrentDictionary<string, object>();
        }

        public T Get<T>(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                throw new KeyNotFoundException(
                    $"The given key '{key}' was not present in the cache");
            }

            if (_cache[key] is CachedItem<T> cached)
            {
                if (cached.HasExpired)
                {
                    throw new ItemExpiredException(key);
                }

                return cached.Item;
            }

            throw new InvalidOperationException(
                $"The given key '{key}' does not hold the given type '{typeof(T)}'");
        }

        public void Set<T>(string key, T value, CachingOptions<T> options = null)
        {
            var item = new CachedItem<T>(value, options);
            _cache[key] = item;
        }

        public void Refresh(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                throw new KeyNotFoundException(
                    $"The given key '{key}' was not present in the cache");
            }

            var item = (IUpdateable)_cache[key];

            item.Refresh();
        }

        public void UpdateExpiration(string key, DateTime expiration)
        {
            if (!_cache.ContainsKey(key))
            {
                throw new KeyNotFoundException(
                    $"The given key '{key}' was not present in the cache");
            }

            var item = (IUpdateable)_cache[key];

            item.UpdateExpiration(expiration);
        }

        public bool Remove(string key)
        {
            return _cache.TryRemove(key, out _);
        }

        public void Clear()
        {
            _cache.Clear();
        }


        public Task<T> GetAsync<T>(string key)
        {
            return Task.FromResult(Get<T>(key));
        }

        public Task SetAsync<T>(string key, T value, CachingOptions<T> options = null)
        {
            Set(key, value, options);
            return Task.FromResult(0);
        }

        public Task RefreshAsync(string key)
        {
            Refresh(key);
            return Task.FromResult(0);
        }

        public Task UpdateExpirationAsync(string key, DateTime expiration)
        {
            UpdateExpiration(key, expiration);
            return Task.FromResult(0);
        }

        public Task<bool> RemoveAsync(string key)
        {
            return Task.FromResult(Remove(key));
        }

        public Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
        }
    }
}