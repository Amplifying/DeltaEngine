using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class GameTests : TestWithAllFrameworks
	{
		[Test]
		public void GameOver()
		{
			Start(typeof(MockResolver),
				(ContentLoader contentLoader, InputCommands inputCommands, ScreenSpace screenSpace) =>
				{
					var game = new AsteroidsGame(contentLoader, inputCommands, screenSpace);
					game.GameOver();
					Assert.AreEqual(GameState.GameOver, game.GameState);
					Assert.IsFalse(game.GameLogic.Player.IsActive);
				});
		}
	}
}