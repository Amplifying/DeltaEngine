using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using GameOfDeath.Items;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class RabbitGridTests : TestWithMocksOrVisually
	{
		[Test]
		public void PlayGame()
		{
			Resolve<UI>();
			Resolve<RabbitGrid>();
		}

		[Test]
		public void InitializeField()
		{
			Resolve<UI>();
			var rabbits = Resolve<RabbitGrid>();
			rabbits[1, 1] = true;
		}

		[Test]
		public void IterateBoard()
		{
			var rabbits = Resolve<RabbitGrid>();
			rabbits[1, 1] = true;
			Assert.IsTrue(rabbits.ShouldSurvive(1, 1));
			resolver.AdvanceTimeAndExecuteRunners(5.0f);
		}

		[Test]
		public void SimulateGameOver()
		{
			var rabbits = Resolve<RabbitGrid>();
			Assert.IsFalse(rabbits.IsOverPopulated());
			for (int x = 0; x < rabbits.width; x++)
				for (int y = 0; y < rabbits.height; y++)
					rabbits[x, y] = true;

			resolver.AdvanceTimeAndExecuteRunners(0.05f);
			rabbits.DoDamage(Point.Half, 0.05f, Mallet.DefaultDamage);
			resolver.AdvanceTimeAndExecuteRunners(0.05f);
			rabbits.DoDamage(Point.Half, 0.1f, 50);
			resolver.AdvanceTimeAndExecuteRunners(0.05f);
			Assert.IsTrue(rabbits.IsOverPopulated());
		}
	}
}