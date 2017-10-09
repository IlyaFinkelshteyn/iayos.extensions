using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using iayos.extensions.ArrayExtensions;

namespace iayos.extensions
{
	
	/// <summary>
	/// 
	/// </summary>
	public static class ObjectExtensions
	{

		#region Deep Copy Object Cloning - see https://github.com/Burtsev-Alexey/net-object-deep-copy
		private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

		[DebuggerStepThrough]
		public static bool IsPrimitive(this Type type)
		{
			if (type == typeof(String)) return true;
			return type.GetTypeInfo().IsPrimitive;
		}

		[DebuggerStepThrough]
		public static Object Copy(this Object originalObject)
		{
			return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
		}

		[DebuggerStepThrough]
		private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
		{
			if (originalObject == null) return null;
			var typeToReflect = originalObject.GetType();
			if (IsPrimitive(typeToReflect)) return originalObject;
			if (visited.ContainsKey(originalObject)) return visited[originalObject];
			if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
			var cloneObject = CloneMethod.Invoke(originalObject, null);
			if (typeToReflect.IsArray)
			{
				var arrayType = typeToReflect.GetElementType();
				if (IsPrimitive(arrayType) == false)
				{
					Array clonedArray = (Array)cloneObject;
					clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
				}

			}
			visited.Add(originalObject, cloneObject);
			CopyFields(originalObject, visited, cloneObject, typeToReflect);
			RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
			return cloneObject;
		}

		[DebuggerStepThrough]
		private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
		{
			var baseType = typeToReflect.GetTypeInfo().BaseType;
			if (baseType != null)
			{
				RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, baseType);
				CopyFields(originalObject, visited, cloneObject, baseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
			}
		}

		[DebuggerStepThrough]
		private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
		{
			foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
			{
				if (filter != null && filter(fieldInfo) == false) continue;
				if (IsPrimitive(fieldInfo.FieldType)) continue;
				var originalFieldValue = fieldInfo.GetValue(originalObject);
				var clonedFieldValue = InternalCopy(originalFieldValue, visited);
				fieldInfo.SetValue(cloneObject, clonedFieldValue);
			}
		}


		public static T Copy<T>(this T original)
		{
			return (T)Copy((Object)original);
		}

		#endregion


		#region Property extensions


		/// <summary>
		/// 
		/// http://stackoverflow.com/a/5508068/4413476
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static object GetPropertyValue(this object entity, string propertyName)
		{
			return entity.GetType().GetProperties()
			   .Single(pi => pi.Name == propertyName)
			   .GetValue(entity, null);
		}


		/// <summary>
		/// http://stackoverflow.com/a/5508068/4413476
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static TValue GetPropertyValue<TValue>(this object entity, string propertyName)
		{
			return (TValue)GetPropertyValue(entity, propertyName);
		}


		/// <summary>
		/// Get property name for an INSTANCE:
		/// e.g. 
		/// User user = new User();
		/// string propertyName = user.GetPropertyName (u =&gt; u.Email);
		/// </summary>
		/// <typeparam name="TObject"></typeparam>
		/// <param name="type"></param>
		/// <param name="propertyRefExpr"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static string GetPropertyName<TObject>(this TObject type, Expression<Func<TObject, object>> propertyRefExpr)
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


		/// <summary>
		/// Call like: myObject.SetPropertyValue("myProperty", "myValue");
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propName"></param>
		/// <param name="value"></param>
		[DebuggerStepThrough]
		public static void SetPropertyValue(this object obj, string propName, object value)
		{
			PropertyInfo property = obj.GetType().GetProperty(propName);
			Type t = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
			object safeValue = (value == null) ? null : Convert.ChangeType(value, t);
			property.SetValue(obj, safeValue, null);
		}



		/// <summary>
		/// Callable like: ValidatePropertiesNotEmptyOrDefault(user, u => u.FirstName, u => u.LastName, u => u.Email);
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="expectEmpty"></param>
		/// <param name="propertySelectors"></param>
		private static void ValidatePropertiesAgainstEmptyOrDefaultedness<T>(this T obj, bool expectEmpty, params Expression<Func<T, long?>>[] propertySelectors) where T : class
		{
			foreach (var propertySelector in propertySelectors)
			{
				var expression = propertySelector.Body as MemberExpression;
				if (expression == null)
				{
					throw new ArgumentException("Developer error: Expression is not a property.");
				}

				var propertyToTest = (propertySelector.Compile())(obj);

				if (propertyToTest.GetValueOrDefault() == 0 && expectEmpty == false) // OK
				{
					throw new ArgumentException("Missing required value for " + expression.Member.Name);
				}

				if (propertyToTest.GetValueOrDefault() != 0 && expectEmpty == true)
				{
					throw new ArgumentException($"Detected value for {expression.Member.Name} where required to be empty");
				}
			}
		}



		private static readonly string ExpressionCannotBeNullMessage = "The expression cannot be null.";

		private static readonly string InvalidExpressionMessage = "Invalid expression.";


		[DebuggerStepThrough]
		public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression)
		{
			return GetMemberName(expression.Body);
		}


		[DebuggerStepThrough]
		public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
		{
			return expressions.Select(cExpression => GetMemberName(cExpression.Body)).ToList();
		}


		[DebuggerStepThrough]
		public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression)
		{
			return GetMemberName(expression.Body);
		}


		[DebuggerStepThrough]
		private static string GetMemberName(Expression expression)
		{
			if (expression == null) throw new ArgumentException(ExpressionCannotBeNullMessage);

			if (expression is MemberExpression)
			{
			//	Reference type property or field
				var memberExpression = (MemberExpression)expression;
				return memberExpression.Member.Name;
			}

			if (expression is MethodCallExpression)
			{
				//	Reference type method
				var methodCallExpression = (MethodCallExpression)expression;
				return methodCallExpression.Method.Name;
			}

			if (expression is UnaryExpression)
			{
				// Property, field of method returning value type
				var unaryExpression = (UnaryExpression)expression;
				return GetMemberName(unaryExpression);
			}

			throw new ArgumentException(InvalidExpressionMessage);
		}


		private static string GetMemberName(UnaryExpression unaryExpression)
		{
			if (unaryExpression.Operand is MethodCallExpression)
			{
				var methodExpression = (MethodCallExpression)unaryExpression.Operand;
				return methodExpression.Method.Name;
			}

			return ((MemberExpression)unaryExpression.Operand).Member.Name;
		}

		#endregion
	}


	[DebuggerStepThrough]
	public class ReferenceEqualityComparer : EqualityComparer<object>
	{
		[DebuggerStepThrough]
		public override bool Equals(object x, object y)
		{
			return ReferenceEquals(x, y);
		}
		[DebuggerStepThrough]
		public override int GetHashCode(object obj)
		{
			if (obj == null) return 0;
			return obj.GetHashCode();
		}
	}


	namespace ArrayExtensions
	{
		public static class ArrayExtensions
		{
			[DebuggerStepThrough]
			public static void ForEach(this Array array, Action<Array, int[]> action)
			{
				if (array.Length == 0) return;
				ArrayTraverse walker = new ArrayTraverse(array);
				do action(array, walker.Position);
				while (walker.Step());
			}
		}


		internal class ArrayTraverse
		{
			public int[] Position;
			private int[] maxLengths;

			[DebuggerStepThrough]
			public ArrayTraverse(Array array)
			{
				maxLengths = new int[array.Rank];
				for (int i = 0; i < array.Rank; ++i)
				{
					maxLengths[i] = array.GetLength(i) - 1;
				}
				Position = new int[array.Rank];
			}

			[DebuggerStepThrough]
			public bool Step()
			{
				for (int i = 0; i < Position.Length; ++i)
				{
					if (Position[i] < maxLengths[i])
					{
						Position[i]++;
						for (int j = 0; j < i; j++)
						{
							Position[j] = 0;
						}
						return true;
					}
				}
				return false;
			}
		}
	}

}