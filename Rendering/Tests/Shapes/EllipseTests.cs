using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;
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
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f) { Color = Color.Red };
				entitySystem.Add(ellipse.Add<RenderPolygon>());
			});
		}

		[VisualTest]
		public void RenderRedOutlinedEllipse(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f) { OutlineColor = Color.Red };
				entitySystem.Add(ellipse.Add<RenderPolygonOutline>());
			});
		}

		[VisualTest]
		public void RenderingWithEntityHandlersInAnyOrder(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var ellipse1 = new Ellipse(Point.Half, 0.4f, 0.2f)
				{
					Color = Color.Blue,
					OutlineColor = Color.Red,
					RenderLayer = 0
				};
				entitySystem.Add(ellipse1.Add<RenderPolygon, RenderPolygonOutline>());
				var ellipse2 = new Ellipse(Point.Half, 0.1f, 0.2f)
				{
					Color = Color.Green,
					OutlineColor = Color.Purple,
					RenderLayer = 1
				};
				entitySystem.Add(ellipse2.Add<RenderPolygonOutline, RenderPolygon>());
			});
		}
	}
}