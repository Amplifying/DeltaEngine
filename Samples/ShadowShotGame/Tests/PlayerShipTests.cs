using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Physics2D;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;
using ShadowShotGame;

namespace ShadowShotGameTests
{
	public class PlayerShipTests : TestWithAllFrameworks
	{
		private void InitPlayerShip(ContentLoader contentLoader, ScreenSpace screen)
		{
			screen.Window.TotalPixelSize = new Size(500, 500);
			content = contentLoader;
			var shipImage = contentLoader.Load<Image>("player");
			var shipDrawArea = Rectangle.FromCenter(new Point(0.5f, 0.95f), playerShipSize);
			playerShip = new PlayerShip(shipImage, shipDrawArea, content);
		}

		private PlayerShip playerShip;
		private ContentLoader content;
		private readonly Size playerShipSize = new Size(0.05f);

		[VisualTest]
		public void CreateShip(Type resolver)
		{
			Start(resolver,
				(ContentLoader contentLoader, ScreenSpace screenSpace) =>
				{ InitPlayerShip(contentLoader, screenSpace); });
		}

		[Test]
		public void Accelerate()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, ScreenSpace screen) =>
			{
				InitPlayerShip(contentLoader, screen);
				Point originalVelocity = playerShip.Get<Velocity2D>().velocity;
				playerShip.Accelerate(Point.One);
				Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().velocity);
				playerShip.Accelerate(new Point(0, 1.0f));
				Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().velocity);
			});
		}

		[Test]
		public void HittingBordersBottomRight()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, ScreenSpace screenspace) =>
			{
				InitPlayerShip(contentLoader, screenspace);
				playerShip.Set(new Rectangle(screenspace.BottomRight, playerShipSize));
				Assert.LessOrEqual(screenspace.Viewport.BottomRight.X, playerShip.DrawArea.BottomRight.X);
				Assert.LessOrEqual(screenspace.Viewport.BottomRight.Y, playerShip.DrawArea.BottomRight.Y);
			});
		}

		[Test]
		public void HittingBordersBottomLeft()
		{
			Start(typeof(MockResolver), (ContentLoader contentLoader, ScreenSpace screenspace) =>
			{
				InitPlayerShip(contentLoader, screenspace);
				playerShip.Set(new Rectangle(new Point(0.0f, 1.0f), playerShipSize));
				Assert.LessOrEqual(0.0f, playerShip.DrawArea.BottomRight.X);
				Assert.LessOrEqual(1.0, playerShip.DrawArea.BottomRight.Y);
			});
		}

		[VisualTest]
		public void PressSpacebarToFireWeapon(Type resolver)
		{
			Start(resolver,
				(ContentLoader contentLoader, InputCommands inputCommands, ScreenSpace screenSpace) =>
				{
					InitPlayerShip(contentLoader, screenSpace);
					inputCommands.Add(Key.Space, State.Pressed, key => { playerShip.Fire(); });
				});
		}

		[Test]
		public void CheckForWeaponFire()
		{
			Start(typeof(MockResolver),
				(ContentLoader contentLoader, InputCommands inputCommands, ScreenSpace screenSpace) =>
				{
					var game = new Game(screenSpace, inputCommands, contentLoader);
					bool weaponFired = false;
					game.Ship.ProjectileFired += point => { weaponFired = true; };
					mockResolver.input.SetKeyboardState(Key.Space, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners(0.5f);
					Assert.IsTrue(weaponFired);
				});
		}

		[Test]
		public void CheckShipCollisionWithAsteroid()
		{
			Start(typeof(MockResolver),
				(ScreenSpace screenSpace, ContentLoader contentLoader, InputCommands inputCommands) =>
				{
					var game = new Game(screenSpace, inputCommands, contentLoader);
					mockResolver.AdvanceTimeAndExecuteRunners(10.0f);
					Assert.IsFalse(game.Ship.IsActive);
				});
		}

		[VisualTest]
		public void CollisonWithAsteroidEndsGame(Type resolver)
		{
			Start(resolver,
				(ScreenSpace screenSpace, ContentLoader contentLoader, InputCommands inputCommands) =>
				{
					InitPlayerShip(contentLoader, screenSpace);
					playerShip.DrawArea = new Rectangle(Point.Half, playerShip.Size);
					new GameController(playerShip, content.Load<Image>("asteroid"), playerShip.Size);
				});
		}

		[Test]
		public void DisposePlayerShip()
		{
			Start(typeof(MockResolver), (ScreenSpace screenSpace, ContentLoader contentLoader) =>
			{
				InitPlayerShip(contentLoader, screenSpace);
				mockResolver.input.SetKeyboardState(Key.Space, State.Pressed);
				playerShip.Dispose();
				Assert.AreEqual(0, playerShip.ActiveProjectileList.Count);
				Assert.IsFalse(playerShip.IsActive);
			});
		}
	}
}