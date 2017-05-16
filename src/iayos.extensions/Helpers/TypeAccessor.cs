using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace iayos.extensions
{

	/// <summary>
	/// Dale created partial classes so i could add extra methods etc
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class TypeAccessor<T>
	{
		public string GetPropertyName<TValue>(T instance, Expression<Func<T, TValue>> property)=> GetMemberInfo(property).Name;


		public MemberInfo GetMemberInfo(Expression expression)
		{
			LambdaExpression lambda = (LambdaExpression)expression;
			MemberExpression memberExpr = null;
			switch (lambda.Body.NodeType)
			{
				case ExpressionType.Convert:
					memberExpr =
						((UnaryExpression)lambda.Body).Operand as MemberExpression;
					break;
				case ExpressionType.MemberAccess:
					memberExpr = lambda.Body as MemberExpression;
					break;
			}
			return memberExpr.Member;
		}


		public IList<CustomAttributeData> ListPropertyAttributes(T instance, Expression<Func<T, object>> property)
		{
			return GetMemberInfo(property).GetCustomAttributesData();
		} 

	}
}