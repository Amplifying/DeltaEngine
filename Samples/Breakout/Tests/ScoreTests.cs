using System;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class ScoreTests : TestStarter
	{
		[IntegrationTest]
		public void IncreasePoints(Type type)
		{
			Start(type, (Score score) =>
			{
				Assert.IsTrue(score.ToString().Contains("Score: 0"), score.ToString());
				score.IncreasePoints();
				Assert.IsTrue(score.ToString().Contains("Score: 2"), score.ToString());
			});
		}

		[IntegrationTest]
		public void NextLevelWithoutInitialization(Type type)
		{
			Start(type, (Score score) =>
			{
				bool isGameOver = false;
				score.GameOver += () => isGameOver = true;
				Assert.AreEqual(1, score.Level);
				score.NextLevel();
				Assert.AreEqual(2, score.Level);
				Assert.IsFalse(isGameOver);
			});
		}

		[IntegrationTest]
		public void NextLevelWithLevelInitialization(Type type)
		{
			Start(type, (Level level, Score score) =>
			{
				bool isGameOver = false;
				score.GameOver += () => isGameOver = true;
				Assert.AreEqual(1, score.Level);
				score.NextLevel();
				Assert.AreEqual(2, score.Level);
				Assert.IsFalse(isGameOver);
			});
		}

		[IntegrationTest]
		public void LoseLivesUntilGameOver(Type type)
		{
			Start(type, (Score score) =>
			{
				bool isGameOver = false;
				score.GameOver += () => isGameOver = true;
				score.LifeLost();
				score.LifeLost();
				score.LifeLost();
				Assert.IsTrue(isGameOver);
			});
		}
	}
}