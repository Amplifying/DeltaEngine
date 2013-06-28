using DeltaEngine.Platforms;
using DeltaEngine.Scenes.UserInterfaces.Graphing;

namespace LogoApp
{
	/// <summary>
	/// Displays a number of colored moving logo sprites bouncing around.
	/// </summary>
	public class Program : App
	{
		public Program()
		{
			for (int num = 0; num < 50; num++)
				new BouncingLogo();
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}