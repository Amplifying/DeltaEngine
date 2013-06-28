using DeltaEngine.Input;
using DeltaEngine.Platforms;

namespace FindTheWord
{
	public class Program : App
	{
		public Program()
		{
			var input = Resolve<InputCommands>();
			var startupScreen = new StartupScreen(input);
			var gameScreen = new GameScreen(input);
			new Game(Resolve<Window>(), startupScreen, gameScreen);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}