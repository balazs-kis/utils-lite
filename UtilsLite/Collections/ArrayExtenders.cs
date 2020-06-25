using System;

namespace UtilsLite.Collections
{
    public static class ArrayExtenders
    {
        public static void ForEach<T>(this T[] arr, Action<T> action)
        {
            foreach (var i in arr)
            {
                action.Invoke(i);
            }
        }
    }
}