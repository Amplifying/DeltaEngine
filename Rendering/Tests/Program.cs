using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Rendering.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new ColoredRectangleTests().DrawWithRunnerClass(TestStarter.OpenGL);
			//new ColoredRectangleTests().DrawTwoBoxes(TestStarter.OpenGL);
			new Line2DTests().DrawLine(TestStarter.OpenGL);
			//new Line2DTests().DrawInPixelSpace(TestStarter.OpenGL);
			//new CircleTests().DrawLotsOfCircles(TestStarter.OpenGL);
			//new EllipseTests().DrawRotatingEllipse(TestStarter.OpenGL);
			//new EllipseTests().DrawLotsOfEllipses(TestStarter.OpenGL);
			//new CircleTests().DrawLotsOfColoredBorderlessCircles(TestStarter.OpenGL);
			//new CircleTests().DrawLotsOfColoredBorderedCircles(TestStarter.OpenGL);
			//new ColoredEllipseTests().DrawLotsOfColoredBorderlessEllipses(TestStarter.OpenGL);
			//new ColoredEllipseTests().DrawLotsOfColoredBorderedEllipses(TestStarter.OpenGL);
			//new VectorTextTests().DrawSampleText(TestStarter.OpenGL);
			//new ColoredRectangleTests().RedBoxOverLaysBlueBox(TestStarter.OpenGL);
			//new ZoomingEffectTests().RenderZoomingLogo(TestStarter.OpenGL);
			//new FallingEffectTests().RenderFallingLogo(TestStarter.OpenGL);
			//new Camera2DControlledQuadraticScreenSpaceTests().RenderPanAndZoomIntoLogo(TestStarter.OpenGL);
			//new OutlinedCircleTests().DrawLotsOfCircles(TestStarter.OpenGL);
			new RectTests().RedBoxOverLaysBlueBox(TestStarter.DirectX);
		}
	}
}