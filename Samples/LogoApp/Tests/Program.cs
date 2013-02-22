using DeltaEngine.Platforms.Tests;

namespace LogoApp.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new BouncingLogoTests().ShowOneLogo(TestStarter.Xna);
			new BouncingLogoTests().Show50LogosAndDisplayFps(TestStarter.OpenGL);
		}
	}
}