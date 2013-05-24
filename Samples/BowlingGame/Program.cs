using System;
using DeltaEngine.Core;

namespace BowlingGame
{
	internal static class Program
	{
		private static void Main()
		{
			var game = new Game();
			PseudoRandom randomizer = new PseudoRandom();
			for (int i = 0; i < 20;i++)
				game.Roll(1);
			Console.WriteLine("All one game results in score: "+game.CalulateScore());

			game = new Game();
			for (int i = 0; i < 12; i++)
				game.Roll(10);
			Console.WriteLine("All strikes results in score: " + game.CalulateScore());
			Console.ReadLine();
		}
	}
}