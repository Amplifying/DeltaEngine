using DeltaEngine.Platforms.All;

namespace LogoApp.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new BouncingLogoTests().ShowOneLogo(TestWithAllFrameworks.OpenGL);
		}
	}
}