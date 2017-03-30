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
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="instance"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static TAttribute GetAttributeFrom<TEntity, TAttribute>(
			this TEntity instance,
			Expression<Func<TEntity, object>> expression
		)
			where TEntity : class, new()
		{
			var propertyName = instance.GetPropertyName(expression);
			var attrType = typeof(TAttribute);
			var property = instance.GetType().GetProperty(propertyName);
			return (TAttribute)property.GetCustomAttributes(attrType, false).First();
		}

	}

}