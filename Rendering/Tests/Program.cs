using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Rendering.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new ColoredRectangleTests().DrawWithRunnerClass(TestStarter.OpenGL);
			//new ColoredRectangleTests().DrawTwoBoxes(TestStarter.OpenGL);
			//new Line2DTests().DrawLine(TestStarter.OpenGL);
			//new Line2DTests().DrawInPixelSpace(TestStarter.OpenGL);
			//new CircleTests().DrawLotsOfCircles(TestStarter.OpenGL);
			//new EllipseTests().DrawRotatingEllipse(TestStarter.OpenGL);
			//new EllipseTests().DrawLotsOfEllipses(TestStarter.OpenGL);
			//new ColoredCircleTests().DrawLotsOfColoredBorderlessCircles(TestStarter.OpenGL);
			//new ColoredCircleTests().DrawLotsOfColoredBorderedCircles(TestStarter.OpenGL);
			//new ColoredEllipseTests().DrawLotsOfColoredBorderlessEllipses(TestStarter.OpenGL);
			//new ColoredEllipseTests().DrawLotsOfColoredBorderedEllipses(TestStarter.OpenGL);
			new VectorTextTests().DrawSampleText(TestStarter.OpenGL);
		}
	}
}