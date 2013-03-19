using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class InvertedScreenSpaceTests
	{
		public InvertedScreenSpaceTests()
		{
			var resolver = new TestResolver();
			window = resolver.Resolve<Window>();
		}

		private readonly Window window;

		[Test]
		public void SquareWindowWithInvertedSpace()
		{
			window.TotalPixelSize = new Size(100, 100);
			var screen = new InvertedScreenSpace(window);
			Assert.AreEqual(Point.UnitY, screen.TopLeft);
			Assert.AreEqual(Point.UnitX, screen.BottomRight);
			Assert.AreEqual(new Rectangle(Point.UnitY, Size.One), screen.Viewport);
			Assert.AreEqual(Point.UnitX, screen.FromPixelSpace(new Point(100, 100)));
			Assert.AreEqual(new Rectangle(0.1f, 0.9f, 0.8f, 0.8f),
				screen.FromPixelSpace(new Rectangle(10, 10, 80, 80)));
		}

		[Test]
		public void GetInnerPoint()
		{
			window.TotalPixelSize = new Size(800, 600);
			ScreenSpace screen = new InvertedScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPoint(Point.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPoint(Point.One));
		}

		[Test]
		public void ToPixelSpaceAndFromInvertedSpace()
		{
			window.TotalPixelSize = new Size(75, 100);
			var screen = new InvertedScreenSpace(window);
			Assert.AreEqual(Point.Zero, screen.ToPixelSpace(screen.TopLeft));
			Assert.AreEqual(new Point(75, 100), screen.ToPixelSpace(screen.BottomRight));
			Assert.AreEqual(Size.Zero, screen.ToPixelSpace(Size.Zero));
			Assert.AreEqual(new Size(75, 100), screen.ToPixelSpace(Size.One));
		}

		[Test]
		public void NonSquareWindowWithInvertedSpace()
		{
			window.TotalPixelSize = new Size(100, 75);
			var screen = new InvertedScreenSpace(window);
			Assert.AreEqual(0.0f, screen.Left);
			Assert.AreEqual(1.0f, screen.Top);
			Assert.AreEqual(1.0f, screen.Right);
			Assert.AreEqual(0.0f, screen.Bottom);
		}
	}

	/// <summary>
	/// Converts to and from Inverted space. https://deltaengine.fogbugz.com/default.asp?W101
	/// (0, 0) == BottomLeft, (1, 1) == TopRight
	/// </summary>
	internal class InvertedScreenSpace : ScreenSpace
	{
		public InvertedScreenSpace(Window window)
			: base(window)
		{
			pixelToRelativeScale = 1.0f / window.ViewportPixelSize;
		}

		private Size pixelToRelativeScale;

		protected override void Update(Size newViewportSize)
		{
			base.Update(newViewportSize);
			pixelToRelativeScale = 1.0f / viewportPixelSize;
		}

		public override Point ToPixelSpace(Point currentScreenSpacePos)
		{
			Point point = Point.Zero;
			point.X = currentScreenSpacePos.X * viewportPixelSize.Width;
			point.Y = (1 - currentScreenSpacePos.Y) * viewportPixelSize.Height;
			return point;
		}

		public override Size ToPixelSpace(Size currentScreenSpaceSize)
		{
			return currentScreenSpaceSize * viewportPixelSize;
		}

		public override Point FromPixelSpace(Point pixelPosition)
		{
			Point point = Point.Zero;
			point.X = pixelPosition.X * pixelToRelativeScale.Width;
			point.Y = 1 - pixelPosition.Y * pixelToRelativeScale.Height;
			return point;
		}

		public override Size FromPixelSpace(Size pixelSize)
		{
			return pixelSize * pixelToRelativeScale;
		}

		public override Point TopLeft
		{
			get { return Point.UnitY; }
		}
		public override Point BottomRight
		{
			get { return Point.UnitX; }
		}
		public override float Left
		{
			get { return 0.0f; }
		}
		public override float Top
		{
			get { return 1.0f; }
		}
		public override float Right
		{
			get { return 1.0f; }
		}
		public override float Bottom
		{
			get { return 0.0f; }
		}
		protected override Size GetSize
		{
			get { return Size.One; }
		}
		public override Point GetInnerPoint(Point relativePoint)
		{
			Point point = Point.Zero;
			point.X = relativePoint.X;
			point.Y = 1 - relativePoint.Y;
			return point;
		}
	}
}