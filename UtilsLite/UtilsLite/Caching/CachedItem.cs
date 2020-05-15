using System;

namespace UtilsLite.Caching
{
    internal class CachedItem<T> : IUpdateable
    {
        private CachingOptions<T> _cachingOptions;

        public T Item { get; }
        public bool HasExpired => _cachingOptions.Expiration.HasValue && _cachingOptions.Expiration < DateTime.Now;

        public CachedItem(T item, CachingOptions<T> cachingOptions = null)
        {
            Item = item;
            _cachingOptions = cachingOptions ?? CachingOptions.NoExpiration<T>();
        }

        public void Refresh()
        {
            if (_cachingOptions.TimeToLive != default)
            {
                var newOptions = new CachingOptions<T>(
                    _cachingOptions.TimeToLive.Value,
                    _cachingOptions.OnRemove);

                _cachingOptions = newOptions;
            }
        }

        public void UpdateExpiration(DateTime expiration)
        {
            var newOptions = new CachingOptions<T>(expiration, _cachingOptions.OnRemove);
            _cachingOptions = newOptions;
        }
    }
}