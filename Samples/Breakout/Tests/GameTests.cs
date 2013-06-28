using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw(Type type)
		{
			Resolve<Paddle>();
			Resolve<RelativeScreenSpace>();
			Resolve<Game>();
		}

		[Test]
		public void RemoveBallIfGameIsOver(Type type)
		{
			var score = Resolve<Score>();
			bool isGameOver = false;
			score.GameOver += () => isGameOver = true;
			score.LifeLost();
			score.LifeLost();
			score.LifeLost();
			Assert.IsTrue(isGameOver);
		}

		[Test]
		public void UpdateScore(Type type)
		{
			resolver.AdvanceTimeAndExecuteRunners(0.2f);
			Assert.IsTrue(Window.Title.Contains("Breakout Level:"));
		}

		[Test]
		public void KillingAllBricksShouldAdvanceToNextLevel()
		{
			Score remScore = null;
			bool isGameOver = false;
			var level = Resolve<Level>();
			var score = Resolve<Score>();
			remScore = score;
			remScore.GameOver += () => isGameOver = true;
			Assert.AreEqual(1, score.Level);
			DisposeAllBricks(level);
			Assert.AreEqual(0, level.BricksLeft);
			Assert.AreEqual(1, remScore.Level);
			Assert.IsFalse(isGameOver);
		}

		private static void DisposeAllBricks(Level level)
		{
			for (float x = 0; x < 1.0f; x += 0.1f)
				for (float y = 0; y < 1.0f; y += 0.1f)
					if (level.GetBrickAt(x, y) != null)
						level.GetBrickAt(x, y).Visibility = Visibility.Hide;
		}

		[Test]
		public void GoFullscreen()
		{
			Resolve<Game>();
			var fullscreenResolution = new Size(1920, 1080);
			Window.SetFullscreen(fullscreenResolution);
			Assert.IsTrue(Window.IsFullscreen);
			Assert.AreEqual(fullscreenResolution, Window.TotalPixelSize);
			Window.CloseAfterFrame();
		}
	}
}