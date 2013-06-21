using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class RenderPolygonTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderFilledEllipse(Type resolver)
		{
			Start(resolver, () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add<Polygon.Render>();
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
				ellipse.Add<Polygon.Render>();
			});
		}

		[Test]
		public void CheckDrawsWith1Polygon()
		{
			Start(typeof(MockResolver), () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add<Polygon.Render>();
			});
			Assert.AreEqual(1, mockResolver.rendering.NumberOfTimesDrawn);
		}

		[Test]
		public void CheckDrawsWith2Polygon()
		{
			Start(typeof(MockResolver), () =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add<Polygon.Render>();
				ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue);
				ellipse.Add<Polygon.Render>();
			});
			Assert.AreEqual(2, mockResolver.rendering.NumberOfTimesDrawn);
		}
	}
}