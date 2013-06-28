using DeltaEngine.Input;
using DeltaEngine.Platforms;
//using DeltaEngine.Rendering.ScreenSpaces;

namespace TinyPlatformer
{
	internal class Program : App
	{
		public Program()
		{
			new Game(Resolve<Map>(), Resolve<InputCommands>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}