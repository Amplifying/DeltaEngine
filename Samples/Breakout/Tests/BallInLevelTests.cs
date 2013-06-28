using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BallInLevelTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw(Type type)
		{
			Resolve<BallInLevel>();
		}

		[Test]
		public void AdvanceInLevelAfterDestroyingAllBricks(Type type)
		{
			Resolve<BallInLevel>();
			var level = Resolve<Level>();
			level.GetBrickAt(0.25f, 0.125f).Visibility = Visibility.Hide;
			level.GetBrickAt(0.75f, 0.125f).Visibility = Visibility.Hide;
			level.GetBrickAt(0.25f, 0.375f).Visibility = Visibility.Hide;
			level.GetBrickAt(0.75f, 0.375f).Visibility = Visibility.Hide;
			RunCode = () =>
			{
				if (level.BricksLeft == 0)
					level.InitializeNextLevel();
			};
		}

		[Test]
		public void FireBall()
		{
			Resolve<BallInLevel>();
			var ball = Resolve<Ball>();
			Assert.IsTrue(ball.Visibility == Visibility.Show);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			var initialBallPosition = new Point(0.5f, -0.02f);
			Assert.AreEqual(initialBallPosition, ball.Position);
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreNotEqual(initialBallPosition, ball.Position);
		}

		[Test]
		public void PlayGameWithGravity(Type type)
		{
			Resolve<Paddle>();
			Resolve<BallWithGravity>();
		}

		private class BallWithGravity : BallInLevel
		{
			public BallWithGravity(Paddle paddle, ContentLoader content, InputCommands inputCommands,
				Level level)
				: base(paddle, inputCommands, level) {}

			public class RunGravity : EventListener2D
			{
				public override void ReceiveMessage(Entity2D entity, object message)
				{
					var gravity = new Point(0.0f, 9.81f);
					velocity += gravity * 0.15f * Time.Current.Delta;
				}		
			}
		}
	}
}