using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.ScreenSpaces
{
	public class PixelScreenSpaceTests : TestWithMocksOrVisually
	{
		[Test]
		public void SquareWindowWithPixelSpace()
		{
			Window.ViewportPixelSize = new Size(100, 100);
			var screen = new PixelScreenSpace(Window);
			Assert.AreEqual(Point.Zero, screen.TopLeft);
			Assert.AreEqual(Window.ViewportPixelSize, (Size)screen.BottomRight);
			Assert.AreEqual(new Rectangle(Point.Zero, Window.TotalPixelSize), screen.Viewport);
			Assert.AreEqual(new Point(100, 100), screen.FromPixelSpace(new Point(100, 100)));
			Assert.AreEqual(new Rectangle(10, 10, 80, 80),
				screen.FromPixelSpace(new Rectangle(10, 10, 80, 80)));
			Window.CloseAfterFrame();
		}

		[Test]
		public void GetInnerPoint()
		{
			Window.ViewportPixelSize = new Size(800, 600);
			ScreenSpace screen = new PixelScreenSpace(Window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPoint(Point.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPoint(Point.One));
			Window.CloseAfterFrame();
		}

		[Test]
		public void ToPixelSpaceAndFromPixelSpace()
		{
			Window.ViewportPixelSize = new Size(75, 100);
			var pixelScreen = new PixelScreenSpace(Window);
			Assert.AreEqual(pixelScreen.TopLeft, pixelScreen.ToPixelSpace(pixelScreen.TopLeft));
			Assert.AreEqual(pixelScreen.BottomRight, pixelScreen.ToPixelSpace(pixelScreen.BottomRight));
			Assert.AreEqual(Size.Zero, pixelScreen.ToPixelSpace(Size.Zero));
			Assert.AreEqual(Size.One, pixelScreen.ToPixelSpace(Size.One));
			Window.CloseAfterFrame();
		}

		[Test]
		public void NonSquareWindowWithPixelSpace()
		{
			Window.ViewportPixelSize = new Size(100, 75);
			var screen = new PixelScreenSpace(Window);
			Assert.AreEqual(0.0f, screen.Left);
			Assert.AreEqual(0.0f, screen.Top);
			Assert.AreEqual(100.0f, screen.Right);
			Assert.AreEqual(75.0f, screen.Bottom);
			Assert.AreEqual(75.0f, screen.Bottom);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeWindowTotalPixelSize()
		{
			Size border = Window.TotalPixelSize - Window.ViewportPixelSize;
			Window.ViewportPixelSize = PixelSize;
			Assert.AreEqual(PixelSize, Window.ViewportPixelSize);
			Assert.AreEqual(PixelSize + border, Window.TotalPixelSize);
			Window.CloseAfterFrame();
		}

		private static readonly Size PixelSize = new Size(800, 600);

		[Test]
		public void ChangeWindowViewportPixelSize()
		{
			Size border = Window.TotalPixelSize - Window.ViewportPixelSize;
			Window.ViewportPixelSize = PixelSize;
			Assert.AreEqual(PixelSize + border, Window.TotalPixelSize);
			Assert.AreEqual(PixelSize, Window.ViewportPixelSize);
			Window.CloseAfterFrame();
		}

		[Test]
		public void MoveWindow()
		{
			Window.PixelPosition = new Point(100, 200);
			Assert.AreEqual(new Point(100, 200), Window.PixelPosition);
			Window.CloseAfterFrame();
		}
	}
}