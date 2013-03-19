using System;
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
			Rectangle centered = Rectangle.FromCenter(Point.Half, new Size(0.2f));
			var redFrames = new[] { new Point(1.0f, 0), new Point(2.0f, 0), new Point(3.0f, 0),
				new Point(4.0f, 0) };
			var animatedSprite = new AnimatedSprite(content.Load<Image>("test"), centered, Color.Blue,
				32, 32, 4, redFrames);
			Assert.AreEqual(centered.Center, animatedSprite.DrawArea.Center);
			Assert.AreEqual(Color.Blue, animatedSprite.Color);
		}

		[VisualTest]
		public void ShowAnimatedSprite(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer, Window window) =>
			{
				var redFrames = new[] { new Point(1.0f, 0), new Point(2.0f, 0), new Point(3.0f, 0),
					new Point(4.0f, 0) };
				var red = new AnimatedSprite(content.Load<Image>("redgs"),
					Rectangle.FromCenter(0.5f, 0.5f, 0.16f, 0.16f), Color.White, 32, 32, 4, redFrames);
				renderer.Add(red);
			});
		}

		[VisualTest]
		public void RotateAroundSpriteCenterPoint(Type resolver)
		{
			AnimatedSprite sprite = null;
			Start(resolver, (Content content, Renderer renderer) =>
			{
				var redFrames = new[] { new Point(1.0f, 0), new Point(2.0f, 0), new Point(3.0f, 0),
					new Point(4.0f, 0) };
				sprite = new AnimatedSprite(content.Load<Image>("redgs"),
					Rectangle.FromCenter(0.5f, 0.5f, 0.16f, 0.16f), Color.White, 32, 32, 4, redFrames);
				renderer.Add(sprite);
			}, () => sprite.Rotation += 0.01f);
		}

		[VisualTest]
		public void OverloadingNumberSpritesPerSecond(Type resolver)
		{
			Start(resolver, (Content content, Renderer renderer, Window window) =>
			{
				var redFrames = new[] { new Point(1.0f, 0), new Point(2.0f, 0), new Point(3.0f, 0),
					new Point(4.0f, 0) };
				var red = new AnimatedSprite(content.Load<Image>("redgs"),
					Rectangle.FromCenter(0.5f, 0.5f, 0.16f, 0.16f), Color.White, 32, 32, 120, redFrames);
				renderer.Add(red);
			});
		}
	}
}
