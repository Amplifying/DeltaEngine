using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class LevelTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Level level) => { });
		}

		[VisualTest]
		public void ForceResolutionChange(Type type)
		{
			Start(type, (Level bricks, Window window) => window.TotalPixelSize = new Size(400, 600));
		}

		[VisualTest]
		public void DrawLevelTwo(Type type)
		{
			Start(type, (Level level) => level.InitializeNextLevel());
		}

		[VisualTest]
		public void DrawLevelThree(Type type)
		{
			Start(type, (Level level) =>
			{
				level.InitializeNextLevel();
				level.InitializeNextLevel();
			});
		}

		[VisualTest]
		public void DrawLevelFour(Type type)
		{
			Start(type, (Level level) =>
			{
				level.InitializeNextLevel();
				level.InitializeNextLevel();
				level.InitializeNextLevel();
			});
		}

		[VisualTest]
		public void DrawLevelFive(Type type)
		{
			Start(type, (Level level) =>
			{
				level.InitializeNextLevel();
				level.InitializeNextLevel();
				level.InitializeNextLevel();
				level.InitializeNextLevel();
			});
		}

		[Test]
		public void SwitchLevels()
		{
			var resolver = new TestResolver();
			var level = resolver.Resolve<Level>();
			var score = resolver.Resolve<Score>();
			for (int levelNum = 1; levelNum <= 5; levelNum++)
			{
				Assert.AreEqual(levelNum, score.Level);
				level.InitializeNextLevel();
			}
		}

		[IntegrationTest]
		public void GetBrickAtScreenPosition(Type type)
		{
			Start(type, (Level level) =>
			{
				Assert.Null(level.GetBrickAt(0f, 0.6f));
				Assert.Null(level.GetBrickAt(1f, 0f));
				Assert.NotNull(level.GetBrickAt(0.25f, 0.25f));
			});
		}

		[IntegrationTest]
		public void CheckEmptyLevel(Type type)
		{
			Start(type, (Paddle paddle, EmptyLevel level) =>
			{
				Assert.IsNull(level.GetBrickAt(0.25f, 0.25f));
				Assert.IsNull(level.GetBrickAt(0.5f, 0.25f));
				Assert.IsNull(level.GetBrickAt(0.75f, 0.35f));
				Assert.AreEqual(0, level.BricksLeft);
			});
		}

		[IntegrationTest]
		public void RemoveBrick(Type type)
		{
			Start(type, (Level level) =>
			{
				Assert.AreEqual(4, level.BricksLeft);
				var brick = level.GetBrickAt(0.25f, 0.25f);
				Assert.IsTrue(brick.IsVisible);
				brick.Dispose();
				Assert.IsFalse(brick.IsVisible);
				Assert.AreEqual(3, level.BricksLeft);
				Assert.IsNull(level.GetBrickAt(0.25f, 0.25f));
			});
		}

		[VisualTest]
		public void UpdateGameWithBallReleased(Type type)
		{
			TestBall remBall = null;
			Start(type, (Level level, TestBall ball) =>
			{
				remBall = ball;
				if (testResolver != null)
				{
					testResolver.SetKeyboardState(Key.Space, State.Pressing);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
				}
			});
			if (remBall != null)
				Assert.IsFalse(remBall.IsCurrentlyOnPaddle);
		}
	}
}