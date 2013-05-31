using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class GameLogicTests : TestWithAllFrameworks
	{
		[Test]
		public void AsteroidCreatedWhenTimeReached()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, InputCommands inputCommands) =>
			{
				gameLogic = new GameLogic(contentLoader);
				mockResolver.AdvanceTimeAndExecuteRunners(1.1f);
				Assert.GreaterOrEqual(gameLogic.ExistantAsteroids.Count, 2);
			});
		}

		private GameLogic gameLogic;

		[Test]
		public void ProjectileAndAsteroidDisposedOnCollision()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, InputCommands inputCommands) =>
			{
				gameLogic = new GameLogic(contentLoader);
				var projectile = new Projectile(contentLoader.Load<Image>("DeltaEngineLogo"), Point.Half, 0);
				gameLogic.ExistantProjectiles.Add(projectile);
				gameLogic.CreateAsteroidsAtPosition(Point.Half, 1, 1);
				mockResolver.AdvanceTimeAndExecuteRunners(0.2f);
				Assert.IsFalse(projectile.IsActive);
			});
		}

		[Test]
		public void PlayerShipAndAsteroidCollidingResultsInGameOver()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, InputCommands inputCommands) =>
			{
				gameLogic = new GameLogic(contentLoader);
				bool gameOver = false;
				gameLogic.GameOver += () => { gameOver = true; };
				gameLogic.Player.Set(new Rectangle(Point.Half, PlayerShip.PlayerShipSize));
				gameLogic.CreateAsteroidsAtPosition(Point.Half, 1, 1);
				mockResolver.AdvanceTimeAndExecuteRunners(0.2f);
				Assert.IsTrue(gameOver);
			});
		}
	}
}