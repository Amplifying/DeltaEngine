using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Blocks
{
	/// <summary>
	/// Overarching class controlling Blocks
	/// </summary>
	public class Game
	{
		public Game(InputCommands input, Renderer renderer, Controller controller)
		{
			this.controller = controller;
			SetControllerEvents();
			SetInputEvents(input);
			AddScore(renderer);
		}

		private readonly Controller controller;

		private void SetControllerEvents()
		{
			controller.ScorePoints += AddToScore;
			controller.Lost += Lose;
		}

		private void Lose()
		{
			scoreboard.Text = "Final Score " + Score;
			Score = 0;
		}

		private void AddToScore(int points)
		{
			Score += points;
			scoreboard.Text = "Score " + Score;
		}

		public int Score { get; set; }
		protected VectorText scoreboard;

		private void SetInputEvents(InputCommands input)
		{
			input.Add(Key.CursorLeft, State.Pressing, controller.TryToMoveBlockLeft);
			input.Add(Key.CursorRight, State.Pressing, controller.TryToMoveBlockRight);
			input.Add(Key.CursorUp, State.Pressing, controller.TryToRotateBlockClockwise);
			input.Add(Key.CursorDown, State.Pressing, controller.DropBlockFast);
			input.Add(Key.CursorDown, State.Releasing, controller.DropBlockSlow);
		}

		private void AddScore(Renderer renderer)
		{
			renderer.Add(
				scoreboard =
					new VectorText("Welcome to Blocks", new Point(0.45f, 0.3f), 0.025f)
					{
						RenderLayer = (byte)RenderLayer.Foreground
					});
		}
	}
}