using DeltaEngine.Input;

namespace TinyPlatformer
{
	/// <summary>
	/// Manages the map with lots of simple square sprites, also starts the game logic
	/// </summary>
	public class Game
	{
		public Game(Map map, InputCommands inputCommands)
		{
			this.map = map;
			gameLogic = new GameLogic(map, inputCommands);
		}
		private Map map;
		private GameLogic gameLogic;
	}
}