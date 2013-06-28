using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class ScoreboardTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowTwentyFiveDollarsAndElevenKills()
		{
			new FilledRect(Rectangle.One, Color.White);
			var numbers = Resolve<Scoreboard>();
			numbers.Money = 25;
			numbers.Kills = 11;
		}
	}
}