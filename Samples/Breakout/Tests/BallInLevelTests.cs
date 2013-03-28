using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BallInLevelTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (BallInLevel ball) => { });
		}

		[VisualTest]
		public void AdvanceInLevelAfterDestroyingAllBricks(Type type)
		{
			Start(type, (BallInLevel ball, Level level) =>
			{
				level.GetBrickAt(0.25f, 0.125f).Dispose();
				level.GetBrickAt(0.75f, 0.125f).Dispose();
				level.GetBrickAt(0.25f, 0.375f).Dispose();
				level.GetBrickAt(0.75f, 0.375f).Dispose();
			}, (Level level) =>
			{
				if (level.BricksLeft == 0)
					level.InitializeNextLevel();
			});
		}

		[Test]
		public void FireBall()
		{
			var resolver = new TestResolver();
			var ball = resolver.Resolve<BallInLevel>();
			Assert.IsTrue(ball.IsVisible);
			resolver.Run();
			var initialBallPosition = new Point(0.5f, 0.86f);
			Assert.AreEqual(initialBallPosition, ball.Position);
			resolver.SetKeyboardState(Key.Space, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.AreNotEqual(initialBallPosition, ball.Position);
		}

		[VisualTest]
		public void PlayGameWithGravity(Type type)
		{
			Start(type, (Paddle paddle, BallWithGravity ball) => { });
		}

		private class BallWithGravity : BallInLevel
		{
			public BallWithGravity(Paddle paddle, Content content, InputCommands inputCommands,
				Level level)
				: base(paddle, content, inputCommands, level) {}

			protected override void Render(Renderer renderer, Time time)
			{
				var gravity = new Point(0.0f, 9.81f);
				velocity += gravity * 0.15f * time.CurrentDelta;
				base.Render(renderer, time);
			}
		}
	}
}