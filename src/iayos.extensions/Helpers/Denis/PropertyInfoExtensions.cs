using System;
using System.Linq.Expressions;
using System.Reflection;

namespace iayos.extensions {
	public static class PropertyInfoExtensions
	{
		/// <summary>
		/// Generates an <see cref="Expression{Func{_,_}}"/> that represents the current <see cref="PropertyInfo"/>'s getter.
		/// </summary>
		public static Expression<Func<TSource, TProperty>> GetGetAccessor<TSource, TProperty>(
			this PropertyInfo propertyInfo, bool includeNonPublic = false)
		{
			var getMethod = propertyInfo.GetGetMethod(includeNonPublic);

			if (getMethod != null && propertyInfo.GetIndexParameters().Length == 0)
			{
				var instance = Expression.Parameter(typeof(TSource), "instance");
				var value = Expression.Call(instance, getMethod);

				return Expression.Lambda<Func<TSource, TProperty>>(
					propertyInfo.PropertyType.IsValueType
						? Expression.Convert(value, typeof(TProperty))
						: Expression.TypeAs(value, typeof(TProperty)),
					instance
				);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Generates a <see cref="Func{_,_}"/> delegate to the current <see cref="PropertyInfo"/>'s getter.
		/// </summary>
		/// <param name="includeNonPublic">Indicates whether a non-public get accessor should be returned.</param>
		public static Func<TSource, object> GetGetAccessor<TSource>(this PropertyInfo propertyInfo,
			bool includeNonPublic = false)
		{
			return propertyInfo.GetGetAccessor<TSource, object>(includeNonPublic)?.Compile();
		}

		/// <summary>
		/// Generates an <see cref="Expression{Action{_,_}};"/> that represents the current <see cref="PropertyInfo"/>'s setter.
		/// </summary>
		/// <param name="includeNonPublic">Indicates whether a non-public set accessor should be returned.</param>
		public static Expression<Action<TSource, TProperty>> GetSetAccessor<TSource, TProperty>(
			this PropertyInfo propertyInfo, bool includeNonPublic = false)
		{
			var setMethod = propertyInfo.GetSetMethod(includeNonPublic);

			if (setMethod != null && propertyInfo.GetIndexParameters().Length == 0)
			{
				var instance = Expression.Parameter(typeof(TSource), "instance");
				var value = Expression.Parameter(typeof(TProperty), "value");

				return Expression.Lambda<Action<TSource, TProperty>>(
					Expression.Call(
						instance,
						setMethod,
						propertyInfo.PropertyType.IsValueType
							? Expression.Convert(value, propertyInfo.PropertyType)
							: Expression.TypeAs(value, propertyInfo.PropertyType)
					),
					instance,
					value
				);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Generates an <see cref="Action{_,_}"/> delegate to the current <see cref="PropertyInfo"/>'s setter.
		/// </summary>
		/// <param name="includeNonPublic">Indicates whether a non-public set accessor should be returned.</param>
		public static Action<TSource, object> GetSetAccessor<TSource>(this PropertyInfo propertyInfo,
			bool includeNonPublic = false)
		{
			return propertyInfo.GetSetAccessor<TSource, object>(includeNonPublic)?.Compile();
		}
	}
}