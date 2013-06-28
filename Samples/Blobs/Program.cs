using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blobs
{
	/// <summary>
	/// Fire your blob onto other blobs to absorb them
	/// </summary>
	public class Program : App
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