using System.Drawing;

namespace DeltaEngine.Editor.Emulator
{
	public struct Device
	{
		public string Type;
		public string Name;
		public string ImageFile;
		public Point ScreenPoint;
		public Size ScreenSize;
		public bool CanRotate;
		public bool CanScale;
		public int DefaultScaleIndex;
	}
}