using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace ShadowShotGame
{
	public class GameController : Entity, IDisposable
	{
		public GameController(PlayerShip playerShip, Image image, Size size)
		{
			ship = playerShip;
			asteroidImage = image;
			asteroidSize = size;
			Add<GameLogicHandler>();
			ShipCollidedWithAsteroid += RestartGame;
		}

		private readonly PlayerShip ship;
		private readonly Image asteroidImage;
		private readonly Size asteroidSize;
		private readonly List<Asteroid> addAsteroidsList = new List<Asteroid>();
		public List<Asteroid> ActiveAsteroidList = new List<Asteroid>();

		internal class GameLogicHandler : EntityHandler
		{
			public GameLogicHandler()
			{
				random = new PseudoRandom();
				Filter = entity => entity is GameController;
			}

			private readonly PseudoRandom random;

			public override void Handle(Entity entity)
			{
				var gameEntity = entity as GameController;
				DoAddAndRemoveAsteroids(gameEntity);
				CreateRandomAsteroids(gameEntity);
				CheckForShipAsteroidCollision(gameEntity);
				CheckForProjectileAsteroidCollision(gameEntity);
			}

			private void DoAddAndRemoveAsteroids(GameController manager)
			{
				foreach (Asteroid asteroid in manager.addAsteroidsList)
					manager.ActiveAsteroidList.Add(asteroid);

				manager.addAsteroidsList.Clear();

				foreach (Asteroid asteroid in manager.AsteroidRemoveList)
				{
					manager.ActiveAsteroidList.Remove(asteroid);
					asteroid.IsActive = false;
				}

				manager.AsteroidRemoveList.Clear();
			}

			private void CreateRandomAsteroids(GameController manager)
			{
				if (random.Get() < Constants.AsteroidSpawnProbability * Time.Current.Delta)
					if (AsteroidsCount < Constants.MaximumAsteroids)
					{
						var drawArea = GetRandomDrawArea(manager);
						manager.addAsteroidsList.Add(new Asteroid(manager.asteroidImage, drawArea));
						AsteroidsCount++;
					}

				AsteroidsCount = 0;
			}

			private int AsteroidsCount { get; set; }

			private Rectangle GetRandomDrawArea(GameController gameController)
			{
				var posX = random.Get(0.05f, 0.95f);
				return Rectangle.FromCenter(new Point(posX, 0.1f), gameController.asteroidSize);
			}

			private void CheckForShipAsteroidCollision(GameController gameController)
			{
				foreach (Asteroid asteroid in gameController.ActiveAsteroidList)
					if (gameController.ship.DrawArea.IsColliding(0.0f, asteroid.DrawArea, 0.0f))
						if (gameController.ShipCollidedWithAsteroid != null)
							gameController.ShipCollidedWithAsteroid();
			}

			private void CheckForProjectileAsteroidCollision(GameController gameController)
			{
				var toRemove = new List<Projectile>();
				foreach (Projectile projectile in gameController.ship.ActiveProjectileList)
					if (projectile.IsActive)
						foreach (Asteroid asteroid in gameController.ActiveAsteroidList)
							if (asteroid.IsActive)
								if (asteroid.DrawArea.IsColliding(0.0f, projectile.DrawArea, 0.0f))
								{
									projectile.IsActive = false;
									toRemove.Add(projectile);
									gameController.AsteroidRemoveList.Add(asteroid);
								}

				foreach (var projectile in toRemove)
					gameController.ship.ActiveProjectileList.Remove(projectile);
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.High; }
			}
		}

		public List<Asteroid> AsteroidRemoveList = new List<Asteroid>();

		public event Action ShipCollidedWithAsteroid;

		private void RestartGame()
		{
			ship.Dispose();
			Dispose();
		}

		public void Dispose()
		{
			AsteroidRemoveList.AddRange(addAsteroidsList);
			AsteroidRemoveList.AddRange(ActiveAsteroidList);

			foreach (Asteroid asteroid in AsteroidRemoveList)
				asteroid.Dispose();

			Remove<GameLogicHandler>();
			IsActive = false;
		}
	}
}