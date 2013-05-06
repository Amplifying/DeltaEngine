using System;

namespace BowlingGame
{
	internal static class Program
	{
		private static void Main()
		{
			var game = new Game();
			for (int i = 0; i < 20;i++)
				game.Roll(1);
			Console.WriteLine("All one game results in score: "+game.CalulateScore());
		}
	}
}