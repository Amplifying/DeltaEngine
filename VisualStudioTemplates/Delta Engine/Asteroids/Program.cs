using DeltaEngine;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	internal class Program : App
	{
		public Program()
		{
			new RelativeScreenSpace(Resolve<Window>());
			new AsteroidsGame();
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}