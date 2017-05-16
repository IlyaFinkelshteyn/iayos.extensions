using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace iayos.extensions
{
	/// <summary>
	/// How to use nice short type-safe property names on generic classes
	/// http://ivanz.com/2009/12/04/how-to-avoid-passing-property-names-as-strings-using-c-3-0-expression-trees/
	/// </summary>
	public static class ClassHelper
	{

		/// <summary>
		/// Get property name for a TYPE:
		/// e.g. string propertyName = PropertyUtil.GetPropertyName&lt;User&gt; (u =&gt; u.Email);
		/// </summary>
		/// <typeparam name="TClass"></typeparam>
		/// <param name="propertyRefExpr"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static string GetPropertyName<TClass>(Expression<Func<TClass, object>> propertyRefExpr) where TClass : class, new()
		{
			return GetPropertyNameCore(propertyRefExpr.Body);
		}

		
		[DebuggerStepThrough]
		private static string GetPropertyNameCore(Expression propertyRefExpr)
		{
			if (propertyRefExpr == null) throw new ArgumentNullException("propertyRefExpr", "propertyRefExpr is null.");

			MemberExpression memberExpr = propertyRefExpr as MemberExpression;
			if (memberExpr == null)
			{
				UnaryExpression unaryExpr = propertyRefExpr as UnaryExpression;
				if (unaryExpr != null && unaryExpr.NodeType == ExpressionType.Convert) memberExpr = unaryExpr.Operand as MemberExpression;
			}

			if (memberExpr != null && memberExpr.Member.MemberType == MemberTypes.Property) return memberExpr.Member.Name;

			throw new ArgumentException("No property reference expression was found.", "propertyRefExpr");
		}

	}
}
