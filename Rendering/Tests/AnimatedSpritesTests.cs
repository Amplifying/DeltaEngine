using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class AnimatedSpritesTests : TestStarter
	{
		[Test]
		public void CreateEmptyAnimatedSprite()
		{
			var resolver = new TestResolver();
			resolver.RegisterAllUnknownTypesAutomatically();
			var content = new Content(resolver);
			var images = new List<Image>
			{
				content.Load<Image>("test")
			};
			Rectangle centered = Rectangle.FromCenter(Point.Half, new Size(0.2f));
			var animatedSprite = new AnimatedSprite(images, centered);
			Assert.AreEqual(centered.Center, animatedSprite.DrawArea.Center);
			Assert.AreEqual(Color.White, animatedSprite.Color);
		}

		[VisualTest]
		public void ShowAnimatedSprite(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer, Window window) =>
			{
				var images = new List<Image>
				{
					content.Load<Image>("ImageAnimation01"),
					content.Load<Image>("ImageAnimation02"),
					content.Load<Image>("ImageAnimation03")
				};
				var sprites = new AnimatedSprite(images, Rectangle.FromCenter(0.5f, 0.5f, 0.16f, 0.16f));
				renderer.Add(sprites);
			});
		}

		[VisualTest]
		public void AddImage(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer, Window window) =>
			{
				var images = new List<Image>
				{
					content.Load<Image>("ImageAnimation01"),
					content.Load<Image>("ImageAnimation02")
				};
				var sprites = new AnimatedSprite(images, Rectangle.FromCenter(0.5f, 0.5f, 0.16f, 0.16f));
				sprites.AddImage(content.Load<Image>("ImageAnimation03"));
				renderer.Add(sprites);
			});
		}

		[VisualTest]
		public void RotateAroundSpriteCenterPoint(Type resolver)
		{
			AnimatedSprite sprite = null;
			Start(resolver, (Content content, Renderer renderer) =>
			{
				var images = new List<Image>
				{
					content.Load<Image>("ImageAnimation01"),
					content.Load<Image>("ImageAnimation02"),
					content.Load<Image>("ImageAnimation03")
				};
				sprite = new AnimatedSprite(images, Rectangle.FromCenter(0.5f, 0.5f, 0.16f, 0.16f));
				renderer.Add(sprite);
			}, () => sprite.Rotation += 0.01f);
		}

		[VisualTest]
		public void OverloadingNumberSpritesPerSecond(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer, Window window) =>
			{
				var images = new List<Image>
				{
					content.Load<Image>("ImageAnimation01"),
					content.Load<Image>("ImageAnimation02"),
					content.Load<Image>("ImageAnimation03")
				};
				var red = new AnimatedSprite(images, Rectangle.FromCenter(0.5f, 0.5f, 0.16f, 0.16f));
				red.SetNumberSpritesPerSecond(120);
				renderer.Add(red);
			});
		}
	}
}