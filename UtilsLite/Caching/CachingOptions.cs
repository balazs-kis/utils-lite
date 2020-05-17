using System;

namespace UtilsLite.Caching
{
    public class CachingOptions<TItem>
    {

        public DateTime? Expiration { get; }
        public TimeSpan? TimeToLive { get; }
        public Action<TItem> OnRemove { get; }

        internal CachingOptions()
        {
            Expiration = null;
            TimeToLive = null;
            OnRemove = null;
        }

        public CachingOptions(DateTime expiration, Action<TItem> removeAction = null)
        {
            Expiration = expiration;
            OnRemove = removeAction;
        }

        public CachingOptions(TimeSpan timeToLive, Action<TItem> removeAction = null)
        {
            TimeToLive = timeToLive;
            Expiration = DateTime.Now + timeToLive;
            OnRemove = removeAction;
        }
    }

    public static class CachingOptions
    {
        public static CachingOptions<T> NoExpiration<T>() => new CachingOptions<T>();
    }
}