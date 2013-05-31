using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;

namespace GameOfDeath.Tests
{
	internal class ScoreboardTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void ShowTwentyFiveDollarsAndElevenKills(Type resolver)
		{
			Start(resolver, (Scoreboard numbers) => {}, (Scoreboard numbers) =>
			{
				new Rect(Rectangle.One, Color.White);
				numbers.Money = 25;
				numbers.Kills = 11;
			});
		}
	}
}