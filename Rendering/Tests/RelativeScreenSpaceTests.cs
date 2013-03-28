using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class RelativeScreenSpaceTests
	{
		public RelativeScreenSpaceTests()
		{
			var resolver = new TestResolver();
			window = resolver.Resolve<Window>();
		}

		private readonly Window window;

		[Test]
		public void SquareWindowWithRelativeSpace()
		{
			SquareWindowShouldAlwaysReturnRelativeValues(new RelativeScreenSpace(window));
		}

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
		public void GetInnerPoint()
		{
			window.TotalPixelSize = new Size(800, 600);
			ScreenSpace screen = new RelativeScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPoint(Point.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPoint(Point.One));
		}

		[Test]
		public void ToRelativeWithUnevenSize()
		{
			window.TotalPixelSize = new Size(99, 199);
			var screen = new RelativeScreenSpace(window);
			Assert.AreEqual(Point.Zero, screen.TopLeft);
			Assert.AreEqual(Point.One, screen.BottomRight);
			Assert.AreEqual(screen.BottomRight, screen.FromPixelSpace(new Point(99, 199)));
		}

		[Test]
		public void ToPixelSpaceFromRelativeSpace()
		{
			window.TotalPixelSize = new Size(30, 50);
			var screen = new RelativeScreenSpace(window);
			Assert.AreEqual(new Point(30, 50), screen.ToPixelSpace(Point.One));
			Assert.AreEqual(Size.Zero, screen.ToPixelSpace(Size.Zero));
			Assert.AreEqual(new Point(10, 20), screen.ToPixelSpace(new Point(10 / 30.0f, 20 / 50.0f)));
			Assert.AreEqual(new Size(7.5f, 12.5f), screen.ToPixelSpace(new Size(0.25f)));
		}

		[Test]
		public void NonSquareWindowWithRelativeSpace()
		{
			window.TotalPixelSize = new Size(100, 75);
			var screen = new RelativeScreenSpace(window);
			Assert.AreEqual(0.0f, screen.Left);
			Assert.AreEqual(0.0f, screen.Top);
			Assert.AreEqual(1.0f, screen.Right);
			Assert.AreEqual(1.0f, screen.Bottom);
		}
	}
}