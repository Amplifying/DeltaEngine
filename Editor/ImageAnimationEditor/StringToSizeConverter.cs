using System;
using System.Globalization;
using System.Windows.Data;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.ImageAnimationEditor
{
	public class StringToSizeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value is Size ? (Size)value : new Size()).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			var components = (value as string).SplitAndTrim(new[] { ',', ' ' });
			foreach (string component in components)
				if (CheckIfStringIsANumber(component))
					return null;
			if (components.Length != 2)
				return null;
			return new Size(value as string);
		}

		private static bool CheckIfStringIsANumber(string component)
		{
			float num;
			bool isNum = float.TryParse(component, out num);
			if (!isNum)
				return true;
			return false;
		}
	}
}