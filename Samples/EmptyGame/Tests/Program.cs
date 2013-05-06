using DeltaEngine.Platforms.All;

namespace EmptyGame.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new GameTests().ContinuouslyChangeBackgroundColor(TestWithAllFrameworks.OpenGL);
		}
	}
}