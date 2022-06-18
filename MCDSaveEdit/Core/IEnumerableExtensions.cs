using System;
using System.Collections.Generic;
using System.Linq;

namespace MCDSaveEdit
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Return a new collection with the given <paramref name="item"/> removed
        /// </summary>
        public static IEnumerable<T> removing<T>(this IEnumerable<T> collection, T item)
        {
            return collection.Except(new[] { item });
        }

        /// <summary>
        /// Return a new collection with the given <paramref name="newValue"/> replacing the given <paramref name="oldValue"/>
        /// </summary>
        public static IEnumerable<T> replacing<T>(this IEnumerable<T> collection, T oldValue, T newValue)
        {
            var list = collection.ToList();
            var index = list.IndexOf(oldValue);
            list.RemoveAt(index);
            list.Insert(index, newValue);
            return list;
        }

        /// <summary>
        /// Return a new collection with the given <paramref name="item"/> added at the end
        /// </summary>
        public static IEnumerable<T> adding<T>(this IEnumerable<T> collection, T item)
        {
            var list = collection.ToList();
            list.Add(item);
            return list;
        }

        /// <summary>
        /// Return a new collection with the given <paramref name="numberToDropFromEnd"/> dropped from the end
        /// </summary>
        public static IEnumerable<T> dropLast<T>(this IEnumerable<T> enumerable, int numberToDropFromEnd)
        {
            var count = enumerable.Count();
            return enumerable.Take(count - numberToDropFromEnd);
        }

        public static IEnumerable<T> deepClone<T>(this IEnumerable<T> enumerable) where T : ICloneable
        {
            return enumerable.Select(element => element.Clone()).Cast<T>();
        }


    }
}
