using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class AsteroidTests : TestWithAllFrameworks
	{
		[Test]
		public void FractureAsteroid()
		{
			Start(typeof(MockResolver),
				(ContentLoader contentLoader, PseudoRandom randomizer, GameLogic gameLogic) =>
				{
					var asteroid = new Asteroid(contentLoader, randomizer, gameLogic);
					asteroid.Fracture();
					Assert.IsFalse(asteroid.IsActive);
				});
		}

		[VisualTest]
		public void ShowAsteroidsOfSeveralSizemodsAndFracture(Type resolver)
		{
			Start(resolver,
				(ContentLoader contentLoader, PseudoRandom randomizer, GameLogic gameLogic) =>
				{
					var largeAsteroid = new Asteroid(contentLoader, randomizer, gameLogic);
					var mediumAsteroid = new Asteroid(contentLoader, randomizer, gameLogic,2);
					var smallAsteroid = new Asteroid(contentLoader, randomizer, gameLogic, 3);

					largeAsteroid.Fracture();
					Assert.IsFalse(largeAsteroid.IsActive);
				});
		}
	}
}