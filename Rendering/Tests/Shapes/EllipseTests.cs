using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class EllipseTests : TestWithAllFrameworks
	{
		[Test]
		public void ChangeRadii()
		{
			var ellipse = new Ellipse { RadiusX = 0.2f, RadiusY = 0.4f };
			Assert.AreEqual(0.2f, ellipse.RadiusX);
			Assert.AreEqual(0.4f, ellipse.RadiusY);
		}

		[Test]
		public void ChangeRadius()
		{
			var ellipse = new Ellipse { Radius = 0.2f };
			Assert.AreEqual(0.2f, ellipse.Radius);
		}

		[VisualTest]
		public void RenderRedEllipse(Type resolver)
		{
			Start(resolver, () => { new Ellipse(Point.Half, 0.4f, 0.2f, Color.Red); });
		}

		[VisualTest]
		public void RenderRedOutlinedEllipse(Type resolver)
		{
			Start(resolver,
				() =>
				{
					new Ellipse(Point.Half, 0.4f, 0.2f, Color.Red).Add(new OutlineColor(Color.Yellow)).Add
						<Polygon.RenderOutline>();
				});
		}

		[VisualTest]
		public void RenderingWithEntityHandlersInAnyOrder(Type resolver)
		{
			Start(resolver, () =>
			{
				var ellipse1 = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue) { RenderLayer = 0 };
				ellipse1.Add(new OutlineColor(Color.Red));
				ellipse1.Add<Polygon.Render, Polygon.RenderOutline>();
				var ellipse2 = new Ellipse(Point.Half, 0.1f, 0.2f, Color.Green) { RenderLayer = 1 };
				ellipse2.Add(new OutlineColor(Color.Red));
				ellipse2.Add<Polygon.RenderOutline, Polygon.Render>();
			});
		}
	}
}