using DeltaEngine.Datatypes;
using NUnit.Framework;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Rendering.Tests
{
	public class RendererTests
	{
		private static readonly Rectangle HalfScreenRect = new Rectangle(Point.Zero, Size.Half);

		[Test]
		public void AddDrawSetColorRemove()
		{
			ColoredRectangle box = null;
			TestAppOnce.Start((Renderer r) =>
			{
				box = new ColoredRectangle(r, HalfScreenRect, Color.Red);
				r.Add(box);
				r.DrawRectangle(box.Rect, Color.Green);
				r.Remove(box);
			});
			box.Dispose();
		}
	}
}