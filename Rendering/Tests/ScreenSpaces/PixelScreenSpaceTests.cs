using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.ScreenSpaces
{
	public class PixelScreenSpaceTests
	{
		private readonly Window window = new MockResolver().rendering.Window;

		[Test]
		public void SquareWindowWithPixelSpace()
		{
			window.TotalPixelSize = new Size(100, 100);
			var screen = new PixelScreenSpace(window);
			Assert.AreEqual(Point.Zero, screen.TopLeft);
			Assert.AreEqual(window.ViewportPixelSize, (Size)screen.BottomRight);
			Assert.AreEqual(new Rectangle(Point.Zero, window.TotalPixelSize), screen.Viewport);
			Assert.AreEqual(new Point(100, 100), screen.FromPixelSpace(new Point(100, 100)));
			Assert.AreEqual(new Rectangle(10, 10, 80, 80),
				screen.FromPixelSpace(new Rectangle(10, 10, 80, 80)));
		}

		[Test]
		public void GetInnerPoint()
		{
			window.TotalPixelSize = new Size(800, 600);
			ScreenSpace screen = new PixelScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPoint(Point.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPoint(Point.One));
		}

		[Test]
		public void ToPixelSpaceAndFromPixelSpace()
		{
			window.TotalPixelSize = new Size(75, 100);
			var pixelScreen = new PixelScreenSpace(window);
			Assert.AreEqual(pixelScreen.TopLeft, pixelScreen.ToPixelSpace(pixelScreen.TopLeft));
			Assert.AreEqual(pixelScreen.BottomRight, pixelScreen.ToPixelSpace(pixelScreen.BottomRight));
			Assert.AreEqual(Size.Zero, pixelScreen.ToPixelSpace(Size.Zero));
			Assert.AreEqual(Size.One, pixelScreen.ToPixelSpace(Size.One));
		}

		[Test]
		public void NonSquareWindowWithPixelSpace()
		{
			window.TotalPixelSize = new Size(100, 75);
			var screen = new PixelScreenSpace(window);
			Assert.AreEqual(0.0f, screen.Left);
			Assert.AreEqual(0.0f, screen.Top);
			Assert.AreEqual(100.0f, screen.Right);
			Assert.AreEqual(75.0f, screen.Bottom);
		}
	}
}