using System;
using System.Collections.Generic;
using System.Globalization;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides additional and simplified string manipulation methods.
	/// </summary>
	public static class StringExtensions
	{
		public static string ToInvariantString(this float number)
		{
			return number.ToString(NumberFormatInfo.InvariantInfo);
		}

		public static string ToInvariantString(this float number, string format)
		{
			return number.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		public static T FromInvariantString<T>(this string value, T defaultValue)
		{
			try
			{
				return TryConvertInvariantString(value, defaultValue);
			}
			catch (FormatException)
		{
				return defaultValue;
			}
		}

		private static T TryConvertInvariantString<T>(string value, T defaultValue)
		{
			if (defaultValue is float)
				return (T)(Convert.ToSingle(value, CultureInfo.InvariantCulture) as object);

			return defaultValue;
		}

		public static string ToInvariantString(object someObj)
		{
			if (someObj == null)
				return "null";

			if (someObj is float)
				return ((float)someObj).ToString(NumberFormatInfo.InvariantInfo);

			if (someObj is double)
				return ((double)someObj).ToString(NumberFormatInfo.InvariantInfo);

			if (someObj is decimal)
				return ((decimal)someObj).ToString(NumberFormatInfo.InvariantInfo);

			return someObj.ToString();
		}

		public static float[] SplitIntoFloats(this string value, char[] separators = null)
		{
			if (separators == null)
				separators = new[] { ',', '(', ')' };

			string[] components = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			return SplitIntoFloats(components);
		}

		public static float[] SplitIntoFloats(this string[] components)
		{
			var floats = new float[components.Length];
			for (int i = 0; i < floats.Length; i++)
				floats[i] = components[i].FromInvariantString(0.0f);

			return floats;
		}

		public static string MaxStringLength(this string value, int maxLength)
		{
			if (String.IsNullOrEmpty(value) || value.Length <= maxLength)
				return value;

			if (maxLength < 2)
				maxLength = 2;

			return value.Substring(0, maxLength - 2).TrimEnd() + "..";
		}

		public static string[] SplitAndTrim(this string value, params char[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.None);
			return TrimAndRemoveEmptyElements(components);
		}

		private static string[] TrimAndRemoveEmptyElements(string[] values)
		{
			var nonEmptyElements = new List<string>();
			for (int i = 0; i < values.Length; i++)
			{
				string trimmedElement = values[i].Trim();
				if (trimmedElement.Length > 0)
					nonEmptyElements.Add(trimmedElement);
			}

			return nonEmptyElements.ToArray();
		}

		public static string[] SplitAndTrim(this string value, params string[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.None);
			return TrimAndRemoveEmptyElements(components);
		}

		public static bool Compare(this string value, string other)
		{
			return String.Compare(value, other, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public static bool ContainsCaseInsensitive(this string value, string searchText)
		{
			return value != null &&
				value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
		}
	}
}