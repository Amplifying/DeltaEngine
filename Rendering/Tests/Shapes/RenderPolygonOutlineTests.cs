using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RenderPolygonOutlineTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderEllipseOutline(Type resolver)
		{
			Start(resolver, () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add(new OutlineColor(Color.Red));
				ellipse.Add<Polygon.RenderOutline>();
			});
		}

		[IntegrationTest]
		public void RenderingPolygonWithNoPointsDoesNotError(Type resolver)
		{
			Start(resolver, () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				var points = ellipse.Get<List<Point>>();
				points.Clear();
				ellipse.Remove<Ellipse.UpdatePoints>();
				ellipse.Add(new OutlineColor(Color.Red));
				ellipse.Add<Polygon.RenderOutline>();
			});
		}

		[Test]
		public void CheckDrawsWith1Polygon()
		{
			Start(typeof(MockResolver), () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add(new OutlineColor(Color.Red));
				ellipse.Add<Polygon.RenderOutline>();
			});
			Assert.AreEqual(2, mockResolver.rendering.NumberOfTimesDrawn);
		}

		[Test]
		public void CheckDrawsWith2Polygon()
		{
			Start(typeof(MockResolver), () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add(new OutlineColor(Color.Red));
				ellipse.Add<Polygon.RenderOutline>();
				ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add(new OutlineColor(Color.Red));
				ellipse.Add<Polygon.RenderOutline>();
			});
			Assert.AreEqual(4, mockResolver.rendering.NumberOfTimesDrawn);
		}
	}
}