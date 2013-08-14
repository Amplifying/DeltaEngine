using System;
using System.Drawing;
using System.Xml.Linq;

namespace DeltaEngine.Editor.Emulator
{
	internal static class LinqXmlReader
	{
		public static string ReadStringValue(this XContainer container, string element)
		{
			var value = container.Element(element);
			return value != null ? value.Value : "";
		}

		public static Point ReadPointValue(this XContainer container, string element)
		{
			var value = container.Element(element);
			if (value == null)
				return new Point(0, 0);

			string[] pointStrings = value.Value.Split(',');
			return new Point(Int32.Parse(pointStrings[0]), Int32.Parse(pointStrings[1]));
		}

		public static Size ReadSizeValue(this XContainer container, string element)
		{
			var value = container.Element(element);
			if (value == null)
				return new Size(0, 0);

			string[] sizeStrings = value.Value.Split(',');
			return new Size(Int32.Parse(sizeStrings[0]), Int32.Parse(sizeStrings[1]));
		}

		public static int ReadIntValue(this XContainer container, string element)
		{
			var value = container.Element(element);
			return value != null ? Int32.Parse(value.Value) : 0;
		}

		public static bool ReadBoolValue(this XContainer container, string element)
		{
			return ReadIntValue(container, element) != 0;
		}
	}
}