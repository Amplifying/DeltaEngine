using DeltaEngine.Input;
using DeltaEngine.Platforms;

namespace GameOfDeath
{
	/// <summary>
	/// Initializes input handling
	/// </summary>
	public class InputCoordinator
	{
		public InputCoordinator(Window window, InputCommands input, GameCoordinator gameCoordinator)
		{
			window.ShowCursor = false;
			window.Title = "Game Of Death - Kill rabbits before they occupy more than 75% of the world!";
			input.Add(Key.Escape, window.Dispose);
			gameCoordinator.RespondToInput(input);
		}
	}
}
