using System;
using System.Globalization;
using System.Windows.Data;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class StringToGraphRangePointConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return
				(value is RangeGraph<Point> ? (RangeGraph<Point>)value : new RangeGraph<Point>()).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			var components = (value as string).SplitAndTrim(new[] { ',', ' ' });
			foreach (string component in components)
				if (!CheckIfStringIsANumber(component))
					return null;
			if (components.Length != 4)
				return null;
			return
				new RangeGraph<Point>(new Point(ParseToFloat(components[0]), ParseToFloat(components[1])),
					new Point(ParseToFloat(components[2]), ParseToFloat(components[3])));
		}

		private static bool CheckIfStringIsANumber(string component)
		{
			if (component.Equals("-0") || component.EndsWith("."))
				return false;
			float num;
			return float.TryParse(component, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
		}

		private static float ParseToFloat(string component)
		{
			float num;
			float.TryParse(component, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
			return num;
		}
	}
}