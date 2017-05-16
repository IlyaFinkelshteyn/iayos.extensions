using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace iayos.extensions
{
	public static class ObjectExtensions
	{
		public static string GetPropertyName<TClass, TValue>(this TClass instance, Expression<Func<TClass, TValue>> expression)
		{
			var accessor = TypeAccessor.Create(instance);
			var propertyName = accessor.GetPropertyName(instance, expression);
			return propertyName;
		}


		public static TValue GetPropertyValue<TClass, TValue>(this TClass instance, Expression<Func<TClass, TValue>> expression)
		{
			var accessor = TypeAccessor.Create(instance);
			var propertyValue = accessor.GetProperty(instance, expression);
			return propertyValue;
		}


		/// <summary>
		/// For a given instance get all the attributes of a certain type
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="instance"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static List<TAttribute> ListPropertyAttributes<TClass, TAttribute>(this TClass instance, Expression<Func<TClass, object>> expression)
			where TClass : class, new()
		{
			var accessor = TypeAccessor.Create(instance);
			var memberInfo = accessor.GetMemberInfo(expression);
			var attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), false).ToList() as List<TAttribute>;
			return attributes;
		}


		/// <summary>
		/// For a given instance, find the first instance of an attribute by type, or default if none
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="instance"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static TAttribute FindPropertyAttribute<TClass, TAttribute>(this TClass instance, Expression<Func<TClass, object>> expression)
			where TClass : class, new()
		{
			return instance.ListPropertyAttributes<TClass, TAttribute>(expression).FirstOrDefault();
		}


	}


	/// <summary>
	/// Collection of extension methods related to access attributes values etc
	/// </summary>
	public static class AttributeExtensions
	{

		

		///// <summary>
		///// Testing testing
		///// </summary>
		///// <typeparam name="TClass"></typeparam>
		///// <typeparam name="TAttribute"></typeparam>
		///// <param name="instance"></param>
		///// <param name="expression"></param>
		///// <returns></returns>
		//public static TAttribute GetAttributeOnProperty<TClass, TPropertyType, TAttribute>(
		//	this TClass instance,
		//	Expression<Func<TClass, TPropertyType>> expression
		//)
		//	where TClass : class, new()
		//{
		//	var propertyName = instance.GetPropertyName(expression);
		//	var attrType = typeof(TAttribute);
		//	var propertyInfo = instance.GetType().GetProperty(propertyName);
		//	return (TAttribute)propertyInfo.GetCustomAttributes(attrType, false).First();
		//}



		///// <summary>
		///// For a given instance, get a designated Property Attribute and pluck values as desired
		///// </summary>
		///// <typeparam name="TClass"></typeparam>
		///// <typeparam name="TAttribute"></typeparam>
		///// <param name="instance"></param>
		///// <param name="expression"></param>
		///// <returns></returns>
		//public static TAttribute GetAttributeOnProperty<TClass, TAttribute>(
		//	this TClass instance,
		//	Expression<Func<TClass, object>> expression
		//)
		//	where TClass : class, new()
		//{
		//	var propertyName = instance.GetPropertyName(expression);
		//	var attrType = typeof(TAttribute);
		//	var propertyInfo = instance.GetType().GetProperty(propertyName);
		//	return (TAttribute)propertyInfo.GetCustomAttributes(attrType, false).First();
		//}


		/// <summary>
		/// Get the maximum length for a given property as defined by its StringLength
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="propertyExpression"></param>
		/// <returns></returns>
		public static Int32 GetStringLengthMax<T>(this T instance, Expression<Func<T, string>> propertyExpression)
		{
			return AttributeHelper.GetStringLengthMax<T>(propertyExpression);
		}


		/// <summary>
		/// Get the minimum length for a given property as defined by its StringLength
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <param name="propertyExpression"></param>
		/// <returns></returns>
		public static Int32 GetStringLengthMin<T>(this T instance, Expression<Func<T, string>> propertyExpression)
		{
			return AttributeHelper.GetStringLengthMin<T>(propertyExpression);
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