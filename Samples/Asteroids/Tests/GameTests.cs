using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void GameOver()
		{
			var game = new AsteroidsGame(Resolve<InputCommands>(), Resolve<ScreenSpace>());
			game.GameOver();
			Assert.AreEqual(GameState.GameOver, game.GameState);
			Assert.IsFalse(game.GameLogic.Player.IsActive);
		}
	}
}