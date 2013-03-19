using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Graphics.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new DeviceTests().DrawRedBackground(TestStarter.Xna);
			new DrawingTests().ShowRedLine(TestStarter.OpenGL);
		}
	}
}