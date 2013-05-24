using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BallInLevelTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (BallInLevel ball) => {});
		}

		[VisualTest]
		public void AdvanceInLevelAfterDestroyingAllBricks(Type type)
		{
			Start(type, (BallInLevel ball, Level level) =>
			{
				level.GetBrickAt(0.25f, 0.125f).Visibility = Visibility.Hide;
				level.GetBrickAt(0.75f, 0.125f).Visibility = Visibility.Hide;
				level.GetBrickAt(0.25f, 0.375f).Visibility = Visibility.Hide;
				level.GetBrickAt(0.75f, 0.375f).Visibility = Visibility.Hide;
			}, (Level level) =>
			{
				if (level.BricksLeft == 0)
					level.InitializeNextLevel();
			});
		}

		[Test]
		public void FireBall()
		{
			Start(typeof(MockResolver), (BallInLevel ball) =>
			{
				Assert.IsTrue(ball.Visibility == Visibility.Show);
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				var initialBallPosition = new Point(0.5f, 0.86f);
				Assert.AreEqual(initialBallPosition, ball.Position);
				mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.AreNotEqual(initialBallPosition, ball.Position);
			});
		}

		[VisualTest]
		public void PlayGameWithGravity(Type type)
		{
			Start(type, (Paddle paddle, BallWithGravity ball) => {});
		}

		private class BallWithGravity : BallInLevel
		{
			public BallWithGravity(Paddle paddle, ContentLoader content, InputCommands inputCommands,
				Level level)
				: base(paddle, content, inputCommands, level) {}

			public override void Run(ScreenSpace screen)
			{
				var gravity = new Point(0.0f, 9.81f);
				velocity += gravity * 0.15f * Time.Current.Delta;
				base.Run(screen);
			}
		}
	}
}