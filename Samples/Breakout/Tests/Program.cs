using DeltaEngine.Platforms.All;

namespace Breakout.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new BackgroundTests().Draw(TestWithAllFrameworks.OpenGL);
		}
	}
}