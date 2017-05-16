using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace iayos.extensions
{
	/// <summary>
	/// Collection of utility helper methods to access attributes values etc
	/// </summary>
	public static class AttributeHelper
	{

		
		/// <summary>
		/// Get any value from attribute from any class
		/// var authorName = AttributeHelper.GetPropertyAttributeValue&lt;Book, string, AuthorAttribute, string&gt;(prop =&gt; prop.Name, attr =&gt; attr.Author);
		/// See: http://stackoverflow.com/a/32501356/4413476
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TOut"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="propertyExpression"></param>
		/// <param name="valueSelector"></param>
		/// <returns></returns>
		public static TValue GetAttributeValueOnProperty<TClass, TOut, TAttribute, TValue>(Expression<Func<TClass, TOut>> propertyExpression, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
		{
			var expression = (MemberExpression)propertyExpression.Body;
			var propertyInfo = (PropertyInfo)expression.Member;
			var attr = propertyInfo.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
			if (attr == null) throw new MissingMemberException(typeof(TClass).Name + "." + propertyInfo.Name, typeof(TAttribute).Name);
			return valueSelector(attr);
		}


		/// <summary>
		/// Get the maximum length based on stringlength attribute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyExpression"></param>
		/// <returns></returns>
		public static Int32 GetStringLengthMax<T>(Expression<Func<T, string>> propertyExpression)
		{
			return GetAttributeValueOnProperty<T, string, StringLengthAttribute, Int32>(propertyExpression, attr => attr.MaximumLength);
		}


		/// <summary>
		/// Get the minimum length based on stringlength attribute
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyExpression"></param>
		/// <returns></returns>
		public static Int32 GetStringLengthMin<T>(Expression<Func<T, string>> propertyExpression)
		{
			return GetAttributeValueOnProperty<T, string, StringLengthAttribute, Int32>(propertyExpression, attr => attr.MinimumLength);
		}


		/// <summary>
		/// Get a particular Attribute from a particular Method on a particular Class
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="methodName"></param>
		/// <param name="throwOnError"></param>
		/// <returns></returns>
		public static TAttribute GetAttributeOnMethod<TClass, TAttribute>(string methodName, bool throwOnError = true)
		{
			var attribute = typeof(TClass).GetMethod(methodName).CustomAttributes.OfType<TAttribute>().FirstOrDefault();
			if (attribute == null && throwOnError) throw new ArgumentException($"{typeof(TAttribute).FullName} not found on {typeof(TClass)}.{methodName}");
			return attribute;
		}

	}

}
