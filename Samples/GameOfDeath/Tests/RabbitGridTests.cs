using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using GameOfDeath.Items;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	class RabbitGridTests : TestStarter
	{
		[VisualTest]
		public void PlayGame(Type resolver)
		{
			Start(resolver, (UI bg, RabbitGrid rabbits) => { });
		}

		[VisualTest]
		public void InitializeField(Type resolver)
		{
			Start(resolver, (UI bg, RabbitGrid rabbits) => rabbits[1, 1] = true);
		}

		[VisualTest]
		public void IterateBoard(Type resolver)
		{
			Start(resolver, (UI bg, RabbitGrid rabbits, Time time) =>
			{
				rabbits[1, 1] = true;
				Assert.IsTrue(rabbits.ShouldSurvive(time, 1, 1));
				if (testResolver != null)
					testResolver.AdvanceTimeAndExecuteRunners(5.0f);
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

				if (testResolver == null)
					return;

				testResolver.AdvanceTimeAndExecuteRunners(0.05f);
				rabbits.DoDamage(Point.Half, 0.05f, Mallet.DefaultDamage);
				testResolver.AdvanceTimeAndExecuteRunners(0.05f);
				rabbits.DoDamage(Point.Half, 0.1f, 50);
				testResolver.AdvanceTimeAndExecuteRunners(0.05f);
				Assert.IsTrue(rabbits.IsOverPopulated());
			});
		}
	}
}