using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class ColoredRectangleTests : TestStarter
	{
		[VisualTest]
		public void CreateFromRectangle(Type resolver)
		{
			var halfScreenRect = new Rectangle(Point.Zero, Size.Half);
			ColoredRectangle box = null;
			Start(resolver, (Renderer r) => r.Add(box = new ColoredRectangle(halfScreenRect, Color.Red)),
				() => Assert.AreEqual(Color.Red, box.Color));
		}

		[VisualTest]
		public void CreateFromPointAndSize(Type resolver)
		{
			ColoredRectangle box = null;
			Start(resolver,
				(Renderer r) => r.Add(box = new ColoredRectangle(Point.Half, Size.Half, Color.Red)),
				() => Assert.AreEqual(Color.Red, box.Color));
		}

		[VisualTest]
		public void AdddingBoxTwiceWillOnlyDisplayItOnce(Type resolver)
		{
			ColoredRectangle box = null;
			Start(resolver, (Renderer r) =>
			{
				box = new ColoredRectangle(Point.Half, Size.Half, Color.Yellow);
				r.Add(box);
				box.DrawArea.Center = new Point(0.6f, 0.6f);
				box.Color = Color.Teal;
				r.Add(box);
			}, () => Assert.AreEqual(Color.Teal, box.Color));
		}

		[VisualTest]
		public void DrawWithRunnerClass(Type resolver)
		{
			Start<BlinkingBox>(resolver);
		}
	}
}