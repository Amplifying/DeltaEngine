using DeltaEngine.Core;

namespace Breakout
{
	/// <summary>
	/// Renders the background, ball, level and score; Also handles starting new levels
	/// </summary>
	public class Game
	{
		public Game(BallInLevel ball, Score score)
		{
			this.ball = ball;
			score.GameOver += ball.Dispose;
			Score = score;
		}

		private readonly BallInLevel ball;
		public Score Score { get; private set; }
	}
}