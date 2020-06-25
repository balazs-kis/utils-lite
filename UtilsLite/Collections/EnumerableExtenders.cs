using System;
using System.Collections.Generic;
using System.Linq;

namespace UtilsLite.Collections
{
    public static class EnumerableExtenders
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                return;
            }
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static TItem MaxBy<TItem, TQuantity>(this IEnumerable<TItem> collection, Func<TItem, TQuantity> selector)
        {
            var c = collection.ToArray();

            var max = c.Select(selector).Max();
            return c.FirstOrDefault(i => selector(i).Equals(max));
        }

        public static TItem MaxByOrDefault<TItem, TQuantity>(this IEnumerable<TItem> collection, Func<TItem, TQuantity> selector)
        {
            var c = collection.ToArray();

            if (!c.Any())
            {
                return default;
            }

            var max = c.Select(selector).Max();
            return c.FirstOrDefault(i => selector(i).Equals(max));
        }

        public static TItem MinBy<TItem, TQuantity>(this IEnumerable<TItem> collection, Func<TItem, TQuantity> selector)
        {
            var c = collection.ToArray();

            var max = c.Select(selector).Min();
            return c.FirstOrDefault(i => selector(i).Equals(max));
        }

        public static TItem MinByOrDefault<TItem, TQuantity>(this IEnumerable<TItem> collection, Func<TItem, TQuantity> selector)
        {
            var c = collection.ToArray();

            if (!c.Any())
            {
                return default;
            }

            var min = c.Select(selector).Min();
            return c.FirstOrDefault(i => selector(i).Equals(min));
        }
    }
}