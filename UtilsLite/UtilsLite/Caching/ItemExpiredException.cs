using System;

namespace UtilsLite.Caching
{
    public class ItemExpiredException : Exception
    {
        public ItemExpiredException(string key) : base($"The item for the given key '{key}' has expired")
        {
        }
    }
}