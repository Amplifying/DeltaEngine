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
				Assert.AreEqual(1, score.Level);
				score.NextLevel();
				Assert.AreEqual(2, score.Level);
				Assert.IsFalse(score.IsGameOver);
			});
		}

		[IntegrationTest]
		public void NextLevelWithLevelInitialization(Type type)
		{
			Start(type, (Level level, Score score) =>
			{
				Assert.AreEqual(1, score.Level);
				score.NextLevel();
				Assert.AreEqual(2, score.Level);
				Assert.IsFalse(score.IsGameOver);
			});
		}

		[IntegrationTest]
		public void LoseLivesUntilGameOver(Type type)
		{
			Start(type, (Score score) =>
			{
				score.LostLive();
				score.LostLive();
				score.LostLive();
				Assert.IsTrue(score.IsGameOver);
			});
		}
	}
}