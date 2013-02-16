using System;
using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class GameTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Game game) => { });
		}

		[VisualTest]
		public void CreateBreakoutGameManually(Type type)
		{
			Start(type, (Paddle paddle, AutofacResolver app) =>
			{
				app.Resolve<Ball>();
				app.Resolve<Level>();
				app.Resolve<Background>();
				app.Resolve<InputCommands>().Add(Key.Escape, app.Close);
			});
		}

		[VisualTest]
		public void RemoveBallIfGameIsOver(Type type)
		{
			Start(type, (Game game, Score score) =>
			{
				Assert.IsFalse(score.IsGameOver);
				score.LostLive();
				score.LostLive();
				score.LostLive();
				Assert.IsTrue(score.IsGameOver);
			});
		}

		[VisualTest]
		public void UpdateScore(Type type)
		{
			Start(type, (Game game, Window window) =>
			{
				if (testResolver != null)
				{
					testResolver.AdvanceTimeAndExecuteRunners(0.2f);
					Assert.IsTrue(window.Title.Contains("Breakout Level:"));
				}
			});
		}

		[IntegrationTest]
		public void KillingAllBricksShouldAdvanceToNextLevel(Type type)
		{
			Score remScore = null;
			Start(type, (Game game, Level level, Score score) =>
			{
				remScore = score;
				Assert.AreEqual(1, score.Level);
				for (float x = 0; x < 1.0f; x += 0.1f)
					for (float y = 0; y < 1.0f; y += 0.1f)
						if (level.GetBrickAt(x, y) != null)
							level.GetBrickAt(x, y).Dispose();

				Assert.AreEqual(0, level.BricksLeft);
			});
			if (remScore != null)
			{
				Assert.AreEqual(2, remScore.Level);
				Assert.IsFalse(remScore.IsGameOver);
			}
		}

		[VisualTest]
		public void ResurrectBricksRandomly(Type type)
		{
			Start(type, (Game game, LevelWithRessurrection level) => { }, () =>
			{
				if (testResolver != null)
				{
					testResolver.SetKeyboardState(Key.Space, State.Pressing);
					var level = testResolver.Resolve<LevelWithRessurrection>();
					level.GetBrickAt(0.25f, 0.25f).Dispose();
					level.GetBrickAt(0.75f, 0.25f).Dispose();
					level.GetBrickAt(0.25f, 0.45f).Dispose();
					testResolver.AdvanceTimeAndExecuteRunners(2.0f);
				}
			});
		}
	}

	public class LevelWithRessurrection : Level
	{
		public LevelWithRessurrection(Content content, Renderer renderer, Score score, Time time)
			: base(content, renderer, score)
		{
			this.time = time;
		}

		private readonly Time time;

		public override Sprite GetBrickAt(float x, float y)
		{
			if (time.CheckEvery(2))
			{
				var random = new PseudoRandom();
				var brick = bricks[random.Get(0, rows), random.Get(0, columns)];
				if (brick != null && !brick.IsVisible)
				{
					brick.IsVisible = true;
					renderer.Add(brick);
				}
			}

			return base.GetBrickAt(x, y);
		}
	}
}