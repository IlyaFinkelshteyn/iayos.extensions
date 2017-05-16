//using System;
//using iayos.extensions.Helpers;

//namespace iayos.extensions
//{

//	/// <summary>
//	/// https://github.com/ByteTerrace/ByteTerrace.CSharp.TypeAccessor
//	/// Provides cached access to a type's properties.
//	/// </summary>
//	public class PropertyAccessor
//	{
//		/// <summary>
//		/// Provides cached access to a type's property getters.
//		/// </summary>
//		public PropertyReader PropertyReader { get; }
//		/// <summary>
//		/// Provides cached access to a type's property setters.
//		/// </summary>
//		public PropertyWriter PropertyWriter { get; }

//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyAccessor"/> class.
//		/// </summary>
//		/// <param name="type">The Type to generate accessors from.</param>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public PropertyAccessor(Type type, bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			PropertyReader = PropertyReader.Create(type: type, includePublic: includePublic, includeNonPublic: includeNonPublic, includeInstance: includeInstance, includeStatic: includeStatic);
//			PropertyWriter = PropertyWriter.Create(type: type, includePublic: includePublic, includeNonPublic: includeNonPublic, includeInstance: includeInstance, includeStatic: includeStatic);
//		}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyAccessor"/> class.
//		/// </summary>
//		/// <param name="type">The Type to generate accessors from.</param>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public static PropertyAccessor Create(Type type, bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			return new PropertyAccessor(type: type, includePublic: includePublic, includeNonPublic: includeNonPublic, includeInstance: includeInstance, includeStatic: includeStatic);
//		}
//		/// <summary>
//		/// Initializes a new instance of the <see cref="PropertyAccessor"/> class.
//		/// </summary>
//		/// <param name="includePublic">Indicates whether public properties should be included.</param>
//		/// <param name="includeNonPublic">Indicates whether non-public properties should be included.</param>
//		/// <param name="includeInstance">Indicates whether instance properties should be included.</param>
//		/// <param name="includeStatic">Indicates whether static properties should be included.</param>
//		public static PropertyAccessor Create<T>(bool includePublic = true, bool includeNonPublic = false, bool includeInstance = true, bool includeStatic = false)
//		{
//			return Create(type: typeof(T), includePublic: includePublic, includeNonPublic: includeNonPublic, includeInstance: includeInstance, includeStatic: includeStatic);
//		}
//	}
//}