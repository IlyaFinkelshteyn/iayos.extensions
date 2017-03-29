using System;
using System.Diagnostics;
using System.Globalization;

namespace iayos.extensions
{

	public static class StringExtensions
	{

		[DebuggerStepThrough]
		public static int? ToNullableInt(this string text)
		{
			if (!int.TryParse(text, out int result)) return null;
			return result;
		}


		[DebuggerStepThrough]
		public static long? ToNullableLong(this string text)
		{
			if (!long.TryParse(text, out long result)) return null;
			return result;
		}


		[DebuggerStepThrough]
		public static long? ToNullableLong(this string text, CultureInfo cultureInfo)
		{
			try
			{
				return Convert.ToInt64(text, cultureInfo);
			}
			catch
			{
				return null;
			}
		}


		[DebuggerStepThrough]
		public static float? ToNullableFloat(this string text)
		{
			if (!float.TryParse(text, out float result)) return null;
			return result;
		}


		[DebuggerStepThrough]
		public static float? ToNullableFloat(this string text, CultureInfo cultureInfo)
		{
			try
			{
				return Convert.ToSingle(text, cultureInfo);
			}
			catch
			{
				return null;
			}
		}


		[DebuggerStepThrough]
		public static double? ToNullableDouble(this string text)
		{
			if (!double.TryParse(text, out double result)) return null;
			return result;
		}


		[DebuggerStepThrough]
		public static double? ToNullableDouble(this string text, CultureInfo cultureInfo)
		{
			try
			{
				return Convert.ToDouble(text, cultureInfo);
			}
			catch
			{
				return null;
			}
		}


		[DebuggerStepThrough]
		public static decimal? ToNullableDecimal(this string text)
		{
			if (!decimal.TryParse(text, out decimal result)) return null;
			return result;
		}


		public static decimal? ToNullableDecimal(this string text, CultureInfo cultureInfo)
		{
			try
			{
				return Convert.ToDecimal(text, cultureInfo);
			}
			catch
			{
				return null;
			}
		}


		[DebuggerStepThrough]
		public static DateTime? ToNullableDateTime(this string text, DateTime defaulTime)
		{
			return !DateTime.TryParse(text, out DateTime result) ? defaulTime : result;
		}


		[DebuggerStepThrough]
		public static DateTime? ToNullableDateTime(this string text)
		{
			if (!DateTime.TryParse(text, out DateTime result)) return null;
			return result;
		}


		[DebuggerStepThrough]
		public static DateTimeOffset? ToNullableDateTimeOffset(this string text, DateTimeOffset defaultTime)
		{
			return !DateTimeOffset.TryParse(text, out DateTimeOffset result) ? defaultTime : result;
		}


		[DebuggerStepThrough]
		public static DateTimeOffset? ToNullableDateTimeOffset(this string text)
		{
			if (!DateTimeOffset.TryParse(text, out DateTimeOffset result)) return null;
			return result;
		}


		[DebuggerStepThrough]
		public static string RemoveLineEndings(this string value)
		{
			if (string.IsNullOrEmpty(value)) return value;
			var lineSeparator = ((char)0x2028).ToString();
			var paragraphSeparator = ((char)0x2029).ToString();

			return value.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty).Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
		}

	}
	
}
