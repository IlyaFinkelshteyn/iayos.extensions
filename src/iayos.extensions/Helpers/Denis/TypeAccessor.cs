using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace iayos.extensions
{

	/// <summary>
	/// See: https://codereview.stackexchange.com/a/151199/138947
	/// public class Point2D
	///{
	///    public double X { get; set; }
	///    public double Y { get; set; }
	///}
	///
	///var pointA = new Point2D {X = 9000.01, Y = 0.0};
	///var accessor = TypeAccessor.CreateNewTypeAccessor(pointA);
	///var pointB = new Point2D();
	///
	/////obtains properties by name with compile time safety
	///Dictionary&lt;string, object&gt; a = accessor.GetProperties(pointA, new List&lt;Expression&lt;Func&lt;Point2D, object&gt;&gt;&gt;
	///{
	///    d =&gt; d.X,
	///    d =&gt; d.Y
	///});
	///
	///accessor.CloneProperties(pointA, pointB); // pointB.X should now be 9000.01
	///accessor.SetProperty(pointA, p =&gt; p.X, 0.0);// sets pointA.X to 0.0
	///Console.WriteLine(accessor.GetProperty(pointA, p =&gt; p.X)); // prints pointA.X, should be 0.0
	///accessor.SetToDefault(ref pointA); // sets pointA's properties to default the accessor's default values
	///Console.WriteLine(accessor.GetProperty(pointA, p =&gt; p.X)); // prints pointA.X, should be 9000.01
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public partial class TypeAccessor<T>
	{
		private readonly Func<T, T> m_applyDefaultValues;
		private readonly Func<T> m_constructType;

		public ReadOnlyCollection<string> CloneableProperties { get; }

		public ReadOnlyDictionary<string, Func<T, object>> GetterCache { get; }

		public ReadOnlyDictionary<string, Action<T, object>> SetterCache { get; }


		public TypeAccessor(T defaultValue, bool includeNonPublic = false)
		{
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance |
			                                                    (includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default) |
			                                                    BindingFlags.Public);

			GetterCache = new ReadOnlyDictionary<string, Func<T, object>>(properties.Select(propertyInfo => new
				{
					PropertyName = propertyInfo.Name,
					PropertyGetAccessor = PropertyInfoExtensions.GetGetAccessor<T>(propertyInfo, includeNonPublic)
				}).Where(a => a.PropertyGetAccessor != null)
				.ToDictionary(a => a.PropertyName, a => a.PropertyGetAccessor));

			SetterCache = new ReadOnlyDictionary<string, Action<T, object>>(properties.Select(propertyInfo => new
				{
					PropertyName = propertyInfo.Name,
					PropertySetAccessor = propertyInfo.GetSetAccessor<T>(includeNonPublic)
				}).Where(a => a.PropertySetAccessor != null)
				.ToDictionary(a => a.PropertyName, a => a.PropertySetAccessor));

			CloneableProperties = Array.AsReadOnly(GetterCache.Keys.Intersect(SetterCache.Keys).ToArray());


			if (typeof(T).IsValueType)
			{
				m_applyDefaultValues = instance => defaultValue;
				m_constructType = () => defaultValue;
			}
			else if (defaultValue != null)
			{
				var defaultConstructor = GetDefaultConstructor();
				var propertyValues = GetProperties(defaultValue, CloneableProperties).ToArray();

				m_applyDefaultValues = instance =>
				{
					SetProperties(instance, propertyValues);
					return instance;
				};
				m_constructType = () => m_applyDefaultValues(defaultConstructor());
			}
			else
			{
				m_applyDefaultValues = instance => default(T);
				m_constructType = () => default(T);
			}

		}
		public void CloneProperties(T source, T target)
		{
			SetProperties(target, GetProperties(source, CloneableProperties));
		}

		public void SetToDefault(ref T instance)
		{
			instance = m_applyDefaultValues(instance);
		}

		public T New()
		{
			return m_constructType();
		}

		private Dictionary<string, object> GetProperties(T instance, IEnumerable<string> properties)
			=> properties?.ToDictionary(propertyName => propertyName,
				   propertyName => GetProperty(instance, propertyName)) ?? new Dictionary<string, object>();

		public Dictionary<string, object> GetProperties(T instance,
			IEnumerable<Expression<Func<T, object>>> properties)
			=> properties?.ToDictionary(property => GetMemberInfo(property).Name,
				   property => GetProperty(instance, property)) ?? new Dictionary<string, object>();

		public Dictionary<string, object> GetProperties(T instance)
			=> GetterCache.Keys.ToDictionary(key => key, key => GetProperty(instance, key));

		private object GetProperty(T instance, string propertyName)
			=> GetterCache[propertyName].Invoke(instance);

		public TValue GetProperty<TValue>(T instance, Expression<Func<T, TValue>> property)
			=> (TValue)GetterCache[GetMemberInfo(property).Name](instance);

		private void SetProperty(T instance, string propertyName, object value)
		{
			Action<T, object> setter;

			if (SetterCache.TryGetValue(propertyName, out setter))
			{
				setter(instance, value);
			}
			else
			{
				throw new KeyNotFoundException(
					$"a property setter with the name does not {propertyName} exist on {typeof(T).FullName}");
			}
		}

		public void SetProperty<TValue>(T instance, Expression<Func<T, TValue>> property, TValue value)
			=> SetterCache[GetMemberInfo(property).Name](instance, value);


		private void SetProperties<TValue>(T instance, IEnumerable<KeyValuePair<string, TValue>> properties)
		{
			if (properties != null)
			{
				foreach (var property in properties)
				{
					SetProperty(instance, property.Key, property.Value);
				}
			}
		}


		public void SetProperties<TValue>(T instance, IEnumerable<KeyValuePair<Expression<Func<T, TValue>>, TValue>> propertiesInfo)
		{
			foreach (var propertyInfo in propertiesInfo)
			{
				SetterCache[GetMemberInfo(propertyInfo.Key).Name](instance, propertyInfo.Value);
			}
		}

		
		private static Func<T> GetDefaultConstructor()
		{
			var type = typeof(T);

			if (type == typeof(string))
			{
				return Expression.Lambda<Func<T>>(Expression.TypeAs(Expression.Constant(null), typeof(string))).Compile();
			}
			if (type.HasDefaultConstructor())
			{
				return Expression.Lambda<Func<T>>(Expression.New(type)).Compile();
			}
			return () => (T)FormatterServices.GetUninitializedObject(type);
		}

		
	}


	public static class TypeAccessor
	{
		/// <summary>
		/// Creates a new instance of the <see cref="TypeAccessor{_}"/> class using the specified <see cref="T"/>.
		/// </summary>
		public static TypeAccessor<T> Create<T>(T instance, bool includeNonPublic = false)
		{
			return new TypeAccessor<T>(instance, includeNonPublic);
		}
	}
}