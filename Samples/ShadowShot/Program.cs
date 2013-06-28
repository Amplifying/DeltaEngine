using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace ShadowShot
{
	internal class Program : App
	{
		public Program()
		{
			new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}
