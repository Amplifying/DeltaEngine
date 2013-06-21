using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;

namespace Asteroids
{
	public class GameLogic : Entity
	{
		public GameLogic(ContentLoader content)
		{
			this.content = content;
			random = new PseudoRandom();
			ExistantAsteroids = new List<Asteroid>();
			ExistantProjectiles = new List<Projectile>();
			Player = new PlayerShip(content);
			Player.ProjectileFired += projectile => { ExistantProjectiles.Add(projectile); };
			CreateRandomAsteroids(1);
			CreateRandomAsteroids(1, 2);
			Add<GameLogicsHandler>();
			IncreaseScore += i => { };
		}

		public PlayerShip Player { get; private set; }
		private readonly ContentLoader content;
		private readonly PseudoRandom random;
		public List<Asteroid> ExistantAsteroids { get; private set; }
		public List<Projectile> ExistantProjectiles { get; private set; }

		public void CreateRandomAsteroids(int howMany, int sizeMod = 1)
		{
			for (int asteroidCount = 0; asteroidCount < howMany; asteroidCount++)
			{
				var asteroid = new Asteroid(content, random, this, sizeMod);
				asteroidsToCreate.Add(asteroid);
			}
		}

		public readonly List<Asteroid> asteroidsToCreate = new List<Asteroid>();

		public void CreateAsteroidsAtPosition(Point position, int sizeMod = 1, int howMany = 2)
		{
			for (int asteroidCount = 0; asteroidCount < howMany; asteroidCount++)
			{
				var asteroid = new Asteroid(content, random, this, sizeMod);
				asteroid.DrawArea = new Rectangle(position, asteroid.DrawArea.Size);
				asteroidsToCreate.Add(asteroid);
			}
		}

		public void IncrementScore(int increase)
		{
			IncreaseScore.Invoke(increase);
		}

		internal class GameLogicsHandler : EntityHandler
		{
			public GameLogicsHandler()
			{
				timeLastNewAsteroid = 0;
			}

			public override void Handle(Entity entity)
			{
				var gameEntity = entity as GameLogic;
				DoRemoveAndAddObjects(gameEntity);
				CheckAsteroidCollisions(gameEntity);
				CreateNewAsteroidIfNecessary(gameEntity);
			}

			private void DoRemoveAndAddObjects(GameLogic gameEntity)
			{
				foreach (Asteroid asteroid in asteroidsToRemove)
					gameEntity.ExistantAsteroids.Remove(asteroid);
				asteroidsToRemove.Clear();

				foreach (Asteroid asteroid in gameEntity.asteroidsToCreate)
					gameEntity.ExistantAsteroids.Add(asteroid);
				gameEntity.asteroidsToCreate.Clear();

				foreach (Projectile projectile in projectilesToRemove)
					gameEntity.ExistantProjectiles.Remove(projectile);
				projectilesToRemove.Clear();
			}

			private void CheckAsteroidCollisions(GameLogic gameEntity)
			{
				foreach (var asteroid in gameEntity.ExistantAsteroids)
				{
					foreach (var projectile in gameEntity.ExistantProjectiles)
						if (ObjectsInHitRadius(projectile, asteroid, 0.1f / asteroid.sizeModifier))
						{
							PrepareProjectileDisposal(projectile);
							PrepareAsteroidDisposal(asteroid);
						}

					if (ObjectsInHitRadius(gameEntity.Player, asteroid, 0.06f / asteroid.sizeModifier))
						if (gameEntity.GameOver != null)
							gameEntity.GameOver();
				}
			}

			private static bool ObjectsInHitRadius(Entity2D entityAlpha, Entity2D entitytBeta,
				float radius)
			{
				return entityAlpha.DrawArea.Center.DistanceTo(entitytBeta.DrawArea.Center) < radius;
			}

			private void PrepareProjectileDisposal(Projectile projectileToDestroy)
			{
				projectilesToRemove.Add(projectileToDestroy);
				projectileToDestroy.IsActive = false;
			}

			private void PrepareAsteroidDisposal(Asteroid asteroidToDestroy)
			{
				asteroidToDestroy.Fracture();
				asteroidsToRemove.Add(asteroidToDestroy);
			}

			private void CreateNewAsteroidIfNecessary(GameLogic gameEntity)
			{
				if (Time.Current.Milliseconds - 1000 > timeLastNewAsteroid &&
					gameEntity.ExistantAsteroids.Count <= MaximumAsteroids)
				{
					gameEntity.CreateRandomAsteroids(1);
					timeLastNewAsteroid = Time.Current.Milliseconds;
				}
			}

			private const int MaximumAsteroids = 10;
			private float timeLastNewAsteroid;
			private readonly List<Projectile> projectilesToRemove = new List<Projectile>();
			private readonly List<Asteroid> asteroidsToRemove = new List<Asteroid>();
		}

		public event Action GameOver;
		public event Action<int> IncreaseScore;
	}
}