using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;

namespace GameOfDeath.Tests
{
	class ScoreboardTests : TestStarter
	{
		[VisualTest]
		public void ShowNumbers(Type resolver)
		{
			Start(resolver, (Scoreboard numbers) => { });
		}

		[VisualTest]
		public void ShowZeroDigit(Type resolver)
		{
			Start(resolver, (Scoreboard numbers) => { },
				(Scoreboard numbers) => numbers.DrawDigit(Point.Half, 0));
		}

		[VisualTest]
		public void ShowZeroDollars(Type resolver)
		{
			Start(resolver, (Scoreboard numbers) => { }, (Scoreboard numbers, Renderer renderer) =>
			{
				numbers.ShowDollars(Point.Half, 0);
				renderer.DrawLine(Point.Half, Point.One, Color.Red);
			});
		}

		[VisualTest]
		public void ShowTwentyFiveDollars(Type resolver)
		{
			Start(resolver, (Scoreboard numbers) => { },
				(Scoreboard numbers) => numbers.ShowDollars(Point.Half, 25));
		}

		[VisualTest]
		public void ShowElevenKills(Type resolver)
		{
			Start(resolver, (Scoreboard numbers) => { },
				(Scoreboard numbers) => numbers.ShowKills(Point.Half, 11));
		}
	}
}