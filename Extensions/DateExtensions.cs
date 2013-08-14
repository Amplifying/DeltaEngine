using System;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Allows to write out date values as structured iso date strings.
	/// </summary>
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

		public static DateTime Parse(string dateString)
		{
			if (dateString.Contains(" "))
				return GetDateTimeFromString(dateString);
			try
			{
				return TryParseDate(dateString);
			}
			catch
			{
				return DateTime.Now;
			}
		}

		private static DateTime TryParseDate(string dateString)
		{
			DateTime returnDate = DateTime.MinValue;
			string[] parts = dateString.SplitAndTrim('.');
			if (parts.Length >= 3)
				returnDate = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[1]),
					Convert.ToInt32(parts[0]));
			else if (parts.Length == 1)
			{
				parts = dateString.SplitAndTrim('-');
				if (parts.Length >= 3)
					returnDate = new DateTime(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]),
						Convert.ToInt32(parts[2]));
				else
				{
					parts = dateString.SplitAndTrim('/');
					if (parts.Length >= 3)
						returnDate = new DateTime(Convert.ToInt32(parts[2]), Convert.ToInt32(parts[0]),
							Convert.ToInt32(parts[1]));
					else
						returnDate = new DateTime(Convert.ToInt32(parts[0]), 1, 1);
				}
			}
			return returnDate;
		}

		private static DateTime GetDateTimeFromString(string dateTimeString)
		{
			if (dateTimeString.Trim().Length == 0)
				return DateTime.Now;
			try
			{
				return TryParseDateTime(dateTimeString);
			}
			catch
			{
				DateTime date;
				DateTime.TryParse(dateTimeString, out date);
				return date;
			}
		}

		private static DateTime TryParseDateTime(string dateTimeString)
		{
			string[] dateAndTime = dateTimeString.Split(new[] { ' ', ':' });
			DateTime date = Parse(dateAndTime[0]);
			if (dateAndTime.Length == 5)
			{
				if (dateAndTime[4].Contains("PM"))
					date = ConvertToTime(date, dateAndTime, 12);
			}
			else if (dateAndTime.Length >= 3)
				date = ConvertToTime(date, dateAndTime);
			return date;
		}

		private static DateTime ConvertToTime(DateTime date, string[] dateAndTime, int addForPm = 0)
		{
			date = date.AddHours(Convert.ToInt32(dateAndTime[1]) + Convert.ToInt32(addForPm));
			date = date.AddMinutes(Convert.ToInt32(dateAndTime[2]));
			if (dateAndTime.Length >= 4)
				date = date.AddSeconds(Convert.ToInt32(dateAndTime[3].Replace("Z", "")));
			return date;
		}

		public static bool IsDateNewer(DateTime newerDate, DateTime olderDate)
		{
			return newerDate.CompareTo(olderDate) > 0;
		}
	}
}