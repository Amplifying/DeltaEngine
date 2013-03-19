using DeltaEngine.Platforms.Tests;

namespace EmptyGame.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new GameTests().ContinuouslyChangeBackgroundColor(TestStarter.OpenGL);
		}
	}
}