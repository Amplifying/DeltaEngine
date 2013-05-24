using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.All
{
	public class OrientationTest : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void TestDrawAreaWhenChangingOrientationToPortrait(Type type)
		{
			Start(type, () => { new Ellipse(new Rectangle(0.7f, 0.7f, 0.1f, 0.1f), Color.Red); },
				(Window testWindow, ScreenSpace screen) =>
				{
					testWindow.TotalPixelSize = new Size(480, 800);
					var quadSize = screen.FromPixelSpace(new Point(0, 0));
					Assert.IsTrue(quadSize.X.IsNearlyEqual(0.2f));
					Assert.AreEqual(0, quadSize.Y);
					quadSize = screen.FromPixelSpace(new Point(480, 800));
					Assert.AreEqual(new Point(0.8f, 1), quadSize);
					var pixelSize = screen.ToPixelSpace(new Point(0.2f, 0));
					Assert.AreEqual(Point.Zero, pixelSize);
					pixelSize = screen.ToPixelSpace(new Point(0.8f, 1));
					Assert.AreEqual(new Point(480, 800), pixelSize);
				});
		}

		[IntegrationTest]
		public void TestDrawAreaWhenChangingOrientationToLandscape(Type type)
		{
			Start(type, () => { new Ellipse(new Rectangle(0.7f, 0.7f, 0.1f, 0.1f), Color.Red); },
				(Window testWindow, ScreenSpace screen) =>
				{
					testWindow.TotalPixelSize = new Size(800, 480);
					var quadSize = screen.FromPixelSpace(new Point(0, 0));
					Assert.AreEqual(new Point(0f, 0.2f), quadSize);
					quadSize = screen.FromPixelSpace(new Point(800, 480));
					Assert.AreEqual(new Point(1, 0.8f), quadSize);
					var pixelSize = screen.ToPixelSpace(new Point(0f, 0.2f));
					Assert.AreEqual(Point.Zero, pixelSize);
					pixelSize = screen.ToPixelSpace(new Point(1, 0.8f));
					Assert.AreEqual(new Point(800, 480), pixelSize);
				});
		}

		[VisualTest]
		public void ChangeOrientaion(Type type)
		{
			Line2D line = null;
			Start(type, (Window window, InputCommands inputCommands) =>
			{
				window.BackgroundColor = Color.Blue;
				line = new Line2D(Point.Zero, Point.One, Color.Green);
				inputCommands.Add(Key.A, () =>
				{
					//ncrunch: no coverage
					window.TotalPixelSize = new Size(800, 480); //ncrunch: no coverage
				});
				inputCommands.Add(Key.B, () =>
				{
					//ncrunch: no coverage
					window.TotalPixelSize = new Size(480, 800); //ncrunch: no coverage
				});
			}, (ScreenSpace screen, Window window) =>
			{
				var startPosition = screen.Viewport.TopLeft;
				var endPosition = screen.Viewport.BottomRight;
				window.Title = "Size: " + window.ViewportPixelSize + " Start: " + startPosition + " End: " +
					endPosition;
				line.Start = startPosition;
				line.End = endPosition;
			});
		}
	}
}