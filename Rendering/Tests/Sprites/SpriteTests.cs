using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Sprites
{
	public class SpriteTests : TestWithAllFrameworks
	{
		[Test]
		public void CreateSprite()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image, screenCenter);
				Assert.AreEqual(Color.White, sprite.Color);
				Assert.AreEqual(image, sprite.Image);
				Assert.AreEqual(new Size(128, 128), sprite.Image.PixelSize);
			});
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);

		[Test]
		public void ChangeImage()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var image1 = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image1, screenCenter, Color.White);
				Assert.AreEqual(image1, sprite.Image);
				var image2 = content.Load<Image>("ImageAnimation01");
				sprite.Image = image2;
				Assert.AreEqual(image2, sprite.Image);
			});
		}

		[VisualTest]
		public void RenderSprite(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var sprite = new Sprite(image, screenCenter, Color.White);
				entitySystem.Add(sprite);
			});
		}

		[VisualTest]
		public void RenderRedSpriteOverBlue(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				entitySystem.Add(new Sprite(image, screenCenter, Color.Red) { RenderLayer = 1 });
				entitySystem.Add(new Sprite(image, screenTopLeft, Color.Blue) { RenderLayer = 0 });
			});
		}

		private readonly Rectangle screenTopLeft = Rectangle.FromCenter(new Point(0.3f, 0.3f),
			Size.Half);
	}
}