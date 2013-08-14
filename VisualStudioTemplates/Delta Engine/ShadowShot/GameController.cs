using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class GameController : Entity2D, IDisposable
	{
		public GameController(PlayerShip playerShip, Material material, Size size, ScreenSpace 
			screenSpace) : base(Rectangle.Zero)
		{
			this.screenSpace = screenSpace;
			ship = playerShip;
			asteroidMaterial = material;
			asteroidSize = size;
			Start<GameLogicHandler>();
		}

		private readonly PlayerShip ship;
		private readonly Material asteroidMaterial;
		private readonly Size asteroidSize;
		private readonly ScreenSpace screenSpace;
		private class GameLogicHandler : UpdateBehavior
		{
			public GameLogicHandler()
			{
				random = new PseudoRandom();
			}

			private readonly PseudoRandom random;

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (GameController gameController in entities)
				{
					DoAddAndRemoveAsteroids(gameController);
					CreateRandomAsteroids(gameController);
					CheckForShipAsteroidCollision(gameController);
					CheckForProjectileAsteroidCollision(gameController);
				}
			}

			private static void DoAddAndRemoveAsteroids(GameController manager)
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
				if (random.Get() < Constants.AsteroidSpawnProbability * Time.Delta)
					if (AsteroidsCount < Constants.MaximumAsteroids)
				{
					var drawArea = GetRandomDrawArea(manager);
					manager.addAsteroidsList.Add(new Asteroid(manager.asteroidMaterial, drawArea, 
						manager.screenSpace.Viewport.Bottom));
					AsteroidsCount++;
				}
				AsteroidsCount = 0;
			}

			private int AsteroidsCount
			{
				get;
				set;
			}

			private Rectangle GetRandomDrawArea(GameController gameController)
			{
				var posX = random.Get(0.05f, 0.95f);
				return Rectangle.FromCenter(new Point(posX, 0.1f), gameController.asteroidSize);
			}

			private static void CheckForShipAsteroidCollision(GameController gameController)
			{
				foreach (Asteroid asteroid in gameController.ActiveAsteroidList)
					if (gameController.ship.DrawArea.IsColliding(0.0f, asteroid.DrawArea, 0.0f))
						if (gameController.ShipCollidedWithAsteroid != null)
							gameController.ShipCollidedWithAsteroid();
			}

			private static void CheckForProjectileAsteroidCollision(GameController gameController)
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
		}
		private readonly List<Asteroid> addAsteroidsList = new List<Asteroid>();
		public readonly List<Asteroid> ActiveAsteroidList = new List<Asteroid>();
		public readonly List<Asteroid> AsteroidRemoveList = new List<Asteroid>();

		public event Action ShipCollidedWithAsteroid;

		public void Dispose()
		{
			ship.Dispose();
			AsteroidRemoveList.AddRange(addAsteroidsList);
			AsteroidRemoveList.AddRange(ActiveAsteroidList);
			foreach (Asteroid asteroid in AsteroidRemoveList)
				asteroid.Dispose();

			Stop<GameLogicHandler>();
			IsActive = false;
		}
	}
}