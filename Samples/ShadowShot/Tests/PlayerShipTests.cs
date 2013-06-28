using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace ShadowShot.Tests
{
	public class PlayerShipTests : TestWithMocksOrVisually
	{
		private void InitPlayerShip()
		{
			Resolve<ScreenSpace>().Window.ViewportPixelSize = new Size(500, 500);
			var shipImage = ContentLoader.Load<Image>("player");
			var shipDrawArea = Rectangle.FromCenter(new Point(0.5f, 0.95f), playerShipSize);
			playerShip = new PlayerShip(shipImage, shipDrawArea);
		}

		private PlayerShip playerShip;
		private readonly Size playerShipSize = new Size(0.05f);

		[Test]
		public void CreateShip(Type resolver)
		{
			InitPlayerShip();
		}

		[Test]
		public void Accelerate()
		{
			InitPlayerShip();
			Point originalVelocity = playerShip.Get<Velocity2D>().velocity;
			playerShip.Accelerate(Point.One);
			Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().velocity);
			playerShip.Accelerate(new Point(0, 1.0f));
			Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().velocity);
		}

		[Test]
		public void HittingBordersBottomRight()
		{
			InitPlayerShip();
			var screenspace = Resolve<ScreenSpace>();
			playerShip.Set(new Rectangle(screenspace.BottomRight, playerShipSize));
			Assert.LessOrEqual(screenspace.Viewport.BottomRight.X, playerShip.DrawArea.BottomRight.X);
			Assert.LessOrEqual(screenspace.Viewport.BottomRight.Y, playerShip.DrawArea.BottomRight.Y);
		}

		[Test]
		public void HittingBordersBottomLeft()
		{
			InitPlayerShip();
			playerShip.Set(new Rectangle(new Point(0.0f, 1.0f), playerShipSize));
			Assert.LessOrEqual(0.0f, playerShip.DrawArea.BottomRight.X);
			Assert.LessOrEqual(1.0, playerShip.DrawArea.BottomRight.Y);
		}

		[Test]
		public void PressSpacebarToFireWeapon(Type resolver)
		{
			InitPlayerShip();
			Resolve<InputCommands>().Add(Key.Space, State.Pressed, key => { playerShip.Fire(); });
		}

		[Test]
		public void CheckForWeaponFire()
		{
			var game = new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			bool weaponFired = false;
			game.Ship.ProjectileFired += point => { weaponFired = true; };
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.Space, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners(0.5f);
			Assert.IsTrue(weaponFired);
		}

		//[Test]
		//public void CheckShipCollisionWithAsteroid()
		//{
		//	var game = new Game(Resolve<ScreenSpace>(), Resolve<InputCommands>());
		//	var collided = false;
		//	game.Controller.ShipCollidedWithAsteroid += () => { collided = true; };
		//	resolver.AdvanceTimeAndExecuteRunners(10.0f);
		//	Assert.IsTrue(collided);
		//}

		[Test]
		public void CollisonWithAsteroidEndsGame(Type resolver)
		{
			InitPlayerShip();
			playerShip.DrawArea = new Rectangle(Point.Half, playerShip.Size);
			new GameController(playerShip, ContentLoader.Load<Image>("asteroid"), playerShip.Size);
		}

		[Test]
		public void DisposePlayerShip()
		{
			InitPlayerShip();
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.Space, State.Pressed);
			playerShip.Dispose();
			Assert.AreEqual(0, playerShip.ActiveProjectileList.Count);
			Assert.IsFalse(playerShip.IsActive);
		}
	}
}