using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class ScreenSpaceTests : TestStarter
	{
		public ScreenSpaceTests()
		{
			var resolver = new TestResolver();
			window = resolver.Resolve<Window>();
		}

		private readonly Window window;

		[Test]
		public void ToQuadraticWithSquareWindow()
		{
			window.TotalPixelSize = new Size(100, 100);
			Assert.AreEqual(new Size(100, 100), window.TotalPixelSize);
			var screen = new ScreenSpace(window);
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
		public void ToQuadraticWithUnevenSize()
		{
			window.TotalPixelSize = new Size(99, 199);
			Assert.AreEqual(new Size(99, 199), window.TotalPixelSize);
			var screen = new ScreenSpace(window);
			Assert.AreEqual(new Point(0.2512563f, 0), screen.TopLeft);
			Assert.AreEqual(new Point(0.7487437f, 1), screen.BottomRight);
			Assert.AreEqual(screen.BottomRight, screen.ToQuadraticSpace(new Point(99, 199)));
		}

		[Test]
		public void ToQuadraticWithNonSquareWindow()
		{
			window.TotalPixelSize = new Size(100, 75);
			var screen = new ScreenSpace(window);
			Assert.AreEqual(0, screen.Left);
			Assert.AreEqual(0.125f, screen.Top);
			Assert.AreEqual(1, screen.Right);
			Assert.AreEqual(0.875f, screen.Bottom);
			Assert.AreEqual(new Size(1, 0.75f), screen.Area);
			Assert.AreEqual(new Rectangle(0, 0.125f, 1, 0.75f), screen.Viewport);
			Assert.AreEqual(new Point(1f, 0.875f), screen.ToQuadraticSpace(new Point(100, 75)));
			Assert.AreEqual(Point.Half, screen.ToQuadraticSpace(new Point(50, 37.5f)));
			Assert.AreEqual(new Size(0.1f, 0.1f), screen.ToQuadraticSpace(new Size(10, 10)));
		}

		[Test]
		public void ToQuadraticWithPortraitWindow()
		{
			window.TotalPixelSize = new Size(75, 100);
			var screen = new ScreenSpace(window);
			Assert.AreEqual(new Point(0.125f, 0), screen.TopLeft);
			Assert.AreEqual(new Point(0.875f, 1), screen.BottomRight);
			Assert.AreEqual(new Size(0.75f, 1), screen.Area);
			Assert.AreEqual(new Rectangle(0.125f, 0, 0.75f, 1), screen.Viewport);
			Assert.AreEqual(new Point(0.875f, 1f), screen.ToQuadraticSpace(new Point(75, 100)));
			Assert.AreEqual(Point.Half, screen.ToQuadraticSpace(new Point(37.5f, 50)));
			Assert.AreEqual(new Size(0.1f, 0.1f), screen.ToQuadraticSpace(new Size(10, 10)));
		}

		[Test]
		public void ToPixelWithSquareWindow()
		{
			window.TotalPixelSize = new Size(100, 100);
			var screen = new ScreenSpace(window);
			Assert.AreEqual(new Point(100, 100), screen.ToPixelSpace(Point.One));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(Point.Zero));
			Assert.AreEqual(new Point(50, 50), screen.ToPixelSpace(Point.Half));
		}

		[Test]
		public void ToPixelWithUnevenSize()
		{
			window.TotalPixelSize = new Size(99, 199);
			var screen = new ScreenSpace(window);
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
			var screen = new ScreenSpace(window);
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
			var screen = new ScreenSpace(window);
			Assert.AreEqual(new Point(75, 100), screen.ToPixelSpace(new Point(0.875f, 1f)));
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(new Point(0.125f, 0)));
			Assert.AreEqual(new Point(37.5f, 50), screen.ToPixelSpace(Point.Half));
			Assert.AreEqual(new Size(10, 20), screen.ToPixelSpace(new Size(0.1f, 0.2f)));
		}

		[Test]
		public void TestScreenSpaceViewportPixelSize()
		{
			window.TotalPixelSize = new Size(800, 600);
			var screen = new ScreenSpace(window);
			Assert.AreEqual(screen.ViewportPixelSize, window.ViewportPixelSize);
		}

		[Test]
		public void GetInnerPoint()
		{
			window.TotalPixelSize = new Size(800, 600);
			var screen = new ScreenSpace(window);
			Assert.AreEqual(Point.Half, screen.GetInnerPoint(0.5f, 0.5f));
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPoint(Point.Zero));
		}

		[Test]
		public void TestViewportSizeChanged()
		{
			window.TotalPixelSize = new Size(800, 800);
			var screen = new ScreenSpace(window);
			Action checkSize = delegate { Assert.AreEqual(Rectangle.One, screen.Viewport); };
			screen.ViewportSizeChanged += checkSize;
			window.TotalPixelSize = new Size(800, 800);
			screen.ViewportSizeChanged -= checkSize;
		}

		[IntegrationTest]
		public void TestDrawArea(Type type)
		{
			var rect = new ColoredRectangle(new Point(0.7f, 0.7f), new Size(0.1f, 0.1f), Color.Red);
			Start(type, (Renderer testRenderer) => testRenderer.Add(rect),
				(Window testWindow, ScreenSpace screen) =>
				{
					testWindow.TotalPixelSize = new Size(480, 800);
					var quadSize = screen.ToQuadraticSpace(new Size(480, 800));
					Assert.AreEqual(new Size(0.6f, 1), quadSize);
				});
		}

		[VisualTest]
		public void ChangeOrientaion(Type type)
		{
			var center = Point.Half;
			var size = new Size(0.9f, 0.9f);
			Start(type, (Window windowToOrient, Input.Input input, Renderer renderer) =>
			{
				var rect = new ColoredRectangle(center, size, Color.Red);
				var line = new Line2D(Point.Zero, Point.One, Color.Yellow);
				renderer.Add(rect);
				renderer.Add(line);
				input.Add(Key.A, () => { windowToOrient.TotalPixelSize = new Size(800, 480); });
				input.Add(Key.B, () => { windowToOrient.TotalPixelSize = new Size(480, 800); });
			});
		}
	}
}