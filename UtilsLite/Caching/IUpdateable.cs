using System;

namespace UtilsLite.Caching
{
    internal interface IUpdateable
    {
        void Refresh();
        void UpdateExpiration(DateTime expiration);
    }
}