using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Sprites
{
	/// <summary>
	/// Tests for the Spritesheet-based animation
	/// </summary>
	public class SpriteSheetTests : TestWithMocksOrVisually
	{
		[Test]
		public void CheckDurationFromMetaData()
		{
			var animation = new SpriteSheetAnimation("SpriteSheetAnimation", center);
			Assert.AreEqual(5, animation.Duration);
			Window.CloseAfterFrame();
		}

		private readonly Rectangle center = Rectangle.FromCenter(Point.Half, new Size(0.2f, 0.2f));

		[Test]
		public void RenderAnimatedSprite()
		{
			new SpriteSheetAnimation("SpriteSheetAnimation", new Rectangle(0.4f, 0.4f, 0.2f, 0.2f));
		}

		[Test]
		public void TryToLoadAnimationFromUnavailableContentMetaDataThrows()
		{
			Assert.Throws<SpriteSheetData.WrongNumberOfChildrenForSpriteSheet>(
				() => new SpriteSheetAnimation("UnavailableAnimation", center));
			Window.CloseAfterFrame();
		}

		[Test]
		public void AdvancingTillLastFrameGivesEvent()
		{
			var animation = new SpriteSheetAnimation("SpriteSheetAnimation", new Rectangle(0.4f, 0.4f, 0.2f, 0.2f));
			bool endReached = false;
			animation.FinalFrame += () => { endReached = true; };
			resolver.AdvanceTimeAndExecuteRunners(animation.Duration);
			Assert.IsTrue(endReached);
		}

		[Test]
		public void FramesWillNotAdvanceIfIsPlayingFalse()
		{
			var animation = new SpriteSheetAnimation("SpriteSheetAnimation", new Rectangle(0.4f, 0.4f, 0.2f, 0.2f));
			bool endReached = false;
			animation.FinalFrame += () => { endReached = true; };
			animation.IsPlaying = false;
			resolver.AdvanceTimeAndExecuteRunners(animation.Duration);
			Assert.IsFalse(endReached);
		}
	}
}