using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class PlayerShipTests : TestWithMocksOrVisually
	{
		private void InitPlayerShip()
		{
			playerShip = new PlayerShip();
		}

		private PlayerShip playerShip;

		[Test]
		public void Accelarate()
		{
			InitPlayerShip();
			Point originalVelocity = playerShip.Get<Velocity2D>().velocity;
			playerShip.ShipAccelerate();
			Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().velocity);
		}

		[Test]
		public void TurnChangesAngleCorrectly()
		{
			InitPlayerShip();
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
			InitPlayerShip();
			bool firedRocket = false;
			playerShip.ProjectileFired += projectile => { firedRocket = true; };
			playerShip.IsFiring = true;
			resolver.AdvanceTimeAndExecuteRunners(1 / 0.003f);
			Assert.IsTrue(firedRocket);
		}

		[Test]
		public void HittingBordersTopLeft()
		{
			var screenspace = Resolve<ScreenSpace>();
			InitPlayerShip();
			playerShip.Set(new Rectangle(screenspace.TopLeft - new Point(0.1f, 0.1f),
				PlayerShip.PlayerShipSize));
		}

		[Test]
		public void HittingBordersBottomRight()
		{
			var screenspace = Resolve<ScreenSpace>();
			InitPlayerShip();
			playerShip.Set(new Rectangle(screenspace.BottomRight, PlayerShip.PlayerShipSize));
		}
	}
}