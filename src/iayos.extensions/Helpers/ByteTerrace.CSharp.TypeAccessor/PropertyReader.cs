//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Reflection.Emit;

//namespace iayos.extensions.Helpers
//{

//	/// <summary>
//	/// https://github.com/ByteTerrace/ByteTerrace.CSharp.TypeAccessor
//	/// Provides cached access to a type's property getters.
//	/// </summary>
//	public interface IPropertyReader
//	{
//		/// <summary>
//		/// Returns the read-only cache of property getters that have been initialized for the Type.
//		/// </summary>
//		IReadOnlyDictionary<string, Delegate> GetterCache { get; }
//		/// <summary>
//		/// Returns the Type that this reader was generated from.
//		/// </summary>
//		Type Type { get; }

//		/// <summary>
//		/// Uses the <see cref="IPropertyReader"/> to get the value of a property by name.
//		/// </summary>
//		/// <param name="instance">The instance to get a property value from.</param>
//		/// <param name="propertyName">The name of the property to retrieve.</param>
//		TProperty GetValue<TProperty>(object instance, string propertyName);
//		/// <summary>
//		/// Uses the <see cref="IPropertyReader"/> to get the value of all available properties.
//		/// </summary>
//		/// <param name="instance">The instance to get property values from.</param>
//		IEnumerable<KeyValuePair<string, object>> GetValues(object instance);
//	}

//	/// <summary>
//	/// Provides cached access to a type's property getters.
//	/// </summary>
//	public sealed class PropertyReader : IPropertyReader
//	{
//		private readonly BindingFlags m_bindingFlags;
//		private readonly Func<object, IEnumerable<KeyValuePair<string, object>>> m_getValuesImpl;

//		/// <summary>
//		/// Returns the read-only cache of property getters that have been initialized for the Type.
//		/// </summary>
//		public IReadOnlyDictionary<string, Delegate> GetterCache { get; } = (new ConcurrentDictionary<string, Delegate>() as IReadOnlyDictionary<string, Delegate>);
//		/// <summary>
//		/// Returns the Type that this reader was generated from.
//		/// </summary>
//		public Type Type { get; }

//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyReader"/> class.
//		/// </summary>
//		/// <param name="type">The Type to generate property getters from.</param>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public PropertyReader(Type type, bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			Type = type;

//			m_bindingFlags =
//				(includeInstance ? BindingFlags.Instance : BindingFlags.Default)
//				| (includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default)
//				| (includePublic ? BindingFlags.Public : BindingFlags.Default)
//				| (includeStatic ? BindingFlags.Static : BindingFlags.Default);
//			m_getValuesImpl = BuildGetValuesImpl() as Func<object, KeyValuePair<string, object>[]>;
//		}

//		/// <summary>
//		/// Uses the <see cref="PropertyReader"/> to get the value of a property by name.
//		/// </summary>
//		/// <param name="instance">The instance to get a property value from.</param>
//		/// <param name="propertyName">The name of the property to retrieve.</param>
//		public TProperty GetValue<TProperty>(object instance, string propertyName)
//		{
//			Delegate getter;

//			try
//			{
//				getter = GetterCache[propertyName];
//			}
//			catch (KeyNotFoundException)
//			{
//				var propertyInfo = Type.GetProperty(propertyName, m_bindingFlags);

//				if ((propertyInfo != null) && (getter = GetGetAccessor(propertyInfo, m_bindingFlags.HasFlag(BindingFlags.NonPublic))) != null)
//				{
//					(GetterCache as ConcurrentDictionary<string, Delegate>).TryAdd(propertyName, getter);
//				}
//				else
//				{
//					throw new KeyNotFoundException($"a property getter with the name {propertyName} could not be found on {Type.FullName}");
//				}
//			}

//			return (getter as Func<object, TProperty>)(instance);
//		}
//		/// <summary>
//		/// Uses the <see cref="PropertyReader"/> to get the value of all available properties.
//		/// </summary>
//		/// <param name="instance">The instance to get property values from.</param>
//		public IEnumerable<KeyValuePair<string, object>> GetValues(object instance)
//		{
//			return m_getValuesImpl(instance);
//		}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyReader"/> class.
//		/// </summary>
//		/// <param name="type">The Type to generate property getters from.</param>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public static PropertyReader Create(Type type, bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			return new PropertyReader(type, includePublic, includeNonPublic, includeInstance, includeStatic);
//		}
//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyReader"/> class.
//		/// </summary>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public static PropertyReader Create<T>(bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			return Create(typeof(T), includePublic, includeNonPublic, includeInstance, includeStatic);
//		}
//		/// <summary>
//		/// Generates a <see cref="Func{_,_}"/> delegate that represents the <see cref="PropertyInfo"/>'s getter.
//		/// </summary>
//		/// <param name="propertyInfo">The <see cref="PropertyInfo"/> instance to extract a getter from.</param>
//		/// <param name="includeNonPublic">Indicates whether a non-public get accessor should be returned.</param>
//		public static Delegate GetGetAccessor(PropertyInfo propertyInfo, bool includeNonPublic = false)
//		{
//			if (propertyInfo == null) { throw new ArgumentNullException(nameof(propertyInfo)); }
//			if (propertyInfo.GetIndexParameters().Length > 0) { throw new NotImplementedException("indexer properties are not supported"); }

//			var getMethod = propertyInfo.GetGetMethod(includeNonPublic);

//			if (getMethod != null)
//			{
//				var getMethodDeclaringType = getMethod.DeclaringType;
//				var getMethodArg0Type = typeof(object);
//				var getMethodReturnType = getMethod.ReturnType;
//				var getMethodDynamicCall = new DynamicMethod(
//					$"{getMethod.Name}_DynamicGetter_{Guid.NewGuid().ToString("N").ToUpper()}",
//					getMethodReturnType,
//					new[] { getMethodArg0Type },
//					getMethodDeclaringType,
//					true
//				);
//				var il = getMethodDynamicCall.GetILGenerator();

//				if (getMethod.IsStatic)
//				{
//					il.EmitCall(OpCodes.Call, getMethod, null);
//				}
//				else
//				{
//					il.Emit(OpCodes.Ldarg_0);

//					if (getMethodDeclaringType.IsValueType)
//					{
//						il.Emit(OpCodes.Unbox, getMethodDeclaringType);
//						il.EmitCall(OpCodes.Call, getMethod, null);
//					}
//					else
//					{
//						il.Emit(OpCodes.Castclass, getMethodDeclaringType);
//						il.EmitCall(OpCodes.Callvirt, getMethod, null);
//					}
//				}
//				il.Emit(OpCodes.Ret);

//				return getMethodDynamicCall.CreateDelegate(typeof(Func<,>).MakeGenericType(getMethodArg0Type, getMethodReturnType));
//			}
//			else
//			{
//				return null;
//			}
//		}

//		private Delegate BuildGetValuesImpl()
//		{
//			var properties = Type.GetProperties(m_bindingFlags);
//			var stringType = typeof(string);
//			var objectType = typeof(object);
//			var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(stringType, objectType);
//			var getValuesReturnType = keyValuePairType.MakeArrayType();
//			var getValuesImpl = new DynamicMethod(
//				$"GetValues_Dynamic_{Guid.NewGuid().ToString("N").ToUpper()}",
//				getValuesReturnType,
//				new[] { objectType },
//				typeof(PropertyReader),
//				true
//			);
//			var il = getValuesImpl.GetILGenerator();
//			var arr = il.DeclareLocal(getValuesReturnType);
//			MethodInfo getMethod;

//			il.Emit(OpCodes.Ldc_I4, properties.Length);
//			il.Emit(OpCodes.Newarr, keyValuePairType);
//			il.Emit(OpCodes.Stloc_0);

//			for (var i = 0; i < properties.Length; i++)
//			{
//				getMethod = properties[i].GetGetMethod(m_bindingFlags.HasFlag(BindingFlags.NonPublic));

//				if (getMethod != null)
//				{
//					il.Emit(OpCodes.Ldloc_0);
//					il.Emit(OpCodes.Ldc_I4, i);
//					il.Emit(OpCodes.Ldstr, properties[i].Name);

//					if (getMethod.IsStatic)
//					{
//						il.EmitCall(OpCodes.Call, getMethod, null);
//					}
//					else
//					{
//						il.Emit(OpCodes.Ldarg_0);

//						if (Type.IsValueType)
//						{
//							il.Emit(OpCodes.Unbox, Type);
//							il.EmitCall(OpCodes.Call, getMethod, null);
//						}
//						else
//						{
//							il.Emit(OpCodes.Castclass, Type);
//							il.EmitCall(OpCodes.Callvirt, getMethod, null);
//						}
//					}

//					if (getMethod.ReturnType.IsValueType)
//					{
//						il.Emit(OpCodes.Box, getMethod.ReturnType);
//					}

//					il.Emit(OpCodes.Newobj, keyValuePairType.GetConstructor(new[] { stringType, objectType }));
//					il.Emit(OpCodes.Stelem, keyValuePairType);
//				}
//			}

//			il.Emit(OpCodes.Ldloc_0);
//			il.Emit(OpCodes.Ret);

//			return getValuesImpl.CreateDelegate(typeof(Func<,>).MakeGenericType(objectType, getValuesReturnType));
//		}
//	}
//}
