using System;

namespace DeltaEngine.Core
{
	public static class DateExtensions
	{
		public static string GetIsoDateTime(this DateTime dateTime)
		{
			return GetIsoDate(dateTime) + " " + GetIsoTime(dateTime);
		}

		public static string GetIsoDate(this DateTime date)
		{
			return date.Year + "-" + date.Month.ToString("00") + "-" + date.Day.ToString("00");
		}

		public static string GetIsoTime(this DateTime time)
		{
			return time.Hour.ToString("00") + ":" + time.Minute.ToString("00") + ":" +
				time.Second.ToString("00");
		}
	}
}