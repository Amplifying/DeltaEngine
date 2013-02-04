using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	internal static class Program
	{
		[Test, Ignore]
		public static void Main()
		{
			//new ColoredRectangleTests().DrawWithRunnerClass(TestStarter.OpenGL);
			new Line2DTests().DrawLine(TestStarter.OpenGL);
			//new Line2DTests().DrawInPixelSpace(TestStarter.OpenGL);
		}
	}
}