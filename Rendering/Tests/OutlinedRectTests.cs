using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class OutlinedRectTests : TestStarter
	{
		[VisualTest]
		public void CreateOutlinedRectFromRectangle(Type resolver)
		{
			var halfScreenRect = new Rectangle(Point.Zero, Size.Half);
			OutlinedRect outlinedRect = null;
			Start(resolver, (Renderer r) => 
				r.Add(outlinedRect = new OutlinedRect(halfScreenRect, Color.Red)),
				() => Assert.AreEqual(Color.Red, outlinedRect.Color));
		}

		[VisualTest]
		public void CreateOutlinedRectFromPointAndSize(Type resolver)
		{
			OutlinedRect outlinedRect = null;
			Start(resolver,
				(Renderer r) => r.Add(outlinedRect = new OutlinedRect(Point.Half, Size.Half, Color.Red)),
				() => Assert.AreEqual(Color.Red, outlinedRect.Color));
		}

		[VisualTest]
		public void AddingOutlinedRectTwiceWillOnlyDisplayItOnce(Type resolver)
		{
			OutlinedRect outlinedRect = null;
			Start(resolver, (Renderer r) =>
			{
				outlinedRect = new OutlinedRect(Point.Half, Size.Half, Color.Yellow);
				r.Add(outlinedRect);
				outlinedRect.DrawArea.Center = new Point(0.6f, 0.6f);
				outlinedRect.Color = Color.Teal;
				r.Add(outlinedRect);
			}, () => Assert.AreEqual(Color.Teal, outlinedRect.Color));
		}

		[VisualTest]
		public void DrawOutlinedRectWithRunnerClass(Type resolver)
		{
			Start<BlinkingBox>(resolver);
		}

		[VisualTest]
		public void DrawTwoOutlinedRect(Type resolver)
		{
			Start(resolver, (Renderer r) =>
			{
				r.Add(new OutlinedRect(new Point(0.4f, 0.4f), Size.Half, Color.Yellow));
				r.Add(new OutlinedRect(new Point(0.5f, 0.6f), Size.Half, Color.Blue));
			});
		}
	}
}