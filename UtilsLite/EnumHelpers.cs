using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UtilsLite
{
    public static class StringToEnumExtender
    {
        public static T ToEnum<T>(this string value) => (T)Enum.Parse(typeof(T), value);
    }

    public static class IntToEnumExtender
    {
        public static T ToEnum<T>(this int value) => (T)Enum.ToObject(typeof(T), value);
    }

    public static class EnumExtenders
    {
        public static IEnumerable<T> GetAllEnumValues<T>() => Enum.GetValues(typeof(T)).OfType<T>();
    }
}
