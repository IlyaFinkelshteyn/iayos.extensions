using System;
using System.Collections.Generic;
using System.Linq;

namespace iayos.extensions
{
	public static class SortedListExtensions
	{
		public static int FindIndexOfKeyGreaterThanOrEqualTo<TKey, TValue>(this SortedList<TKey, TValue> dictionary,
			TKey searchKey, int defaultIfNotFound = -1) where TKey : IComparable<TKey>
		{
			var index = dictionary.Keys.ToList().BinarySearch(searchKey);
			if (index < 0)
			{
				index = ~index;
			}
			return index != dictionary.Count ? index : defaultIfNotFound;
			//throw new IndexOutOfRangeException("Could not find a key greater than " + searchKey);
		}


		public static TKey FindKeyGreaterThanOrEqualTo<TKey, TValue>(this SortedList<TKey, TValue> dictionary, TKey searchKey)
			where TKey : IComparable<TKey>
		{
			var defaultIndexIfNotFound = -1;
			var index = FindIndexOfKeyGreaterThanOrEqualTo(dictionary, searchKey, defaultIndexIfNotFound);
			if (index != defaultIndexIfNotFound)
			{
				return dictionary.Keys[index];
			}
			//if (suppressErrorOnNotFound) return default(TKey);
			throw new IndexOutOfRangeException("Could not find a key greater than or equal to" + searchKey);
		}


		/// <summary>
		///     Finds the lowest index of an item with a key that is equal to or greater than the specified key, then returns the
		///     value at that index.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dictionary"></param>
		/// <param name="searchKey"></param>
		/// <exception cref="IndexOutOfRangeException">Thrown if no index greater than or equal to can be found</exception>
		/// <returns></returns>
		public static TValue GetValueByKeyGreaterThanOrEqualTo<TKey, TValue>(this SortedList<TKey, TValue> dictionary,
			TKey searchKey) where TKey : IComparable<TKey>
		{
			var defaultIndexIfNotFound = -1;
			var index = FindIndexOfKeyGreaterThanOrEqualTo(dictionary, searchKey, defaultIndexIfNotFound);
			if (index != defaultIndexIfNotFound)
			{
				return dictionary.ElementAt(index).Value;
			}
			//if (suppressErrorOnNotFound) return default(TValue);
			throw new IndexOutOfRangeException("Could not find a value based on a key greater than or equal to " + searchKey);
		}
	}
}
