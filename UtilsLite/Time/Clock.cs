using System;

namespace UtilsLite.Time
{
    public class Clock : IClock
    {
        public DateTime GetNow() => DateTime.Now;
        public DateTime GetUtcNow() => DateTime.UtcNow;
    }
}