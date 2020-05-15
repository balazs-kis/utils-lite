using System;

namespace UtilsLite.Tests.Pooling
{
    internal class PoolItem
    {
        public int Number { get; set; }
        public string Text { get; set; }

        public DateTime CreationTime { get; }

        public PoolItem(int number, string text)
        {
            Number = number;
            Text = text;

            CreationTime = DateTime.UtcNow;
        }
    }
}