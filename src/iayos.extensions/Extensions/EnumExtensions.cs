using System;
using System.Diagnostics;

namespace iayos.extensions
{
	public static class EnumExtensions
	{

		/// <summary>
		/// http://stackoverflow.com/questions/16100/how-do-i-convert-a-string-to-an-enum-in-c
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static T ToEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}


		[DebuggerStepThrough]
		public static T ToEnumUppercased<T>(this string value) where T : struct
		{
			value = value.ToUpperInvariant();
			return value.ToEnum<T>();
		}


		[DebuggerStepThrough]
		public static T ToEnum<T>(this string value, T defaultValue) where T : struct
		{
			if (string.IsNullOrEmpty(value)) return defaultValue;

			T result;
			return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
		}


		[DebuggerStepThrough]
		public static T ToEnumUppercased<T>(this string value, T defaultValue) where T : struct
		{
			value = value.ToUpperInvariant();
			return ToEnum(value, defaultValue);
		}

	}

}