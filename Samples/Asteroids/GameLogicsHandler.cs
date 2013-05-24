using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace Asteroids
{
	internal class GameLogicsHandler : EntityHandler
	{
		public GameLogicsHandler()
		{
			timeLastNewAsteroid = 0;
		}

		public override void Handle(List<Entity> entities)
		{
			foreach (AsteroidsGame gameEntity in entities.OfType<AsteroidsGame>())
			{
				foreach (Asteroid asteroid in asteroidsToRemove)
					gameEntity.existantAsteroids.Remove(asteroid);
				asteroidsToRemove.Clear();

				foreach (Asteroid asteroid in gameEntity.asteroidsToCreate)
					gameEntity.existantAsteroids.Add(asteroid);
				gameEntity.asteroidsToCreate.Clear();

				foreach (Projectile projectile in projectilesToRemove)
					gameEntity.existantProjectiles.Remove(projectile);
				projectilesToRemove.Clear();

				foreach (var asteroid in gameEntity.existantAsteroids)
				{
					foreach (var projectile in gameEntity.existantProjectiles)
						if (projectile.DrawArea.Center.DistanceTo(asteroid.DrawArea.Center) <
							0.1f / asteroid.sizeModifier)
						{
							projectilesToRemove.Add(projectile);
							projectile.IsActive = false;

							asteroid.Fracture();
							asteroidsToRemove.Add(asteroid);
						}

					if (gameEntity.player.DrawArea.Center.DistanceTo(asteroid.DrawArea.Center) <
						0.06f / asteroid.sizeModifier)
						gameEntity.GameOver();
				}

				if (Time.Current.Milliseconds - 1000 > timeLastNewAsteroid &&
					gameEntity.existantAsteroids.Count <= Constants.maximumAsteroids)
				{
					gameEntity.CreateRandomAsteroids(1);
					timeLastNewAsteroid = Time.Current.Milliseconds;
				}
			}
		}

		private float timeLastNewAsteroid;

		private readonly List<Projectile> projectilesToRemove = new List<Projectile>();
		private readonly List<Asteroid> asteroidsToRemove = new List<Asteroid>();

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}