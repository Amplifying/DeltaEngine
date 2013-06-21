using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.Tests.UserInterfaces;

namespace DeltaEngine.Scenes.Tests
{
	public static class Program
	{
		public static void Main()
		{
			new MenuTests().ShowMenuWithTwoButtons(TestWithAllFrameworks.OpenGL);
		}
	}
}