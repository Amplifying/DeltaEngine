using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using GameOfDeath.Items;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class RabbitGridTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void PlayGame(Type resolver)
		{
			Start(resolver, (UI bg, RabbitGrid rabbits) => {});
		}

		[VisualTest]
		public void InitializeField(Type resolver)
		{
			Start(resolver, (UI bg, RabbitGrid rabbits) => rabbits[1, 1] = true);
		}

		[VisualTest]
		public void IterateBoard(Type resolver)
		{
			Start(resolver, (UI bg, RabbitGrid rabbits) =>
			{
				rabbits[1, 1] = true;
				Assert.IsTrue(rabbits.ShouldSurvive(1, 1));
				if (mockResolver != null)
					mockResolver.AdvanceTimeAndExecuteRunners(5.0f);
			});
		}

		[VisualTest]
		public void SimulateGameOver(Type resolver)
		{
			Start(resolver, (UI bg, RabbitGrid rabbits) =>
			{
				Assert.IsFalse(rabbits.IsOverPopulated());
				for (int x = 0; x < rabbits.width; x++)
					for (int y = 0; y < rabbits.height; y++)
						rabbits[x, y] = true;

				if (mockResolver == null)
					return;

				mockResolver.AdvanceTimeAndExecuteRunners(0.05f);
				rabbits.DoDamage(Point.Half, 0.05f, Mallet.DefaultDamage);
				mockResolver.AdvanceTimeAndExecuteRunners(0.05f);
				rabbits.DoDamage(Point.Half, 0.1f, 50);
				mockResolver.AdvanceTimeAndExecuteRunners(0.05f);
				Assert.IsTrue(rabbits.IsOverPopulated());
			});
		}
	}
}