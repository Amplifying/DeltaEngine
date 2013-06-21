using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class GameTests : TestWithAllFrameworks
	{
		[Test]
		public void InitializeGame()
		{
			Start(typeof(MockResolver),(Window window, StartupScreen startupScreen,
				GameScreen gameScreen) =>
			{
				new Game(window, startupScreen, gameScreen);
				startupScreen.StartGame();
				Assert.AreEqual(Visibility.Show, gameScreen.Visibility);
			});
		}
	}
}