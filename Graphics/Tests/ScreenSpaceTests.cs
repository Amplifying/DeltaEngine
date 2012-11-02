using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public static class ScreenSpaceTests
	{
		[Test]
		public static void ToQuadraticWithSquareWindow()
		{
			var screen = new ScreenSpace(new Size(100, 100));
			Assert.AreEqual(Point.Zero, screen.TopLeft);
			Assert.AreEqual(Point.One, screen.BottomRight);
			Assert.AreEqual(Size.One, screen.Area);
			Assert.AreEqual(Rectangle.One, screen.Viewport);
			Assert.AreEqual(Point.One, screen.ToQuadraticSpace(new Point(100, 100)));
			Assert.AreEqual(Point.Half, screen.ToQuadraticSpace(new Point(50, 50)));
			Assert.AreEqual(new Rectangle(0.1f, 0.1f, 0.8f, 0.8f),
				screen.ToQuadraticSpace(new Rectangle(10, 10, 80, 80)));
		}

		[Test]
		public static void ToQuadraticWithNonSquareWindow()
		{
			var screen = new ScreenSpace(new Size(100, 75));
			Assert.AreEqual(new Point(0, 0.125f), screen.TopLeft);
			Assert.AreEqual(new Point(1, 0.875f), screen.BottomRight);
			Assert.AreEqual(new Size(1, 0.75f), screen.Area);
			Assert.AreEqual(new Rectangle(0, 0.125f, 1, 0.75f), screen.Viewport);
			Assert.AreEqual(new Point(1f, 0.875f), screen.ToQuadraticSpace(new Point(100, 75)));
			Assert.AreEqual(Point.Half, screen.ToQuadraticSpace(new Point(50, 37.5f)));
			Assert.AreEqual(new Size(0.1f, 0.1f), screen.ToQuadraticSpace(new Size(10, 10)));
		}

		[Test]
		public static void ToQuadraticWithPortraitWindow()
		{
			var screen = new ScreenSpace(new Size(75, 100));
			Assert.AreEqual(new Point(0.125f, 0), screen.TopLeft);
			Assert.AreEqual(new Point(0.875f, 1), screen.BottomRight);
			Assert.AreEqual(new Size(0.75f, 1), screen.Area);
			Assert.AreEqual(new Rectangle(0.125f, 0, 0.75f, 1), screen.Viewport);
			Assert.AreEqual(new Point(0.875f, 1f), screen.ToQuadraticSpace(new Point(75, 100)));
			Assert.AreEqual(Point.Half, screen.ToQuadraticSpace(new Point(37.5f, 50)));
			Assert.AreEqual(new Size(0.1f, 0.1f), screen.ToQuadraticSpace(new Size(10, 10)));
		}

		[Test]
		public static void ToPixelWithSquareWindow()
		{
			var screen = new ScreenSpace(new Size(100, 100));
			Assert.AreEqual(new Point(100, 100), screen.ToPixelSpace(Point.One));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(Point.Zero));
			Assert.AreEqual(new Point(50, 50), screen.ToPixelSpace(Point.Half));
		}

		[Test]
		public static void ToPixelWithNonSquareWindow()
		{
			var screen = new ScreenSpace(new Size(100, 75));
			Assert.AreEqual(new Point(100, 75), screen.ToPixelSpace(new Point(1f, 0.875f)));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(new Point(0, 0.125f)));
			Assert.AreEqual(new Point(50, 37.5f), screen.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Size(10, 20), screen.ToPixelSpace(new Size(0.1f, 0.2f)));
			Assert.AreEqual(new Rectangle(20, 7.5f, 60, 60),
				 screen.ToPixelSpace(new Rectangle(0.2f, 0.2f, 0.6f, 0.6f)));
		}

		[Test]
		public static void ToPixelWithPortraitWindow()
		{
			var screen = new ScreenSpace(new Size(75, 100));
			Assert.AreEqual(new Point(75, 100), screen.ToPixelSpace(new Point(0.875f, 1f)));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(new Point(0.125f, 0)));
			Assert.AreEqual(new Point(37.5f, 50), screen.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Size(10, 20), screen.ToPixelSpace(new Size(0.1f, 0.2f)));
		}
	}
}