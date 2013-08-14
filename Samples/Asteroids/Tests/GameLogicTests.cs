using DeltaEngine;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class GameLogicTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitGameLogic()
		{
			Resolve<Window>();
			gameLogic = new GameLogic();
		}

		private GameLogic gameLogic;

		[Test]
		public void AsteroidCreatedWhenTimeReached()
		{
			AdvanceTimeAndUpdateEntities(1.1f);
			Assert.GreaterOrEqual(gameLogic.ExistantAsteroids.Count, 2);
		}

		[Test]
		public void ProjectileAndAsteroidDisposedOnCollision()
		{
			var projectile = new Projectile(new Material(Shader.Position2DColorUv, "DeltaEngineLogo"),
				Point.Half, 0);
			gameLogic.ExistantProjectiles.Add(projectile);
			gameLogic.CreateAsteroidsAtPosition(Point.Half, 1, 1);
			AdvanceTimeAndUpdateEntities(0.2f);
			Assert.IsFalse(projectile.IsActive);
		}

		[Test]
		public void PlayerShipAndAsteroidCollidingResultsInGameOver()
		{
			bool gameOver = false;
			gameLogic.GameOver += () =>
			{
				gameOver = true;
			};
			gameLogic.Player.Set(new Rectangle(Point.Half, PlayerShip.PlayerShipSize));
			gameLogic.CreateAsteroidsAtPosition(Point.Half, 1, 1);
			AdvanceTimeAndUpdateEntities(0.2f);
			Assert.IsTrue(gameOver);
		}
	}
}