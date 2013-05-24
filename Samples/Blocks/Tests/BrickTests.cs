using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Brick
	/// </summary>
	public class BrickTests : TestWithAllFrameworks
	{

		public void Initialize(ScreenSpace screen,
				ContentLoader contentLoader)
		{
			displayMode = screen.Viewport.Aspect >= 1.0f
				? Blocks.Constants.DisplayMode.LandScape : Blocks.Constants.DisplayMode.Portrait;
			content = new JewelBlocksContent(contentLoader);
		}

		private Constants.DisplayMode displayMode;
		private JewelBlocksContent content;

		[IntegrationTest]
		public void Constructor(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var brick = new Brick(content.Load<Image>("Block1"), Point.Half, displayMode);
				Assert.AreEqual(Point.Half, brick.Offset);
			});
		}

		[Test]
		public void Constants()
		{
			Assert.AreEqual(new Point(0.38f, 0.385f), Brick.OffsetLandscape);
			Assert.AreEqual(0.02f, Brick.ZoomLandscape);
		}

		[IntegrationTest]
		public void Offset(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var brick = new Brick(content.Load<Image>("Block1"), Point.Zero, displayMode)
				{
					Offset = Point.Half
				};
				Assert.AreEqual(Point.Half, brick.Offset);
			});
		}

		[IntegrationTest]
		public void TopLeft(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var brick = new Brick(content.Load<Image>("Block1"), Point.Zero, displayMode)
				{
					TopLeftGridCoord = Point.Half
				};
				Assert.AreEqual(Point.Half, brick.TopLeftGridCoord);
			});
		}

		[IntegrationTest]
		public void Position(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var brick = new Brick(content.Load<Image>("Block1"), new Point(0.1f, 0.2f), displayMode)
				{
					TopLeftGridCoord = new Point(0.4f, 0.8f)
				};
				Assert.AreEqual(new Point(0.5f, 1.0f), brick.Position);
			});
		}

		[VisualTest]
		public void RenderBrick(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				var image = content.Load<Image>("DeltaEngineLogo");
				var brick = new Brick(image, new Point(5, 5), displayMode);
				brick.UpdateDrawArea();
			});
		}

		[VisualTest]
		public void RenderBrickInPortrait(Type resolver)
		{
			Start(resolver, (ScreenSpace screen, ContentLoader contentLoader) =>
			{
				Initialize(screen, contentLoader);
				screen.Window.TotalPixelSize = new Size(600, 800);
				var image = content.Load<Image>("DeltaEngineLogo");
				var brick = new Brick(image, new Point(5, 5), displayMode);
				brick.UpdateDrawArea();
			});
		}
	}
}