using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Sprites
{
	public class SpriteTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderSprite(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{ new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White); });
		}

		[VisualTest]
		public void RenderWalkingCharacterSprite(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White); 
				new Sprite(content.Load<Image>("WalkingCharacter"), screenCenter, Color.White);
			});
		}


		[VisualTest]
		public void RenderRedSpriteOverBlue(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				new Sprite(image, screenCenter, Color.Red) { RenderLayer = 1 };
				new Sprite(image, screenTopLeft, Color.Blue) { RenderLayer = 0 };
			});
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Point.Half, Size.Half);
		private readonly Rectangle screenTopLeft = Rectangle.FromCenter(0.3f, 0.3f, 0.5f, 0.5f);

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

		[Test]
		public void SpriteWithNoImageThrowsException()
		{
			Assert.Throws<NullReferenceException>(() => new Sprite(null, Rectangle.Zero));
		}

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
		public void RenderSpriteAndLines(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				new Line2D(Point.Zero, Point.One, Color.Blue);
				var image = content.Load<Image>("DeltaEngineLogo");
				new Sprite(image, screenCenter, Color.Red);
				new Line2D(Point.UnitX, Point.UnitY, Color.Purple);
			});
		}

		[Test]
		public void CheckDrawsWith1Sprite()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
			});
			Assert.AreEqual(1, mockResolver.rendering.NumberOfTimesDrawn);
		}

		[Test]
		public void CheckDrawsWith2Sprite()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
			});
			Assert.AreEqual(2, mockResolver.rendering.NumberOfTimesDrawn);
		}

		[Test]
		public void CheckDrawsWith2DifferentSprites()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				new Sprite(content.Load<Image>("DeltaEngineLogo"), screenCenter, Color.White);
				new Sprite(content.Load<Image>("WalkingCharacter"), screenCenter, Color.White);
			});
			Assert.AreEqual(2, mockResolver.rendering.NumberOfTimesDrawn);
		}
	}
}