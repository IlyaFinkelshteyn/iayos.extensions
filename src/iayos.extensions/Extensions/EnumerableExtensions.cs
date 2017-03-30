using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace iayos.extensions
{

	public static class IEnumerableExtensions
	{
		/// <summary>
		/// Wraps this object instance into an IEnumerable&lt;T&gt;
		/// consisting of a single item. (so we can, for example, use a single method 
		/// to pass in single and multiple items and validate them)
		/// See: http://stackoverflow.com/q/1577822/4413476
		/// </summary>
		/// <typeparam name="T"> Type of the object. </typeparam>
		/// <param name="item"> The instance that will be wrapped. </param>
		/// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
		[DebuggerStepThrough]
		public static IEnumerable<T> YieldEnumerable<T>(this T item)
		{
			yield return item;
		}


		/// <summary>
		/// http://stackoverflow.com/a/38364191/4413476
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="batchsize"></param>
		/// <returns></returns>
		public static IList<T[]> GetInChunks<T>(this IEnumerable<T> source, int batchsize = 100)
		{
			IList<T[]> result = null;
			if (source != null && batchsize > 0)
			{
				var list = source as List<T> ?? source.ToList();
				if (list.Count > 0)
				{
					result = new List<T[]>();
					for (var index = 0; index < list.Count; index += batchsize)
					{
						var rangesize = Math.Min(batchsize, list.Count - index);
						result.Add(list.GetRange(index, rangesize).ToArray());
					}
				}
			}
			return result ?? Enumerable.Empty<T[]>().ToList();
		}
	}

}
