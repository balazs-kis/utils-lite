using System;
using System.Threading.Tasks;

namespace UtilsLite.Caching
{
    public interface IAsyncCacheProvider
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, CachingOptions<T> options = null);
        Task RefreshAsync(string key);
        Task UpdateExpirationAsync(string key, DateTime expiration);
        Task<bool> RemoveAsync(string key);
        Task ClearAsync();
    }
}