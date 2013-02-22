using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;

namespace Breakout
{
	/// <summary>
	/// Renders the ball of this game in the level with the score and adds the background graphic.
	/// </summary>
	public class Game : Runner<Time>
	{
		public Game(Background bg, BallInLevel ball, Score score, InputCommands inputCommands,
			Window window)
		{
			bg.RenderLayer = Renderable.BackgroundRenderLayer;
			this.ball = ball;
			this.score = score;
			this.window = window;
			inputCommands.Add(Key.Escape, window.Dispose);
		}

		private readonly BallInLevel ball;
		private readonly Score score;
		private readonly Window window;

		public void Run(Time time)
		{
			StartNewLevelIfAllBricksAreDestroyed();
			RemoveBallIfGameIsOver();
			ShowScoreInWindowTitleEvery200Ms(time);
		}

		private void StartNewLevelIfAllBricksAreDestroyed()
		{
			if (ball.CurrentLevel.BricksLeft > 0)
				return;

			ball.CurrentLevel.InitializeNextLevel();
			ball.ResetBall();
		}

		private void RemoveBallIfGameIsOver()
		{
			if (!score.IsGameOver)
				return;

			ball.Dispose();
		}

		private void ShowScoreInWindowTitleEvery200Ms(Time time)
		{
			if (!time.CheckEvery(0.2f))
				return;

			window.Title = "Breakout " + score;
		}
	}
}