using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Tests.Shapes;
using DeltaEngine.Rendering.Tests.Sprites;

namespace DeltaEngine.Rendering.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new EllipseTests().RenderingWithEntityHandlersInAnyOrder(TestWithAllFrameworks.OpenGL);
            new EllipseTests().RenderRedEllipse(TestWithAllFrameworks.OpenGL);
		}
	}
}