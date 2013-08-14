using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class EllipseTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedEllipse()
		{
			new Ellipse(Point.Half, 0.4f, 0.2f, Color.Red);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedOutlinedEllipse()
		{
			var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Red);
			ellipse.Add(new OutlineColor(Color.Yellow));
			ellipse.OnDraw<DrawPolygon2DOutlines>();
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderingWithEntityHandlersInAnyOrder()
		{
			var ellipse1 = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Blue) { RenderLayer = 0 };
			ellipse1.Add(new OutlineColor(Color.Red));
			ellipse1.OnDraw<DrawPolygon2D>();
			ellipse1.OnDraw<DrawPolygon2DOutlines>();
			var ellipse2 = new Ellipse(Point.Half, 0.1f, 0.2f, Color.Green) { RenderLayer = 1 };
			ellipse2.Add(new OutlineColor(Color.Red));
			ellipse2.OnDraw<DrawPolygon2DOutlines>();
			ellipse2.OnDraw<DrawPolygon2D>();
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRadius()
		{
			var ellipse = new Ellipse(Rectangle.One, Color.Red) { RadiusX = 0.2f, RadiusY = 0.4f };
			Assert.AreEqual(0.2f, ellipse.RadiusX);
			Assert.AreEqual(0.4f, ellipse.RadiusY);
		}

		[Test, CloseAfterFirstFrame]
		public void BorderPointsAreCalculatedOnRunningEntitySystem()
		{
			var ellipse = new Ellipse(Rectangle.One, Color.White);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(48, ellipse.Get<List<Point>>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenEllipseDoesNotThrowException()
		{
			var ellipse = new Ellipse(Point.Half, 0.4f, 0.2f, Color.Red);
			ellipse.Add(new OutlineColor(Color.Yellow));
			ellipse.OnDraw<DrawPolygon2DOutlines>();
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}
	}
}