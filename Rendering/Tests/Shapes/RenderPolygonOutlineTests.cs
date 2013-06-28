using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RenderPolygonOutlineTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderEllipseOutline()
		{
			var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
			ellipse.Add(new OutlineColor(Color.Red));
			ellipse.Start<Polygon.RenderOutline>();
		}

		[Test]
		public void RenderingPolygonWithNoPointsDoesNotError()
		{
			var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
			var points = ellipse.Get<List<Point>>();
			points.Clear();
			ellipse.Remove<Ellipse.UpdatePoints>();
			ellipse.Add(new OutlineColor(Color.Red));
			ellipse.Start<Polygon.RenderOutline>();
			Window.CloseAfterFrame();
		}
	}
}