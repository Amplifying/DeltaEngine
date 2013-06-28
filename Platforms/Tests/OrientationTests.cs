using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class OrientationTests : TestWithMocksOrVisually
	{
		[Test]
		public void TestDrawAreaWhenChangingOrientationToPortrait()
		{
			new Ellipse(new Rectangle(0.7f, 0.7f, 0.1f, 0.1f), Color.Red);
			RunCode = () =>
			{
				Window.ViewportPixelSize = new Size(480, 800);
				var screen = Resolve<ScreenSpace>();
				var quadSize = screen.FromPixelSpace(new Point(0, 0));
				ArePointsNearlyEqual(new Point(0.2f, 0f), quadSize);
				quadSize = screen.FromPixelSpace(new Point(480, 800));
				ArePointsNearlyEqual(new Point(0.8f, 1), quadSize);
				var pixelSize = screen.ToPixelSpace(new Point(0.2f, 0));
				ArePointsNearlyEqual(Point.Zero, pixelSize);
				pixelSize = screen.ToPixelSpace(new Point(0.8f, 1));
				ArePointsNearlyEqual(new Point(480, 800), pixelSize);
			};
			Window.CloseAfterFrame();
		}

		private static void ArePointsNearlyEqual(Point expected, Point actual)
		{
			Assert.True(actual.X.IsNearlyEqual(expected.X));
			Assert.True(actual.Y.IsNearlyEqual(expected.Y));
		}

		[Test]
		public void TestDrawAreaWhenChangingOrientationToLandscape()
		{
			new Ellipse(new Rectangle(0.7f, 0.7f, 0.1f, 0.1f), Color.Red);
			RunCode = () =>
			{
				Window.ViewportPixelSize = new Size(800, 480);
				var screen = Resolve<ScreenSpace>();
				var quadSize = screen.FromPixelSpace(new Point(0, 0));
				ArePointsNearlyEqual(new Point(0f, 0.2f), quadSize);
				quadSize = screen.FromPixelSpace(new Point(800, 480));
				ArePointsNearlyEqual(new Point(1, 0.8f), quadSize);
				var pixelSize = screen.ToPixelSpace(new Point(0f, 0.2f));
				ArePointsNearlyEqual(Point.Zero, pixelSize);
				pixelSize = screen.ToPixelSpace(new Point(1, 0.8f));
				ArePointsNearlyEqual(new Point(800, 480), pixelSize);
			};
			Window.CloseAfterFrame();
		}

		[Test]
		public void ChangeOrientaion()
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Green);
			Window.BackgroundColor = Color.Blue;
			Input.Add(Key.A, key =>
			{ Window.ViewportPixelSize = new Size(800, 480); });//ncrunch: no coverage
			Input.Add(Key.B, key =>
			{ Window.ViewportPixelSize = new Size(480, 800); }); //ncrunch: no coverage
			RunCode = () =>
			{
				var screen = Resolve<ScreenSpace>();
				var startPosition = screen.Viewport.TopLeft;
				var endPosition = screen.Viewport.BottomRight;
				Window.Title = "Size: " + Window.ViewportPixelSize + " Start: " + startPosition +
					" End: " + endPosition;
				line.StartPoint = startPosition;
				line.EndPoint = endPosition;
			};
		}
	}
}