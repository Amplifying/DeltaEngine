using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Brick
	/// </summary>
	public class BrickTests : TestWithMocksOrVisually
	{
		public void Initialize(ScreenSpace screen)
		{
			displayMode = screen.Viewport.Aspect >= 1.0f ? Orientation.Landscape : Orientation.Portrait;
			content = new JewelBlocksContent();
		}

		private Orientation displayMode;
		private JewelBlocksContent content;

		[Test]
		public void Constructor()
		{
			Initialize(Resolve<ScreenSpace>());
			var brick = new Brick(content.Load<Image>("Block1"), Point.Half, displayMode);
			Assert.AreEqual(Point.Half, brick.Offset);
		}

		[Test]
		public void Constants()
		{
			Assert.AreEqual(new Point(0.38f, 0.385f), Brick.OffsetLandscape);
			Assert.AreEqual(0.02f, Brick.ZoomLandscape);
		}

		[Test]
		public void Offset(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			var brick = new Brick(content.Load<Image>("Block1"), Point.Zero, displayMode)
			{
				Offset = Point.Half
			};
			Assert.AreEqual(Point.Half, brick.Offset);
		}

		[Test]
		public void TopLeft(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			var brick = new Brick(content.Load<Image>("Block1"), Point.Zero, displayMode)
			{
				TopLeftGridCoord = Point.Half
			};
			Assert.AreEqual(Point.Half, brick.TopLeftGridCoord);
		}

		[Test]
		public void Position(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			var brick = new Brick(content.Load<Image>("Block1"), new Point(0.1f, 0.2f), displayMode)
			{
				TopLeftGridCoord = new Point(0.4f, 0.8f)
			};
			Assert.AreEqual(new Point(0.5f, 1.0f), brick.Position);
		}

		[Test]
		public void RenderBrick(Type resolver)
		{
			Initialize(Resolve<ScreenSpace>());
			var image = content.Load<Image>("Block1");
			var brick = new Brick(image, new Point(5, 5), displayMode);
			brick.UpdateDrawArea();
		}

		[Test]
		public void RenderBrickInPortrait(Type resolver)
		{
			var screen = Resolve<ScreenSpace>();
			Initialize(screen);
			screen.Window.ViewportPixelSize = new Size(600, 800);
			var image = content.Load<Image>("Block1");
			var brick = new Brick(image, new Point(5, 5), displayMode);
			brick.UpdateDrawArea();
		}
	}
}