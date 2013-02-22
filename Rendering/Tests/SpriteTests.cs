using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class SpriteTests : TestStarter
	{
		[Test]
		public void CreateEmptyImage()
		{
			var resolver = new TestResolver();
			resolver.RegisterAllUnknownTypesAutomatically();
			var content = new Content(resolver);
			Rectangle centered = Rectangle.FromCenter(Point.Half, new Size(0.2f));
			var newSprite = new Sprite(content.Load<Image>("test"), centered);
			Assert.IsTrue(newSprite.IsVisible);
			Assert.AreEqual(centered.Center, newSprite.DrawArea.Center);
			Assert.AreEqual(Color.White, newSprite.Color);
		}

		[VisualTest]
		public void Render(Type resolver)
		{
			Start(resolver,
				(Content content, Renderer renderer) =>
					renderer.Add(new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter)));
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);
		
		[VisualTest]
		public void RenderOnGrayBackground(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer, Window window) =>
			{
				window.BackgroundColor = Color.DarkGray;
				renderer.Add(new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter));
			});
		}

		[VisualTest]
		public void RotateAroundSpriteCenterPoint(Type resolver)
		{
			Sprite sprite = null;
			Start(resolver, (Content content, Renderer renderer) =>
			{
				sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				renderer.Add(sprite);
			}, () => sprite.Rotation += 0.01f);
		}

		[VisualTest]
		public void RotateAroundScreenCenter(Type resolver)
		{
			var sprites = new Sprite[4];
			Start(resolver, (Content content, Renderer renderer) =>
			{
				var logoImage = content.Load<Image>("DeltaEngineLogo");
				sprites[0] = new Sprite(logoImage, Rectangle.FromCenter(0.3f, 0.3f, 0.15f, 0.15f));
				sprites[1] = new Sprite(logoImage, Rectangle.FromCenter(0.3f, 0.5f, 0.15f, 0.15f));
				sprites[2] = new Sprite(logoImage, Rectangle.FromCenter(0.5f, 0.5f, 0.15f, 0.15f));
				sprites[3] = new Sprite(logoImage, Rectangle.FromCenter(0.7f, 0.5f, 0.15f, 0.15f));
				foreach (Sprite sprite in sprites)
					sprite.RotationCenter = Point.Half;
				foreach (Sprite sprite in sprites)
					renderer.Add(sprite);
			}, () =>
			{
				foreach (Sprite sprite in sprites)
					sprite.Rotation += 0.01f;
			});
		}

		[VisualTest]
		public void DrawFlipHorizontal(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				sprite.Flip = FlipMode.Horizontal;
				renderer.Add(sprite);
			});
		}

		[VisualTest]
		public void DrawFlipVertical(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer) =>
			{
				var sprite = new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter);
				sprite.Flip = FlipMode.Vertical;
				renderer.Add(sprite);
			});
		}

		[VisualTest]
		public void DrawLotsOfSpritesAndShowFps(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer) =>
			{
				Create300Sprites(content, renderer);
			}, (Time time, Window window) =>
			{
				if (time.CheckEvery(1) || resolver == typeof(TestResolver))
					window.Title = "DrawLotsOfSpritesAndShowFps: " + time.Fps;
			});
		}

		private static void Create300Sprites(Content content, Renderer renderer)
		{
			const int Columns = 20;
			const int Rows = 15;
			var screen = renderer.Screen;
			var size = new Size(screen.Viewport.Width / Columns, screen.Viewport.Height / Rows);
			var corner = screen.TopLeft;
			var logo = content.Load<Image>("DeltaEngineLogo");
			for (int x = 0; x < Columns; x++)
				for (int y = 0; y < Rows; y++)
					renderer.Add(new Sprite(logo,
						new Rectangle(new Point(corner.X + x * size.Width, corner.Y + y * size.Height), size)));
		}
	}
}