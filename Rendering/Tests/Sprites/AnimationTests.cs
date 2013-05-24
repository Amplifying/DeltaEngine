using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Sprites
{
	public class AnimationTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void RenderAnimatedSprite(Type resolver)
		{
			Animation animation = null;
			Start(resolver,
				(ContentLoader content) =>
				{ animation = new Animation(CreateImages(content), Center, 1.5f); }, () =>
				{
					float elapsed = animation.Get<Animation.Data>().Elapsed;
					var currentFrame = (int)((2 * (elapsed % 1.5f)));
					Assert.AreEqual(currentFrame, animation.CurrentFrame);
				});
		}

		private static List<Image> CreateImages(ContentLoader content)
		{
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
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var images = CreateImages(content);
				var animation = new Animation(images, Center, 3);
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
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var images1 = new List<Image> { content.Load<Image>("ImageAnimation01") };
				var images2 = CreateImages(content);
				var animation = new Animation(images1, Center, 3) { Images = images2 };
				Assert.AreEqual(images2, animation.Images);
			});
		}

		[Test]
		public void ChangeDuration()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var animation = new Animation(CreateImages(content), Center, 1) { Duration = 2 };
				Assert.AreEqual(2, animation.Duration);
			});
		}

		[Test]
		public void AddImageWithoutChangingOverallAnimationDuration()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var images = new List<Image> { content.Load<Image>("ImageAnimation01") };
				var animation = new Animation(images, Center, 1);
				VerifyImageCountAndAnimationDuration(animation, 1, 1);
				animation.AddImageWithoutIncreasingDuration(content.Load<Image>("ImageAnimation02"));
				VerifyImageCountAndAnimationDuration(animation, 2, 1);
			});
		}

		private static void VerifyImageCountAndAnimationDuration(Animation animation, int imageCount,
			float animationDuration)
		{
			Assert.AreEqual(imageCount, animation.Images.Count);
			Assert.AreEqual(animationDuration, animation.Duration);
		}

		[Test]
		public void AddImageIncreasingOverallAnimationDuration()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var images = new List<Image> { content.Load<Image>("ImageAnimation01") };
				var animation = new Animation(images, Center, 1);
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
			Animation animation = null;
			Start(typeof(MockResolver),
				(ContentLoader content) => { animation = new Animation(CreateImages(content), Center, 3); },
				() =>
				{
					animation.Reset();
					Assert.AreEqual(0, animation.CurrentFrame);
					Assert.AreEqual(0, animation.Get<Animation.Data>().Elapsed);
				});
		}

		[VisualTest]
		public void RenderThreeFrameAnimationButResetAfterSecondFrame(Type resolver)
		{
			Animation animation = null;
			Start(resolver,
				(ContentLoader content) => { animation = new Animation(CreateImages(content), Center, 3); },
				() =>
				{
					if (animation.Get<Animation.Data>().CurrentFrame != 2)
						return;

					animation.Reset();
					Assert.AreEqual(0, animation.CurrentFrame);
					Assert.AreEqual(0, animation.Get<Animation.Data>().Elapsed);
				});
		}


		[Test]
		public void LoadAnimationByNameMock() 
		{
			Start(typeof(MockResolver),
				(ContentLoader content) =>
				{
					var animation = new Animation("ImageAnimation", content,
						Rectangle.FromCenter(Point.Half, new Size(0.2f)), 3);
				});
		}

		[VisualTest]
		public void LoadAnimationByNameRealContent(Type resolverType)
		{
			Start(resolverType,
				(ContentLoader content) =>
				{
					var animation = new Animation("ImageAnimation", content,
						Rectangle.FromCenter(Point.Half, new Size(0.2f)), 3);
				});
		}
	}
}