using DeltaEngine.Platforms;

namespace DeltaEngine.Graphics.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new DrawingTests().ShowRedLine(TestWithAllFrameworks.OpenGL);
			//new DrawingTests().DrawVertices(TestStarterWithAllFrameworks.Xna);
			//new ImageTests().DrawImage(TestWithAllFrameworks.SlimDX);
			var tests =new MeshTests();
			tests.InitializeResolver();
			tests.DrawRotatingIceTower();
			tests.RunTestAndDisposeResolverWhenDone();
		}
	}
}