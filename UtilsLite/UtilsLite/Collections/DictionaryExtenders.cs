using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UtilsLite.Collections
{
    public static class DictionaryExtenders
    {
        /// <summary>
        /// Turns an enumerable collection into a dictionary, handling duplicate keys
        /// </summary>
        /// <typeparam name="T">The type of the source collection</typeparam>
        /// <typeparam name="TKey">The type of the key</typeparam>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="items">The collection to convert</param>
        /// <param name="keySelector">The key selector function</param>
        /// <param name="valueSelector">The value selector function</param>
        /// <param name="valueUpdater">The value updater function; func(ValueInDictionary, NewValueWithSameKey) => UpdatedValue</param>
        /// <returns>The composed dictionary</returns>
        public static IDictionary<TKey, TValue> ToDictionary<T, TKey, TValue>(
            this IEnumerable<T> items,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector,
            Func<TValue, TValue, TValue> valueUpdater)
        {
            return items
                .GroupBy(keySelector)
                .ToDictionary(
                    g => g.Key,
                    g => g.Aggregate(valueSelector(g.First()),
                        (oldVal, newItem) => valueUpdater(oldVal, valueSelector(newItem))));
        }

        /// <summary>
        /// Adds the elements from the <b>newCollection</b> to the <b>baseCollection</b>. If the same key is present in the <b>baseCollection</b>,
        /// the value is overwritten with the value from the <b>newCollection</b>. Returns the reference to the <b>baseCollection</b>.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys</typeparam>
        /// <typeparam name="TValue">The type of the values</typeparam>
        /// <param name="baseCollection">The collection to extend</param>
        /// <param name="newCollection">The items to extend the <b>baseCollection</b> with</param>
        /// <returns>A reference to the <b>baseCollection</b></returns>
        public static IDictionary<TKey, TValue> UpdateWith<TKey, TValue>(
            this IDictionary<TKey, TValue> baseCollection,
            IDictionary<TKey, TValue> newCollection)
        {
            foreach (var kvp in newCollection)
            {
                baseCollection[kvp.Key] = kvp.Value;
            }

            return baseCollection;
        }

        public static IReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<T, TKey, TValue>(
            this IEnumerable<T> items,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector)
        {
            return new ReadOnlyDictionary<TKey, TValue>(items.ToDictionary(keySelector, valueSelector));
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionaryA,
            params IDictionary<TKey, TValue>[] dictionaries)
        {
            var dict = dictionaries.ToList();
            dict.Insert(0, dictionaryA);

            return dictionaries
                .SelectMany(d => d)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public static IDictionary<TKey, TValue> MergeMany<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionaryA,
            params IDictionary<TKey, TValue>[] dictionaries)
        {
            var dict = dictionaries.ToList();
            dict.Insert(0, dictionaryA);

            return dict
                .SelectMany(d => d)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.First());
        }

        public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            IDictionary<TKey, TValue> keyValuePairs)
        {
            return AddRange(dictionary, keyValuePairs.ToArray());
        }

        public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            params KeyValuePair<TKey, TValue>[] keyValuePairs)
        {
            keyValuePairs.ForEach(dictionary.Add);
            return dictionary;
        }

        public static TOut SafeGet<TKey, TValue, TOut>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue, TOut> converter = null,
            TOut defaultOut = default)
        {
            if (converter == null)
            {
                converter = v => (TOut)(object)v;
            }

            return dictionary.ContainsKey(key) ? converter(dictionary[key]) : defaultOut;
        }

        public static TOut SafeGet<TKey, TOut>(
            this IDictionary<TKey, TOut> dictionary,
            TKey key)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : default;
        }
    }
}