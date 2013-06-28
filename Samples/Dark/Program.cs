using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Dark
{
	public class Program : App
	{
		public Program()
		{
			var sounds = new SoundEffects();
			new Game(Resolve<Window>(), Resolve<InputCommands>(), Resolve<ScreenSpace>(), sounds);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}