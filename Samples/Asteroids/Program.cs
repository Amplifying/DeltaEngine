using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Asteroids
{
	internal class Program : App
	{
		public Program()
		{
			new AsteroidsGame(Resolve<InputCommands>(),Resolve<ScreenSpace>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}