using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Sprites
{
	public class AnimationTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void RenderAnimatedSprite()
		{
			new Animation("ImageAnimation", new Rectangle(0.4f, 0.4f, 0.2f, 0.2f));
		}

		[Test]
		public void CheckAnimatedTime()
		{
			var animation = new Animation("ImageAnimation",
				Rectangle.FromCenter(Point.Half, new Size(0.2f)));
			RunCode = () =>
			{
				float elapsed = animation.Elapsed;
				var currentFrame = (int)elapsed % 3.0f;
				Assert.AreEqual(currentFrame, animation.CurrentFrame);
			};
		}

		[Test]
		public void CreateAnimatedSprite()
		{
			var images = CreateImages();
			var animation = new Animation("ImageAnimation", center);
			Assert.AreEqual(images, animation.Images);
			Assert.AreEqual(images[0], animation.Image);
			Assert.AreEqual(3, animation.Duration);
			Window.CloseAfterFrame();
		}

		private static List<Image> CreateImages()
		{
			var images = new List<Image>
			{
				ContentLoader.Load<Image>("ImageAnimation01"),
				ContentLoader.Load<Image>("ImageAnimation02"),
				ContentLoader.Load<Image>("ImageAnimation03")
			};
			return images;
		}

		private readonly Rectangle center = Rectangle.FromCenter(Point.Half, new Size(0.2f, 0.2f));

		[Test]
		public void ChangeDuration()
		{
			var animation = new Animation("ImageAnimation", center) { Duration = 2 };
			Assert.AreEqual(2, animation.Duration);
			Window.CloseAfterFrame();
		}

		[Test]
		public void ResetZerosCurrentFrameAndElapsed()
		{
			var animation = new Animation("ImageAnimation", center);
			RunCode = () =>
			{
				animation.Reset();
				Assert.AreEqual(0, animation.CurrentFrame);
				Assert.AreEqual(0, animation.Elapsed);
			};
			Window.CloseAfterFrame();
		}

		[Test]
		public void RenderThreeFrameAnimationButResetAfterSecondFrame()
		{
			Animation animation = new Animation("ImageAnimation", center);
			RunCode = () =>
			{
				if (animation.CurrentFrame != 2)
					return;

				animation.Reset();
				Assert.AreEqual(0, animation.CurrentFrame);
				Assert.AreEqual(0, animation.Elapsed);
			};
		}

		[Test]
		public void TryToLoadFromInvalidContentMetaDataThrows()
		{
			Assert.Throws<AnimationData.NoImagesForAnimationPresent>(
				() => new Animation("UnavailableImageAnimation", center));
			Window.CloseAfterFrame();
		}

		[Test]
		public void AdvancingTillLastFrameGivesEvent()
		{
			var animation = new Animation("ImageAnimation", center);
			bool endReached = false;
			animation.FinalFrame += () => { endReached = true; };
			resolver.AdvanceTimeAndExecuteRunners(animation.Duration);
			Assert.IsTrue(endReached);
		}

		[Test]
		public void FramesWillNotAdvanceIfIsPlayingFalse()
		{
			var animation = new Animation("ImageAnimation", center);
			bool endReached = false;
			animation.FinalFrame += () => { endReached = true; };
			animation.IsPlaying = false;
			resolver.AdvanceTimeAndExecuteRunners(animation.Duration);
			Assert.IsFalse(endReached);
		}
	}
}