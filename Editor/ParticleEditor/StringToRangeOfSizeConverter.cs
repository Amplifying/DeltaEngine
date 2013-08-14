using System;
using System.Globalization;
using System.Windows.Data;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.ParticleEditor
{
	internal class StringToRangeOfSizeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return
				((value is Range<Size> ? (Range<Size>)value : new Range<Size>()).Start.Width.ToString(
					CultureInfo.InvariantCulture) + ", " +
					(value is Range<Size> ? (Range<Size>)value : new Range<Size>()).End.Width.ToString(
						CultureInfo.InvariantCulture));
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			var components = (value as string).SplitAndTrim(new[] { ',', ' ' });
			foreach (string component in components)
				if (!CheckIfStringIsANumber(component))
					return null;
			if (components.Length != 2)
				return null;
			return new Range<Size>(new Size(float.Parse(components[0])),
				new Size(float.Parse(components[1])));
		}

		private static bool CheckIfStringIsANumber(string component)
		{
			if (component.Equals("-0") || component.EndsWith("."))
				return false;
			float num;
			return float.TryParse(component, NumberStyles.Any, CultureInfo.InvariantCulture, out num);
		}
	}
}