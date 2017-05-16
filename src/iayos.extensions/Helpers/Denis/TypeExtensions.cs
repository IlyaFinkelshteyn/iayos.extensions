using System;

namespace iayos.extensions {
	public static class TypeExtensions
	{
		public static bool HasDefaultConstructor(this Type type)
		{
			return (type.IsValueType || (type.GetConstructor(Type.EmptyTypes) != null));
		}
	}
}