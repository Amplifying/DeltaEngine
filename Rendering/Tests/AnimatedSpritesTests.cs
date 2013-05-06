using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class AnimatedSpritesTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void RenderAnimatedSprite(Type resolver)
		{
			AnimatedSprite animation = null;
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				animation = new AnimatedSprite(CreateImages(content), Center, 1.5f);
				entitySystem.Add(animation);
			}, () =>
			{
				float elapsed = animation.Get<SpriteAnimationData>().Elapsed;
				var currentFrame = (int)((2 * (elapsed % 1.5f)));
				Assert.AreEqual(currentFrame, animation.CurrentFrame);
			});
		}

		private static List<Image> CreateImages(ContentLoader content)
		{
			//TODO: load simply from animation parent content entry
			var images = new List<Image>
			{
				content.Load<Image>("ImageAnimation01"),
				content.Load<Image>("ImageAnimation02"),
				content.Load<Image>("ImageAnimation03")
			};
			return images;
		}

		[Test]
		public void CreateAnimatedSprite()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var images = CreateImages(content);
				var animation = new AnimatedSprite(images, Center, 3);
				Assert.AreEqual(images, animation.Images);
				Assert.AreEqual(images[0], animation.Image);
				Assert.AreEqual(3, animation.Duration);
			});
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(Point.Half,
			new Size(0.2f, 0.2f));

		[Test]
		public void ChangeImages()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var images1 = new List<Image> { content.Load<Image>("ImageAnimation01") };
				var images2 = CreateImages(content);
				var animation = new AnimatedSprite(images1, Center, 3) { Images = images2 };
				Assert.AreEqual(images2, animation.Images);
			});
		}

		[Test]
		public void ChangeDuration()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var animation = new AnimatedSprite(CreateImages(content), Center, 1) { Duration = 2 };
				Assert.AreEqual(2, animation.Duration);
			});
		}

		[Test]
		public void AddImageWithoutChangingOverallAnimationDuration()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var images = new List<Image> { content.Load<Image>("ImageAnimation01") };
				var animation = new AnimatedSprite(images, Center, 1);
				VerifyImageCountAndAnimationDuration(animation, 1, 1);
				animation.AddImageWithoutIncreasingDuration(content.Load<Image>("ImageAnimation02"));
				VerifyImageCountAndAnimationDuration(animation, 2, 1);
			});
		}

		private static void VerifyImageCountAndAnimationDuration(AnimatedSprite animation,
			int imageCount, float animationDuration)
		{
			Assert.AreEqual(imageCount, animation.Images.Count);
			Assert.AreEqual(animationDuration, animation.Duration);
		}

		[Test]
		public void AddImageIncreasingOverallAnimationDuration()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				var images = new List<Image> { content.Load<Image>("ImageAnimation01") };
				var animation = new AnimatedSprite(images, Center, 1);
				VerifyImageCountAndAnimationDuration(animation, 1, 1);
				animation.AddImageIncreasingDuration(content.Load<Image>("ImageAnimation02"));
				VerifyImageCountAndAnimationDuration(animation, 2, 2);
				animation.AddImageIncreasingDuration(content.Load<Image>("ImageAnimation03"));
				VerifyImageCountAndAnimationDuration(animation, 3, 3);
			});
		}

		[Test]
		public void ResetZerosCurrentFrameAndElapsed()
		{
			AnimatedSprite animation = null;
			Start(typeof(MockResolver), (EntitySystem entitySystem, ContentLoader content) =>
			{
				animation = new AnimatedSprite(CreateImages(content), Center, 3);
				entitySystem.Add(animation);
			}, () =>
			{
				animation.Reset();
				Assert.AreEqual(0, animation.CurrentFrame);
				Assert.AreEqual(0, animation.Get<SpriteAnimationData>().Elapsed);
			});
		}

		[VisualTest]
		public void RenderThreeFrameAnimationButResetAfterSecondFrame(Type resolver)
		{
			AnimatedSprite animation = null;
			Start(resolver, (EntitySystem entitySystem, ContentLoader content) =>
			{
				animation = new AnimatedSprite(CreateImages(content), Center, 3);
				entitySystem.Add(animation);
			}, () =>
			{
				if (animation.Get<SpriteAnimationData>().CurrentFrame != 2)
					return;

				animation.Reset();
				Assert.AreEqual(0, animation.CurrentFrame);
				Assert.AreEqual(0, animation.Get<SpriteAnimationData>().Elapsed);
			});
		}
	}
}