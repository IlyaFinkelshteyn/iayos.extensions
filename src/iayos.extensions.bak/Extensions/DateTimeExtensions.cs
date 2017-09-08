using System;

namespace iayos.extensions
{
	public static class DateTimeExtensions
	{

		/// <summary>
		/// return new DateTime(date.Year, date.Month, 1);
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime StartOfMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1);
		}


		/// <summary>
		/// return date.StartOfNextMonth().AddMonths(1);
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime StartOfNextMonth(this DateTime date)
		{
			return date.StartOfMonth().AddMonths(1);
		}


		/// <summary>
		/// return date.StartOfNextMonth().AddDays(-1);
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime StartOfLastDayOfMonth(this DateTime date)
		{
			return date.StartOfNextMonth().AddDays(-1);
		}


		///// <summary>
		///// return FirstOfNextMonth(date).AddDays(-1);
		///// </summary>
		///// <param name="date"></param>
		///// <returns></returns>
		//public static DateTime EndOfMonth(this DateTime date)
		//{
		//	return FirstOfNextMonth(date).AddDays(-1);
		//}


		///// <summary>
		///// return date.StartOfMonth().AddMonths(1);
		///// </summary>
		///// <param name="date"></param>
		///// <returns></returns>
		//public static DateTime FirstOfNextMonth(this DateTime date)
		//{
		//	return date.StartOfMonth().AddMonths(1);
		//}


		/// <summary>
		/// return new DateTime(date.Year, 1, 1);
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime CalendarYearStart(this DateTime date)
		{
			return new DateTime(date.Year, 1, 1);
		}


		/// <summary>
		/// return new DateTime(date.Year, 12, 31);
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime CalendarYearEnd(this DateTime date)
		{
			return new DateTime(date.Year, 12, 31);
		}


		/// <summary>
		/// return date.AddYears(yearsToAdd).AddDays(-1);
		/// </summary>
		/// <param name="date"></param>
		/// <param name="yearsToAdd"></param>
		/// <returns></returns>
		public static DateTime EndInYears(this DateTime date, int yearsToAdd = 1)
		{
			return date.AddYears(yearsToAdd).AddDays(-1);
		}


		/// <summary>
		/// Get the most recent day of the week prior to given date (i.e. 'get me the most recent Thursday')
		/// </summary>
		/// <param name="date"></param>
		/// <param name="dayOfWeek"></param>
		/// <returns></returns>
		public static DateTime MostRecentXthDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
		{
			var dt = date;
			while (dt.DayOfWeek != dayOfWeek) dt = dt.AddDays(-1);
			return dt;
		}

	}
}
