using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Graphics.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new DrawingTests().DrawVertices(TestStarter.Xna);
			new DrawingTests().ShowRedLine(TestStarter.OpenGL);
		}
	}
}