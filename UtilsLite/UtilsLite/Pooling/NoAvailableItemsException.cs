using System;

namespace UtilsLite.Pooling
{
    public class NoAvailableItemsException : Exception
    {
        public NoAvailableItemsException()
            : base("There are no available items in the pool and the maximum pool size was reached.")
        {
        }
    }
}