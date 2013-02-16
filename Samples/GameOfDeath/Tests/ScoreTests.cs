using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;

namespace GameOfDeath.Tests
{
	class ScoreTests : TestStarter
	{
		[VisualTest]
		public void ShowNumbers(Type resolver)
		{
			Start(resolver, (Score numbers) => { });
		}

		[VisualTest]
		public void ShowZeroDiggit(Type resolver)
		{
			Start(resolver, (Score numbers) => { },
				(Score numbers) => numbers.DrawDiggit(Point.Half, 0));
		}

		[VisualTest]
		public void ShowZeroDollars(Type resolver)
		{
			Start(resolver, (Score numbers) => { }, (Score numbers, Renderer renderer) =>
			{
				numbers.ShowDollars(Point.Half, 0);
				renderer.DrawLine(Point.Half, Point.One, Color.Red);
			});
		}

		[VisualTest]
		public void ShowTwentyFiveDollars(Type resolver)
		{
			Start(resolver, (Score numbers) => { },
				(Score numbers) => numbers.ShowDollars(Point.Half, 25));
		}

		[VisualTest]
		public void ShowElevenKills(Type resolver)
		{
			Start(resolver, (Score numbers) => { },
				(Score numbers) => numbers.ShowKills(Point.Half, 11));
		}
	}
}