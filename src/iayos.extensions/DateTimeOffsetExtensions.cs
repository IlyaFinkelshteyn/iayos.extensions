using System;

namespace iayos.extensions
{
	public static class DateTimeOffsetExtensions
	{

		/// <summary>
		/// return new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(1);
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTimeOffset FirstOfNextMonth(this DateTimeOffset date)
		{
			return new DateTimeOffset(date.Year, date.Month, 1, 0, 0, 0, TimeSpan.Zero).AddMonths(1);
		}


		/// <summary>
		/// return FirstOfNextMonth(date).AddDays(-1);
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTimeOffset EndOfMonth(this DateTimeOffset date)
		{
			return FirstOfNextMonth(date).AddDays(-1);
		}


		/// <summary>
		/// Get the most recent day of the week prior to given date (i.e. 'get me the most recent Thursday')
		/// </summary>
		/// <param name="date"></param>
		/// <param name="dayOfWeek"></param>
		/// <returns></returns>
		public static DateTimeOffset MostRecentXDayOfWeek(this DateTimeOffset date, DayOfWeek dayOfWeek)
		{
			var dt = date;
			while (dt.DayOfWeek != dayOfWeek) dt = dt.AddDays(-1);
			return dt;
		}

	}
}
