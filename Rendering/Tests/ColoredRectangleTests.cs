using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class ColoredRectangleTests
	{
		private static readonly Rectangle HalfScreenRect = new Rectangle(Point.Zero, Size.Half);

		[Test]
		public void CreateAndDispose()
		{
			ColoredRectangle box = null;
			TestAppOnce.Start((Renderer r) => box = new ColoredRectangle(r, HalfScreenRect, Color.Red));
			Assert.AreEqual(Color.Red, box.Color);
			box.Dispose();
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void Draw()
		{
			App.Start((Renderer r) => new ColoredRectangle(r, HalfScreenRect, Color.Red));
		}

		[Test, Ignore]
		public void Show()
		{
			Draw();
		}
	}
}