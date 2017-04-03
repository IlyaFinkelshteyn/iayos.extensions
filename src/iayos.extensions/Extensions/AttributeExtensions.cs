using System;
using System.Linq;
using System.Linq.Expressions;

namespace iayos.extensions
{

	/// <summary>
	/// Collection of extension methods related to access attributes values etc
	/// </summary>
	public static class AttributeExtensions
	{

		/// <summary>
		/// For a given instance, get a designated Property Attribute and pluck values as desired
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="instance"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static TAttribute GetAttributeOnProperty<TClass, TAttribute>(
			this TClass instance,
			Expression<Func<TClass, object>> expression
		)
			where TClass : class, new()
		{
			var propertyName = instance.GetPropertyName(expression);
			var attrType = typeof(TAttribute);
			var property = instance.GetType().GetProperty(propertyName);
			return (TAttribute)property.GetCustomAttributes(attrType, false).First();
		}


		/// <summary>
		/// Get the maximum length for a given property as defined by its attributes
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="propertyExpression"></param>
		/// <returns></returns>
		public static Int32 GetMaxLength<T>(this T instance, Expression<Func<T, string>> propertyExpression)
		{
			return AttributeHelper.GetMaxLength<T>(propertyExpression);
		}


		/// <summary>
		/// Get a particular Attribute from a particular Method on a particular object instance
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="obj"></param>
		/// <param name="methodName"></param>
		/// <param name="throwOnError"></param>
		/// <returns></returns>
		public static TAttribute GetAttributeOnMethod<TClass, TAttribute>(this TClass obj, string methodName, bool throwOnError = true)
			where TClass : class, new()
			where TAttribute : Attribute
		{
			return AttributeHelper.GetAttributeOnMethod<TClass, TAttribute>(methodName, throwOnError);
		}
	}

}