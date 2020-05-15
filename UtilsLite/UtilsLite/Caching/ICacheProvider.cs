using System;

namespace UtilsLite.Caching
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, CachingOptions<T> options = null);
        void Refresh(string key);
        void UpdateExpiration(string key, DateTime expiration);
        bool Remove(string key);
        void Clear();
    }
}