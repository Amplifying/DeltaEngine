using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.ScreenSpaces
{
	public class QuadraticScreenSpaceTests
	{
		[Test]
		public void SquareWindowWithQuadraticSpace()
		{
			SquareWindowShouldAlwaysReturnRelativeValues(new QuadraticScreenSpace(window));
		}

		private readonly Window window = new MockResolver().rendering.Window;

		private void SquareWindowShouldAlwaysReturnRelativeValues(ScreenSpace screen)
		{
			window.TotalPixelSize = new Size(100, 100);
			Assert.AreEqual(Point.Zero, screen.TopLeft);
			Assert.AreEqual(Point.One, screen.BottomRight);
			Assert.AreEqual(Rectangle.One, screen.Viewport);
			Assert.AreEqual(Point.One, screen.FromPixelSpace(new Point(100, 100)));
			Assert.AreEqual(Point.Half, screen.FromPixelSpace(new Point(50, 50)));
			Assert.AreEqual(new Rectangle(0.1f, 0.1f, 0.8f, 0.8f),
				screen.FromPixelSpace(new Rectangle(10, 10, 80, 80)));
		}

		[Test]
		public void ToQuadraticWithUnevenSize()
		{
			window.TotalPixelSize = new Size(99, 199);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Point(0.2512563f, 0), screen.TopLeft);
			Assert.AreEqual(new Point(0.7487437f, 1), screen.BottomRight);
			Assert.AreEqual(screen.BottomRight, screen.FromPixelSpace(new Point(99, 199)));
		}

		[Test]
		public void ToQuadraticWithNonSquareWindow()
		{
			window.TotalPixelSize = new Size(100, 75);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(0, screen.Left);
			Assert.AreEqual(0.125f, screen.Top);
			Assert.AreEqual(1, screen.Right);
			Assert.AreEqual(0.875f, screen.Bottom);
			Assert.AreEqual(new Rectangle(0, 0.125f, 1, 0.75f), screen.Viewport);
			Assert.AreEqual(new Point(1f, 0.875f), screen.FromPixelSpace(new Point(100, 75)));
			Assert.AreEqual(Point.Half, screen.FromPixelSpace(new Point(50, 37.5f)));
			Assert.AreEqual(new Size(0.1f, 0.1f), screen.FromPixelSpace(new Size(10, 10)));
		}

		[Test]
		public void ToQuadraticWithPortraitWindow()
		{
			window.TotalPixelSize = new Size(75, 100);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Point(0.125f, 0), screen.TopLeft);
			Assert.AreEqual(new Point(0.875f, 1), screen.BottomRight);
			Assert.AreEqual(new Rectangle(0.125f, 0, 0.75f, 1), screen.Viewport);
			Assert.AreEqual(new Point(0.875f, 1f), screen.FromPixelSpace(new Point(75, 100)));
			Assert.AreEqual(Point.Half, screen.FromPixelSpace(new Point(37.5f, 50)));
			Assert.AreEqual(new Size(0.1f, 0.1f), screen.FromPixelSpace(new Size(10, 10)));
		}

		[Test]
		public void ToPixelWithSquareWindow()
		{
			window.TotalPixelSize = new Size(100, 100);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Point(100, 100), screen.ToPixelSpace(Point.One));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(Point.Zero));
			Assert.AreEqual(new Point(50, 50), screen.ToPixelSpace(Point.Half));
		}

		[Test]
		public void ToPixelWithUnevenSizeFromQuadraticSpace()
		{
			window.TotalPixelSize = new Size(99, 199);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Point(149, 199), screen.ToPixelSpace(Point.One));
			Assert.AreEqual(new Point(-50, 0), screen.ToPixelSpace(Point.Zero));
			Assert.AreEqual(new Point(49.5f, 99.5f), screen.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Point(50, 100), screen.ToPixelSpaceRounded(Point.Half));
			Assert.AreEqual(new Point(199, 199),
				screen.ToPixelSpaceRounded(Point.One) - screen.ToPixelSpaceRounded(Point.Zero));
		}

		[Test]
		public void ToPixelWithNonSquareWindow()
		{
			window.TotalPixelSize = new Size(100, 75);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Point(100, 75), screen.ToPixelSpace(new Point(1f, 0.875f)));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(new Point(0, 0.125f)));
			Assert.AreEqual(new Point(50, 37.5f), screen.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Size(10, 20), screen.ToPixelSpace(new Size(0.1f, 0.2f)));
			Assert.AreEqual(new Rectangle(20, 7.5f, 60, 60),
				screen.ToPixelSpace(new Rectangle(0.2f, 0.2f, 0.6f, 0.6f)));
		}

		[Test]
		public void ToPixelWithPortraitWindow()
		{
			window.TotalPixelSize = new Size(75, 100);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Point(75, 100), screen.ToPixelSpace(new Point(0.875f, 1f)));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(new Point(0.125f, 0)));
			Assert.AreEqual(new Point(37.5f, 50), screen.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Size(10, 20), screen.ToPixelSpace(new Size(0.1f, 0.2f)));
		}

		[Test]
		public void GetInnerPoint()
		{
			window.TotalPixelSize = new Size(800, 600);
			ScreenSpace screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPoint(Point.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPoint(Point.One));
		}

		[Test]
		public void TestViewportSizeChanged()
		{
			window.TotalPixelSize = new Size(800, 800);
			var screen = new QuadraticScreenSpace(window);
			Action checkSize = delegate { Assert.AreEqual(Rectangle.One, screen.Viewport); };
			screen.ViewportSizeChanged += checkSize;
			window.TotalPixelSize = new Size(800, 800);
			screen.ViewportSizeChanged -= checkSize;
		}
	}
}