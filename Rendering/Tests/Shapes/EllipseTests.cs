using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class EllipseTests : TestWithMocksOrVisually
	{
		[Test]
		public void ChangeRadius()
		{
			var ellipse = new Ellipse(Rectangle.One, Color.Red) { RadiusX = 0.2f, RadiusY = 0.4f };
			Assert.AreEqual(0.2f, ellipse.RadiusX);
			Assert.AreEqual(0.4f, ellipse.RadiusY);
			Window.CloseAfterFrame();
		}

		[Test]
		public void BorderPointsAreCalculatedOnRunningEntitySystem()
		{
			var ellipse = new Ellipse(Rectangle.One, Color.White);
			EntitySystem.Current.Run();
			Assert.AreEqual(48, ellipse.Get<List<Point>>().Count);
			Window.CloseAfterFrame();
		}

		[Test]
		public void RenderRedEllipse()
		{
			new Ellipse(Point.Half, 0.4f, 0.2f, Color.Red);
		}

		[Test]
		public void RenderRedOutlinedEllipse()
		{
			new Ellipse(Point.Half, 0.4f, 0.2f, Color.Red).Add(new OutlineColor(Color.Yellow)).Start
				<Polygon.RenderOutline>();
		}

		[Test]
		public void RenderingWithEntityHandlersInAnyOrder()
		{
			var ellipse1 = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue) { RenderLayer = 0 };
			ellipse1.Add(new OutlineColor(Color.Red));
			ellipse1.Start<Polygon.Render, Polygon.RenderOutline>();
			var ellipse2 = new Ellipse(Point.Half, 0.1f, 0.2f, Color.Green) { RenderLayer = 1 };
			ellipse2.Add(new OutlineColor(Color.Red));
			ellipse2.Start<Polygon.RenderOutline, Polygon.Render>();
		}
	}
}