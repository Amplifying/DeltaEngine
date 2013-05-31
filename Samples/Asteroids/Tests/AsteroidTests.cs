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
		public void ShowAstroids(Type resolver)
		{
			Start(resolver,
				(ContentLoader contentLoader, PseudoRandom randomizer, GameLogic gameLogic) =>
				{ new Asteroid(contentLoader, randomizer, gameLogic); });
		}

		[VisualTest]
		public void ShowfracturedAstroids(Type resolver)
		{
			Start(resolver,
				(ContentLoader contentLoader, PseudoRandom randomizer, GameLogic gameLogic) =>
				{
					var asteroid = new Asteroid(contentLoader, randomizer, gameLogic);
					asteroid.Fracture();
				});
		}
	}
}