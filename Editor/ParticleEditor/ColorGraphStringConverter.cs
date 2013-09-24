using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class ColorGraphStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is RangeGraph<Color>))
				return "";
			return (value as RangeGraph<Color>).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stringValue = value is string ? value as string : null;
			if (string.IsNullOrEmpty(stringValue))
				return null;
			var stringPartitions = stringValue.SplitAndTrim(new[] { '{', ',', ' ', '}' });
			if (stringPartitions.Length % 4 != 0 || stringPartitions.Length < 8)
				return null;
			return FillRangeFromStringPartitions(stringPartitions);
		}

		private static RangeGraph<Color> FillRangeFromStringPartitions(string[] partitions)
		{
			var colorList = new List<Color>();
			for (int i = 0; i < partitions.Length - 3; i += 4)
			{
				if (!IsInputParsable(partitions[i]) || !IsInputParsable(partitions[i + 1]) ||
					!IsInputParsable(partitions[i + 2]) || !IsInputParsable(partitions[i + 3]))
					return null;
				Color color;
				try
				{
					color = (Color)
						TryGetColor(partitions[i] + "," + partitions[i + 1] + "," + partitions[i + 2] + "," +
							partitions[i + 3]);
				}
				catch
				{
					return null;
				}
				colorList.Add(color);
			}
			return new RangeGraph<Color>(colorList);
		}

		private static object TryGetColor(string colorString)
		{
			try
			{
				return new Color(colorString);
			}
			catch (Color.InvalidNumberOfComponents)
			{
				return null;
			}
		}

		private static bool IsInputParsable(string input)
		{
			return !(input == "-0" || input.EndsWith("."));
		}
	}
}
