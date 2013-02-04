using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Graphics.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new DrawingTests().ShowRedLine(TestStarter.OpenGL);
		}
	}
}