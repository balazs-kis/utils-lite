using System;

namespace UtilsLite.Time
{
    public interface IClock
    {
        DateTime GetNow();
        DateTime GetUtcNow();
    }
}