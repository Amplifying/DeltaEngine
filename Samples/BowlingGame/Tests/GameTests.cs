using NUnit.Framework;

namespace BowlingGame.Tests
{
	public class GameTests
	{
		[SetUp]
		public void InitGame()
		{
			game = new Game();
		}

		private Game game;

		[Test]
		public void GutterGame()
		{
			RollMany(20, 0);
			Assert.AreEqual(0, game.CalulateScore());
		}

		private void RollMany(int numberOfThrows, int pinsPerThrow)
		{
			for (int throws = 0; throws < numberOfThrows; throws++)
				game.Roll(pinsPerThrow);
		}

		[Test]
		public void AllOnes()
		{
			RollMany(20, 1);
			Assert.AreEqual(20, game.CalulateScore());
		}

		[Test]
		public void OneSpare()
		{
			RollSpare();
			game.Roll(3);
			RollMany(17, 0);
			Assert.AreEqual(10 + 3 * 2, game.CalulateScore());
		}

		private void RollSpare()
		{
			game.Roll(5);
			game.Roll(5);
		}

		[Test]
		public void OneStrike()
		{
			RollStrike();
			game.Roll(2);
			game.Roll(4);
			RollMany(16, 0);
			Assert.AreEqual(10 + (2 + 4) * 2, game.CalulateScore());
		}

		private void RollStrike()
		{
			game.Roll(10);
		}

		[Test]
		public void PerfectGame()
		{
			RollMany(12, 10);
			Assert.AreEqual(300, game.CalulateScore());
		}
	}
}