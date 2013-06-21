using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class PlayerShipTests : TestWithAllFrameworks
	{
		private void InitPlayerShip(ContentLoader contentLoader)
		{
			playerShip = new PlayerShip(contentLoader);
		}

		private PlayerShip playerShip;

		[Test]
		public void Accelarate()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				InitPlayerShip(contentLoader);
				Point originalVelocity = playerShip.Get<Velocity2D>().velocity;
				playerShip.ShipAccelerate();
				Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().velocity);
			});
		}

		[Test]
		public void TurnChangesAngleCorrectly()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				InitPlayerShip(contentLoader);
				float originalAngle = playerShip.Rotation;
				playerShip.SteerLeft();
				Assert.Less(playerShip.Rotation, originalAngle);
				originalAngle = playerShip.Rotation;
				playerShip.SteerRight();
				Assert.Greater(playerShip.Rotation, originalAngle);
			});
		}

		[Test]
		public void FireRocket()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader) =>
			{
				InitPlayerShip(contentLoader);
				bool firedRocket = false;
				playerShip.ProjectileFired += projectile => { firedRocket = true; };
				playerShip.IsFiring = true;
				mockResolver.AdvanceTimeAndExecuteRunners(1 / 0.003f);
				Assert.IsTrue(firedRocket);
			});
		}

		[Test]
		public void HittingBordersTopLeft()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, ScreenSpace screenspace) =>
			{
				InitPlayerShip(contentLoader);
				playerShip.Set(new Rectangle(screenspace.TopLeft - new Point(0.1f, 0.1f),
					PlayerShip.PlayerShipSize));
			});
		}

		[Test]
		public void HittingBordersBottomRight()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, ScreenSpace screenspace) =>
			{
				InitPlayerShip(contentLoader);
				playerShip.Set(new Rectangle(screenspace.BottomRight, PlayerShip.PlayerShipSize));
			});
		}

		[VisualTest]
		public void ShowPlayerShip(Type resolver)
		{
			Start(resolver, (ContentLoader contentLoader, ScreenSpace screenspace) =>
			{
				playerShip = new PlayerShip(contentLoader);
			});
		}


		[VisualTest]
		public void FireProjectile(Type resolver)
		{
			Start(resolver, (ContentLoader contentLoader, ScreenSpace screenspace) =>
			{
				playerShip = new PlayerShip(contentLoader);
				playerShip.IsFiring = true;
			});
		}
	}
}