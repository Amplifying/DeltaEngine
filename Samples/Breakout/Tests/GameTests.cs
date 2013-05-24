using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class GameTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Paddle paddle, RelativeScreenSpace screen, Game game) => {});
		}

		[VisualTest]
		public void RemoveBallIfGameIsOver(Type type)
		{
			Start(type, (Game game, Score score) =>
			{
				bool isGameOver = false;
				score.GameOver += () => isGameOver = true;
				score.LifeLost();
				score.LifeLost();
				score.LifeLost();
				Assert.IsTrue(isGameOver);
			});
		}

		[VisualTest]
		public void UpdateScore(Type type)
		{
			Start(type, (Game game, UI ui, Window window) =>
			{
				if (mockResolver != null)
					mockResolver.AdvanceTimeAndExecuteRunners(0.2f);

				Assert.IsTrue(window.Title.Contains("Breakout Level:"));
			});
		}

		[Test]
		public void KillingAllBricksShouldAdvanceToNextLevel()
		{
			Score remScore = null;
			bool isGameOver = false;
			Start(typeof(MockResolver), (Game game, Level level, Score score) =>
			{
				remScore = score;
				remScore.GameOver += () => isGameOver = true;
				Assert.AreEqual(1, score.Level);
				DisposeAllBricks(level);
				Assert.AreEqual(0, level.BricksLeft);
				game.Run();
			});

			Assert.AreEqual(2, remScore.Level);
			Assert.IsFalse(isGameOver);
		}

		private static void DisposeAllBricks(Level level)
		{
			for (float x = 0; x < 1.0f; x += 0.1f)
				for (float y = 0; y < 1.0f; y += 0.1f)
					if (level.GetBrickAt(x, y) != null)
						level.GetBrickAt(x, y).Visibility = Visibility.Hide;
		}

		[IntegrationTest]
		public void GoFullscreen(Type resolver)
		{
			Start(resolver, (RelativeScreenSpace screen, Game game, Window window) =>
			{
				var fullscreenResolution = new Size(1920, 1080);
				window.SetFullscreen(fullscreenResolution);
				Assert.IsTrue(window.IsFullscreen);
				Assert.AreEqual(fullscreenResolution, window.TotalPixelSize);
			});
		}
	}
}