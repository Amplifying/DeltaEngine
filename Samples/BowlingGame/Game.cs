using System.Collections.Generic;

namespace BowlingGame
{
	public class Game
	{
		public void Roll(int pins)
		{
			rolls.Add(pins);
		}

		private readonly List<int> rolls = new List<int>();

		public int CalulateScore()
		{
			int score = 0;
			for (int round = 0; round < MaxRounds; round++)
				score += CalculateRoundScore();

			return score;
		}

		private const int MaxRounds = 10;

		private int CalculateRoundScore()
		{
			if (IsStrike())
				return CalculateStrike();
			if (IsSpare())
				return CalculateSpare();
			return CalculateNormally();
		}

		private bool IsStrike()
		{
			return rolls[rollNumber] == 10;
		}

		private int rollNumber;

		private int CalculateStrike()
		{
			rollNumber++;
			return 10 + rolls[rollNumber] + rolls[rollNumber + 1];
		}

		private bool IsSpare()
		{
			return rolls[rollNumber] + rolls[rollNumber + 1] == 10;
		}

		private int CalculateSpare()
		{
			rollNumber += 2;
			return 10 + rolls[rollNumber];
		}

		private int CalculateNormally()
		{
			rollNumber += 2;
			return rolls[rollNumber - 2] + rolls[rollNumber - 1];
		}
	}
}