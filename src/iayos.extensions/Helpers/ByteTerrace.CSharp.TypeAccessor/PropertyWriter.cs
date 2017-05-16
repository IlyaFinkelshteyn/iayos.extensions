//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Reflection.Emit;

//namespace iayos.extensions.Helpers
//{
//	/// <summary>
//	///  https://github.com/ByteTerrace/ByteTerrace.CSharp.TypeAccessor
//	/// Provides cached access to a type's property setters.
//	/// </summary>
//	public interface IPropertyWriter
//	{
//		/// <summary>
//		/// Returns the read-only cache of property setters that have been initialized for the Type.
//		/// </summary>
//		IReadOnlyDictionary<string, Delegate> SetterCache { get; }
//		/// <summary>
//		/// Returns the Type that this writer was generated from.
//		/// </summary>
//		Type Type { get; }

//		/// <summary>
//		/// Uses the <see cref="IPropertyWriter"/> to set the value of a property by name.
//		/// </summary>
//		/// <param name="instance">The instance to set a property value on.</param>
//		/// <param name="propertyName">The name of the property to set.</param>
//		/// <param name="value">The value that will be applied to the property.</param>
//		void SetValue<TProperty>(object instance, string propertyName, TProperty value);
//	}

//	/// <summary>
//	/// Provides cached access to a type's property setters.
//	/// </summary>
//	public sealed class PropertyWriter : IPropertyWriter
//	{
//		private readonly BindingFlags m_bindingFlags;

//		/// <summary>
//		/// Returns the read-only cache of property setters that have been initialized for the Type.
//		/// </summary>
//		public IReadOnlyDictionary<string, Delegate> SetterCache { get; } = (new ConcurrentDictionary<string, Delegate>() as IReadOnlyDictionary<string, Delegate>);
//		/// <summary>
//		/// Returns the Type that this reader was generated from.
//		/// </summary>
//		public Type Type { get; }

//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyWriter"/> class.
//		/// </summary>
//		/// <param name="type">The Type to generate property setters from.</param>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public PropertyWriter(Type type, bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			Type = type;

//			m_bindingFlags =
//				(includeInstance ? BindingFlags.Instance : BindingFlags.Default)
//				| (includeNonPublic ? BindingFlags.NonPublic : BindingFlags.Default)
//				| (includePublic ? BindingFlags.Public : BindingFlags.Default)
//				| (includeStatic ? BindingFlags.Static : BindingFlags.Default);
//		}

//		/// <summary>
//		/// Uses the <see cref="PropertyWriter"/> to set the value of a property by name.
//		/// </summary>
//		/// <param name="instance">The instance to set a property value on.</param>
//		/// <param name="propertyName">The name of the property to set.</param>
//		/// <param name="value">The value that will be applied to the property.</param>
//		public void SetValue<TProperty>(object instance, string propertyName, TProperty value)
//		{
//			Delegate setter;

//			try
//			{
//				setter = SetterCache[propertyName];
//			}
//			catch (KeyNotFoundException)
//			{
//				var propertyInfo = Type.GetProperty(propertyName, m_bindingFlags);

//				if ((propertyInfo != null) && (setter = GetSetAccessor(propertyInfo, m_bindingFlags.HasFlag(BindingFlags.NonPublic))) != null)
//				{
//					(SetterCache as ConcurrentDictionary<string, Delegate>).TryAdd(propertyName, setter);
//				}
//				else
//				{
//					throw new KeyNotFoundException($"a property setter with the name {propertyName} could not be found on {Type.FullName}");
//				}
//			}

//			(setter as Action<object, TProperty>)(instance, value);
//		}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyWriter"/> class.
//		/// </summary>
//		/// <param name="type">The Type to generate property setters from.</param>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public static PropertyWriter Create(Type type, bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			return new PropertyWriter(type, includePublic, includeNonPublic, includeInstance, includeStatic);
//		}
//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyWriter"/> class.
//		/// </summary>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public static PropertyWriter Create<T>(bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			return Create(typeof(T), includePublic, includeNonPublic, includeInstance, includeStatic);
//		}
//		/// <summary>
//		/// Generates an <see cref="Action{_,_}"/> delegate that represents the <see cref="PropertyInfo"/>'s setter.
//		/// </summary>
//		/// <param name="propertyInfo">The <see cref="PropertyInfo"/> instance to extract a setter from.</param>
//		/// <param name="includeNonPublic">Indicates whether a non-public set accessor should be returned.</param>
//		public static Delegate GetSetAccessor(PropertyInfo propertyInfo, bool includeNonPublic = false)
//		{
//			if (propertyInfo == null) { throw new ArgumentNullException(nameof(propertyInfo)); }
//			if (propertyInfo.GetIndexParameters().Length > 0) { throw new NotImplementedException("indexer properties are not supported"); }

//			var setMethod = propertyInfo.GetSetMethod(includeNonPublic);

//			if (setMethod != null)
//			{
//				var setMethodDeclaringType = setMethod.DeclaringType;
//				var setMethodArg0Type = typeof(object);
//				var setMethodArg1Type = propertyInfo.PropertyType;
//				var setMethodDynamicCall = new DynamicMethod(
//					$"{setMethod.Name}_DynamicSetter_{Guid.NewGuid().ToString("N").ToUpper()}",
//					null,
//					new[] { setMethodArg0Type, setMethodArg1Type },
//					setMethodDeclaringType,
//					true
//				);
//				var il = setMethodDynamicCall.GetILGenerator();

//				if (setMethod.IsStatic)
//				{
//					il.Emit(OpCodes.Ldarg_1);
//					il.EmitCall(OpCodes.Call, setMethod, null);
//				}
//				else
//				{
//					il.Emit(OpCodes.Ldarg_0);

//					if (setMethodDeclaringType.IsValueType)
//					{
//						il.Emit(OpCodes.Unbox, setMethodDeclaringType);
//						il.Emit(OpCodes.Ldarg_1);
//						il.EmitCall(OpCodes.Call, setMethod, null);
//					}
//					else
//					{
//						il.Emit(OpCodes.Castclass, setMethodDeclaringType);
//						il.Emit(OpCodes.Ldarg_1);
//						il.EmitCall(OpCodes.Callvirt, setMethod, null);
//					}
//				}
//				il.Emit(OpCodes.Ret);

//				return setMethodDynamicCall.CreateDelegate(typeof(Action<,>).MakeGenericType(setMethodArg0Type, setMethodArg1Type));
//			}
//			else
//			{
//				return null;
//			}
//		}
//	}
//}
