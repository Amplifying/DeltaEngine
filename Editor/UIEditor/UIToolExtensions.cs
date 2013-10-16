using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Editor.UIEditor
{
	public static class UIToolExtensions
	{
		public static List<object> GetNames()
		{
			return (from object tool in Enum.GetValues(typeof(UITool)) select tool).ToList();
		}

		public static string GetImagePath(this UITool tool)
		{
			return BaseImagePath + tool + ImageExtension;
		}

		private const string BaseImagePath = @"..\Images\UIEditor\Create";
		private const string ImageExtension = ".png";
	}
}