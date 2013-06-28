using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class AsteroidTests : TestWithMocksOrVisually
	{
		[Test]
		public void FractureAsteroid()
		{
			var asteroid = new Asteroid(new PseudoRandom(), new GameLogic());
			asteroid.Fracture();
			Assert.IsFalse(asteroid.IsActive);
		}

		[Test]
		public void ShowAsteroidsOfSeveralSizemodsAndFracture()
		{
			var randomizer = new PseudoRandom();
			var gameLogic = new GameLogic();
			var largeAsteroid = new Asteroid(randomizer, gameLogic);
			new Asteroid(randomizer, gameLogic, 2);
			new Asteroid(randomizer, gameLogic, 3);

			largeAsteroid.Fracture();
			Assert.IsFalse(largeAsteroid.IsActive);
		}
	}
}