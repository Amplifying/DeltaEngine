using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;

namespace Breakout
{
	/// <summary>
	/// Renders the ball of this game in the level with the score and adds the background graphic.
	/// </summary>
	public class Game : Runner<Time>
	{
		public Game(Background background, BallInLevel ball, Score score, InputCommands inputCommands,
			Window window)
		{
			this.ball = ball;
			this.score = score;
			score.GameOver += ball.Dispose;
			this.window = window;
			inputCommands.Add(Key.Escape, window.Dispose);
			inputCommands.Add(Key.F, () => window.SetFullscreen(new Size(1920, 1080)));
		}

		private readonly BallInLevel ball;
		private readonly Score score;
		private readonly Window window;

		public void Run(Time time)
		{
			StartNewLevelIfAllBricksAreDestroyed();
			ShowScoreInWindowTitleEvery200Ms(time);
		}

		private void StartNewLevelIfAllBricksAreDestroyed()
		{
			if (ball.Level.BricksLeft > 0)
				return;

			ball.Level.InitializeNextLevel();
			ball.ResetBall();
		}

		private void ShowScoreInWindowTitleEvery200Ms(Time time)
		{
			if (time.CheckEvery(0.2f))
				window.Title = "Breakout " + score;
		}
	}
}