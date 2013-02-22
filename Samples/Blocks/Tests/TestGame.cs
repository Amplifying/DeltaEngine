using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Blocks.Tests
{
	/// <summary>
	/// Helps test Game by exposing the scoreboard field
	/// </summary>
	public class TestGame : Game
	{
		public TestGame(InputCommands input, Renderer renderer, Controller controller)
			: base(input, renderer, controller) {}

		public VectorText Scoreboard
		{
			get { return scoreboard; }
		}
	}
}