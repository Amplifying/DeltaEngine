using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Brick
	/// </summary>
	public class BrickTests : TestStarter
	{
		[Test]
		public void Constructor()
		{
			var brick = new Brick(null, Point.Half);
			Assert.AreEqual(Point.Half, brick.Offset);
		}

		[Test]
		public void Constants()
		{
			Assert.AreEqual(new Point(0.38f, 0.385f), Brick.RenderOffset);
			Assert.AreEqual(0.02f, Brick.RenderZoom);
		}

		[Test]
		public void Offset()
		{
			var brick = new Brick(null, Point.Zero) { Offset = Point.Half };
			Assert.AreEqual(Point.Half, brick.Offset);
		}

		[Test]
		public void TopLeft()
		{
			var brick = new Brick(null, Point.Zero) { TopLeft = Point.Half };
			Assert.AreEqual(Point.Half, brick.TopLeft);
		}

		[Test]
		public void Position()
		{
			var brick = new Brick(null, new Point(0.1f, 0.2f))
			{
				TopLeft = new Point(0.4f, 0.8f)
			};
			Assert.AreEqual(new Point(0.5f, 1.0f), brick.Position);
		}

		[VisualTest]
		public void RenderBrick(Type resolver)
		{
			Start(resolver, (Renderer renderer, Content content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				renderer.Add(new Brick(image, new Point(5, 5)));
			});
		}
	}
}