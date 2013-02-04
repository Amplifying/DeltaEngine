using DeltaEngine.Platforms.Tests;

namespace Breakout.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new BackgroundTests().Draw(TestStarter.DirectX);
			//new PaddleTests().Draw(TestStarter.DirectX);
			//new BallTests().Draw(TestStarter.DirectX);
			//new LevelTests().Draw(TestStarter.DirectX);
			new GameTests().Draw(TestStarter.DirectX);
		}
	}
}