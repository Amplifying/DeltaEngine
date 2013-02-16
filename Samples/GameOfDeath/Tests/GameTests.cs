using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using GameOfDeath.Items;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	class GameTests : TestStarter
	{
		[VisualTest]
		public void PlayGame(Type resolver)
		{
			Start(resolver, (UI bg, Game game) => { });
		}

		[VisualTest]
		public void InitializeField(Type resolver)
		{
			Start(resolver, (UI bg, Game game) => game[1, 1] = true);
		}

		[VisualTest]
		public void IterateBoard(Type resolver)
		{
			Start(resolver, (UI bg, Game game) =>
			{
				game[1, 1] = true;
				Assert.IsTrue(game.ShouldSurvive(1, 1));
				if (testResolver != null)
					testResolver.AdvanceTimeAndExecuteRunners(5.0f);
			});
		}

		[VisualTest]
		public void SimulateGameOver(Type resolver)
		{
			Start(resolver, (UI bg, Game game) =>
			{
				Assert.IsFalse(game.IsGameOver());
				for (int x = 0; x < game.width; x++)
					for (int y = 0; y < game.height; y++)
						game[x, y] = true;
				if (testResolver != null)
				{
					testResolver.AdvanceTimeAndExecuteRunners(0.05f);
					game.DoDamage(Point.Half, 0.05f, Mallet.DefaultDamage);
					testResolver.AdvanceTimeAndExecuteRunners(0.05f);
					game.DoDamage(Point.Half, 0.25f, 50);
					Assert.IsTrue(game.IsGameOver());
				}
			});
		}
	}
}