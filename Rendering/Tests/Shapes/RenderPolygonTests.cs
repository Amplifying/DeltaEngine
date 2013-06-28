using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RenderPolygonTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderFilledEllipse()
		{
			var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
			ellipse.Start<Polygon.Render>();
		}

		[Test]
		public void RenderingPolygonWithNoPointsDoesNotError()
		{
			var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
			var points = ellipse.Get<List<Point>>();
			points.Clear();
			ellipse.Remove<Ellipse.UpdatePoints>();
			ellipse.Start<Polygon.Render>();
			Window.CloseAfterFrame();
		}
	}
}