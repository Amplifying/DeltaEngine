using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace GameOfDeath
{
	/// <summary>
	/// This game is from our first Game Jam early in 2013. It is a spinoff of the good old
	/// Game of Life, but with the twist to kill multiplying rabbits quickly.
	/// </summary>
	public class Program : App
	{
		public Program()
		{
			var screenSpace = Resolve<ScreenSpace>();
			new Intro();
			new UI(screenSpace, Resolve<SoundDevice>());
			new InputCoordinator(screenSpace.Window, Resolve<InputCommands>(), Resolve<GameCoordinator>());
		}

		private static void Main()
		{
			new Program().Run();
		}
	}
}