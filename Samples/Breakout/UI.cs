using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;

namespace Breakout
{
	/// <summary>
	/// Primes the window to respond to keyboard commands and launches the game
	/// </summary>
	public class UI : Runner<Time>
	{
		public UI(Window window, InputCommands inputCommands, Game game)
		{
			this.window = window;
			inputCommands.Add(Key.Escape, window.Dispose);
			inputCommands.Add(Key.F, () => window.SetFullscreen(new Size(1920, 1080)));
			this.game = game;
		}

		private readonly Window window;
		private readonly Game game;

		public void Run(Time time)
		{
			if (time.CheckEvery(0.2f))
				window.Title = "Breakout " + game.Score;
		}
	}
}