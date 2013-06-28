using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class GameLogicTests : TestWithMocksOrVisually
	{
		[Test]
		public void AsteroidCreatedWhenTimeReached()
		{
			var gameLogic = new GameLogic();
			resolver.AdvanceTimeAndExecuteRunners(1.1f);
			Assert.GreaterOrEqual(gameLogic.ExistantAsteroids.Count, 2);
		}

		private GameLogic gameLogic;

		[Test]
		public void ProjectileAndAsteroidDisposedOnCollision()
		{
			gameLogic = new GameLogic();
			var projectile = new Projectile(ContentLoader.Load<Image>("DeltaEngineLogo"), Point.Half, 0);
			gameLogic.ExistantProjectiles.Add(projectile);
			gameLogic.CreateAsteroidsAtPosition(Point.Half, 1, 1);
			resolver.AdvanceTimeAndExecuteRunners(0.2f);
			Assert.IsFalse(projectile.IsActive);
		}

		[Test]
		public void PlayerShipAndAsteroidCollidingResultsInGameOver()
		{
				gameLogic = new GameLogic();
				bool gameOver = false;
				gameLogic.GameOver += () => { gameOver = true; };
				gameLogic.Player.Set(new Rectangle(Point.Half, PlayerShip.PlayerShipSize));
				gameLogic.CreateAsteroidsAtPosition(Point.Half, 1, 1);
				resolver.AdvanceTimeAndExecuteRunners(0.2f);
				Assert.IsTrue(gameOver);
		}
	}
}