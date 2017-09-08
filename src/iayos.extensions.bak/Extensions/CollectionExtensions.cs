using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iayos.extensions
{

	public static class CollectionExtensions
    {

		/// <summary>
		/// Wraps this object instance into an IList&lt;T&gt;
		/// consisting of a single item. (so we can, for example, use a single method 
		/// to pass in single and multiple items and validate them)
		/// See: http://stackoverflow.com/q/1577822/4413476
		/// </summary>
		/// <typeparam name="T"> Type of the object. </typeparam>
		/// <param name="item"> The instance that will be wrapped. </param>
		/// <returns> An IList&lt;T&gt; consisting of a single item. </returns>
		[DebuggerStepThrough]
		public static List<T> YieldList<T>(this T item)
		{
			var theList = new List<T>();
			if (item != null) theList.Add(item);
			return theList;
		}


		/// <summary>
		/// Insert a new element into a collection if it doesnt already exist based on predicate comparison. Will init the collection if currently null.
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <param name="collection"></param>
		/// <param name="element"></param>
		/// <param name="predicate"></param>
		[DebuggerStepThrough()]
		public static void InsertOrSet<TClass>(this ICollection<TClass> collection, TClass element, Func<TClass, bool> predicate)
		{
			if (collection == null) throw new ArgumentException("InsertOrSet called on null ICollection - must initialize before use");
			var matchingElement = collection.SingleOrDefault(predicate);
			if (matchingElement != null)
			{
				collection.Remove(matchingElement);
				collection.Add(element);
			}
			else
			{
				collection.Add(element);
			}
		}


		/// <summary>
		/// var myObject = myList.ObjectWithMin(x=&gt;x.PropA);
		///These methods basically replace usages like
		///
		///var myObject = myList.OrderBy(x=&gt;x.PropA).FirstOrDefault(); //O(nlog(n)) and unstable
		///
		/// and
		///
		///var myObject = myList.Where(x=&gt;x.PropA == myList.Min(x=&gt;x.PropA)).FirstOrDefault(); //O(N^2) but stable
		///
		/// and
		///
		/// var minValue = myList.Min(x=&gt;x.PropA);
		/// var myObject = myList.Where(x=&gt;x.PropA == minValue).FirstOrDefault(); //not a one-liner, and though linear and stable it's slower (evaluates the enumerable twice)
		/// 
		/// See: http://stackoverflow.com/a/4001342/4413476
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		[DebuggerStepThrough()]
		public static T ObjectWithMin<T, TResult>(this ICollection<T> sequence, Func<T, TResult> predicate)
			where T : class
			where TResult : IComparable
		{
			if (!sequence.Any()) return null;

			//get the first object with its predicate value
			var seed = sequence.Select(x => new { Object = x, Value = predicate(x) }).FirstOrDefault();
			//compare against all others, replacing the accumulator with the lesser value
			//tie goes to first object found
			return
				sequence.Select(x => new { Object = x, Value = predicate(x) })
					.Aggregate(seed, (acc, x) => acc.Value.CompareTo(x.Value) <= 0 ? acc : x).Object;
		}



		/// <summary>
		/// Get the element in a collection with the maximum value on a dynamically specified property
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="sequence"></param>
		/// <param name="predicate"></param>
		/// <returns></returns>
		[DebuggerStepThrough()]
		public static T ObjectWithMax<T, TResult>(this ICollection<T> sequence, Func<T, TResult> predicate)
			where T : class
			where TResult : IComparable
		{
			if (!sequence.Any()) return null;

			//get the first object with its predicate value
			var seed = sequence.Select(x => new { Object = x, Value = predicate(x) }).FirstOrDefault();
			//compare against all others, replacing the accumulator with the greater value
			//tie goes to last object found
			return
				sequence.Select(x => new { Object = x, Value = predicate(x) })
					.Aggregate(seed, (acc, x) => acc.Value.CompareTo(x.Value) > 0 ? acc : x).Object;
		}

	}
}
