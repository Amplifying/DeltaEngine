using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	public class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void InitializeGame()
		{
			var startupScreen = Resolve<StartupScreen>();
			var gameScreen = Resolve<GameScreen>();
			new Game(Window, startupScreen, gameScreen);
			startupScreen.StartGame();
			Assert.AreEqual(Visibility.Show, gameScreen.Visibility);
		}
	}
}