using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Rendering.Tests
{
	/// <summary>
	/// Visual tests to see if the Line2D works.
	/// </summary>
	public class Line2DTests : TestStarter
	{
		[VisualTest]
		public void DrawLine(Type resolver)
		{
			Start(resolver, (Renderer r) => r.Add(new Line2D(Point.Zero, Point.One, Color.Red)));
		}

		[VisualTest]
		public void DrawInPixelSpace(Type resolver)
		{
			var line = new Line2D(Point.Zero, Point.Zero, Color.Red);
			Start(resolver, (Renderer r, Window window) =>
			{
				window.BackgroundColor = Color.Black;
				r.Add(line);
			}, (ScreenSpace screen) =>
			{
				line.StartPosition = screen.FromPixelSpace(Point.Zero);
				line.EndPosition = screen.FromPixelSpace(new Point(800, 600));
			});
		}
	}
}