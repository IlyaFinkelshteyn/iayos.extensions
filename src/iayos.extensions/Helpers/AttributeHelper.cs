using System;
using System.Linq;
using System.Linq.Expressions;

namespace iayos.extensions
{
	/// <summary>
	/// Collection of utility helper methods to access attributes values etc
	/// </summary>
	public static class AttributeHelper
	{

		/// <summary>
		/// Reflect over a class and look for a particular Attribute on a particular property
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="propertyRefExpr"></param>
		/// <returns></returns>
		public static TAttribute GetAttributeFrom<TEntity, TAttribute>(
			Expression<Func<TEntity, object>> propertyRefExpr
			)
			where TEntity : class, new()
		{
			// Get the string name of the desired property
			var propertyName = PropertyHelper.GetName(propertyRefExpr);

			// Get the property itself
			var property = typeof(TEntity).GetProperty(propertyName);
			//var property = typeof(TEntity).GetProperties().Single(p => p.Name == propertyName);

			var attrType = typeof(TAttribute);
			return (TAttribute)property.GetCustomAttributes(attrType, false).First();
		}

	}
}
