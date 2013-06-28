using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Snake
{
	internal class Program : App
	{
		public Program()
		{
			new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}