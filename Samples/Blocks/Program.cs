using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blocks
{
	/// <summary>
	/// Falling blocks can be moved and rotated. Points are scored by filling rows.
	/// </summary>
	public class Program : App
	{
		public Program()
		{
			var blocksContent = new FruitBlocksContent();
			new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>(), blocksContent,
				Resolve<SoundDevice>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}