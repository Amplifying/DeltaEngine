using DeltaEngine.Platforms.All;

namespace DeltaEngine.Graphics.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new DrawingTests().ShowRedLine(TestWithAllFrameworks.OpenGL);
			//new DrawingTests().DrawVertices(TestStarterWithAllFrameworks.Xna);
			new ImageTests().DrawImage(TestWithAllFrameworks.DirectX9);
		}
	}
}