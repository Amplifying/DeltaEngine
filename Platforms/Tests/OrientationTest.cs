using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Devices;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class OrientationTest : TestStarter
	{
		[IntegrationTest]
		public void TestDrawAreaWhenChangingOrientation(Type type)
		{
			var rect = new ColoredRectangle(new Point(0.7f, 0.7f), new Size(0.1f, 0.1f), Color.Red);
			Start(type, (Renderer testRenderer) => testRenderer.Add(rect),
				(Window testWindow, ScreenSpace screen) =>
				{
					testWindow.TotalPixelSize = new Size(480, 800);

					var quadSize = screen.ToQuadraticSpace(new Point(0, 0));
					Assert.IsTrue(quadSize.X.IsNearlyEqual(0.2f));
					Assert.AreEqual(0, quadSize.Y);
					quadSize = screen.ToQuadraticSpace(new Point(480, 800));
					Assert.AreEqual(new Point(0.8f, 1), quadSize);

					var pixelSize = screen.ToPixelSpace(new Point(0.2f, 0));
					Assert.AreEqual(Point.Zero, pixelSize);
					pixelSize = screen.ToPixelSpace(new Point(0.8f, 1));
					Assert.AreEqual(new Point(480, 800), pixelSize);

					testWindow.TotalPixelSize = new Size(800, 480);

					quadSize = screen.ToQuadraticSpace(new Point(0, 0));
					Assert.AreEqual(new Point(0f, 0.2f), quadSize);
					quadSize = screen.ToQuadraticSpace(new Point(800, 480));
					Assert.AreEqual(new Point(1, 0.8f), quadSize);

					pixelSize = screen.ToPixelSpace(new Point(0f, 0.2f));
					Assert.AreEqual(Point.Zero, pixelSize);
					pixelSize = screen.ToPixelSpace(new Point(1, 0.8f));
					Assert.AreEqual(new Point(800, 480), pixelSize);
				});
		}

		[VisualTest]
		public void ChangeOrientaion(Type type)
		{
			var line = new Line2D(Point.Zero, Point.One, Color.Green);
			Window window = null;
			Input.InputCommands inputCommands = null;
			Renderer renderer = null;
			ScreenSpace screen = null;
			Mouse mouse = null;
			Start(type, (Resolver resolver) =>
			{
				window = resolver.Resolve<Window>();
				window.BackgroundColor = Color.Blue;
				inputCommands = resolver.Resolve<Input.InputCommands>();
				renderer = resolver.Resolve<Renderer>();
				screen = resolver.Resolve<ScreenSpace>();
				mouse = resolver.Resolve<Mouse>();
				renderer.Add(line);
				inputCommands.Add(Key.A, () => { window.TotalPixelSize = new Size(800, 480); });
				inputCommands.Add(Key.B, () => { window.TotalPixelSize = new Size(480, 800); });
			}, delegate
			{
				var startPosition = screen.Viewport.TopLeft;
				var endPosition = screen.Viewport.BottomRight;
				window.Title = "Size: " + window.ViewportPixelSize + " Start: " + startPosition + " End: " +
					endPosition;
				line.StartPosition = startPosition;
				line.EndPosition = endPosition;
			});
		}
	}
}