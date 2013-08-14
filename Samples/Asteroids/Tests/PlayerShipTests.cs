using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class PlayerShipTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			playerShip = new PlayerShip();
		}

		private PlayerShip playerShip;

		[Test]
		public void Accelarate()
		{
			Point originalVelocity = playerShip.Get<Velocity2D>().velocity;
			playerShip.ShipAccelerate();
			Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().velocity);
		}

		[Test]
		public void TurnChangesAngleCorrectly()
		{
			float originalAngle = playerShip.Rotation;
			playerShip.SteerLeft();
			Assert.Less(playerShip.Rotation, originalAngle);
			originalAngle = playerShip.Rotation;
			playerShip.SteerRight();
			Assert.Greater(playerShip.Rotation, originalAngle);
		}

		[Test]
		public void FireRocket()
		{
			bool firedRocket = false;
			playerShip.ProjectileFired += projectile =>
			{
				firedRocket = true;
			};
			playerShip.IsFiring = true;
			AdvanceTimeAndUpdateEntities(1 / 0.003f);
			Assert.IsTrue(firedRocket);
		}

		[Test]
		public void HittingBordersTopLeft()
		{
			playerShip.Set(new Rectangle(ScreenSpace.Current.TopLeft - new Point(0.1f, 0.1f),
				PlayerShip.PlayerShipSize));
		}

		[Test]
		public void HittingBordersBottomRight()
		{
			playerShip.Set(new Rectangle(ScreenSpace.Current.BottomRight, PlayerShip.PlayerShipSize));
		}
	}
}