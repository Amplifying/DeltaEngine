using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// The player's armed spaceship deployed against the dangerous rocks.
	/// Derived Sprite with movement and fully automatic fireing of rockets. 
	/// </summary>
	public class PlayerShip : Sprite
	{
		public PlayerShip()
			: base(
				ContentLoader.Load<Image>(PlayerShipTextureName), new Rectangle(Point.Half, PlayerShipSize))
		{
			Add(new Velocity2D(Point.Zero, MaximumPlayerVelocity));
			Start<PlayerMovementHandler>();
			Start<FullAutoFire>();
			RenderLayer = (int)AsteroidsRenderLayer.Player;
			projectileTexture = ContentLoader.Load<Image>(ProjectileTextureName);
		}

		private const string PlayerShipTextureName = "ship2";
		public static readonly Size PlayerShipSize = new Size(.05f);
		private const float MaximumPlayerVelocity = .5f;
		private readonly Image projectileTexture;
		private const string ProjectileTextureName = "projectile";

		public void ShipAccelerate()
		{
			Get<Velocity2D>().Accelerate(PlayerAcceleration * Time.Current.Delta, Rotation);
		}

		private const float PlayerAcceleration = 1;

		public void SteerLeft()
		{
			Rotation -= PlayerTurnSpeed * Time.Current.Delta;
		}

		public void SteerRight()
		{
			Rotation += PlayerTurnSpeed * Time.Current.Delta;
		}

		private const float PlayerTurnSpeed = 160;

		private class PlayerMovementHandler : Behavior2D
		{
			public PlayerMovementHandler(ScreenSpace screen)
			{
				this.screen = screen;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity2D entity)
			{
				var nextRect = CalculateRectAfterMove(entity);
				MoveEntity(entity, nextRect);
				var velocity2D = entity.Get<Velocity2D>();
				velocity2D.velocity -= velocity2D.velocity * PlayerDecelFactor * Time.Current.Delta;
				entity.Set(velocity2D);
			}

			private const float PlayerDecelFactor = 0.7f;

			private static Rectangle CalculateRectAfterMove(Entity2D entity)
			{
				return
					new Rectangle(
						entity.DrawArea.TopLeft + entity.Get<Velocity2D>().velocity * Time.Current.Delta,
						entity.Size);
			}

			private void MoveEntity(Entity2D entity, Rectangle rect)
			{
				StopAtBorder(entity);
				entity.Set(rect);
			}

			private void StopAtBorder(Entity2D entity)
			{
				var drawArea = entity.DrawArea;
				var vel = entity.Get<Velocity2D>();
				CheckStopRightBorder(ref drawArea, vel);
				CheckStopLeftBorder(ref drawArea, vel);
				CheckStopTopBorder(ref drawArea, vel);
				CheckStopBottomBorder(ref drawArea, vel);
				entity.Set(vel);
				entity.Set(drawArea);
			}

			private void CheckStopRightBorder(ref Rectangle rect, Velocity2D vel)
			{
				if (rect.Right <= screen.Viewport.Right)
					return;

				vel.velocity.X = -0.02f;
				rect.Right = screen.Viewport.Right;
			}

			private void CheckStopLeftBorder(ref Rectangle rect, Velocity2D vel)
			{
				if (rect.Left >= screen.Viewport.Left)
					return;

				vel.velocity.X = 0.02f;
				rect.Left = screen.Viewport.Left;
			}

			private void CheckStopTopBorder(ref Rectangle rect, Velocity2D vel)
			{
				if (rect.Top >= screen.Viewport.Top)
					return;

				vel.velocity.Y = 0.02f;
				rect.Top = screen.Viewport.Top;
			}

			private void CheckStopBottomBorder(ref Rectangle rect, Velocity2D vel)
			{
				if (rect.Bottom <= screen.Viewport.Bottom)
					return;

				vel.velocity.Y = -0.02f;
				rect.Bottom = screen.Viewport.Bottom;
			}
		}

		private class FullAutoFire : Behavior2D
		{
			public FullAutoFire()
			{
				CadenceShotsPerSec = PlayerCadance;
				timeLastShot = Time.Current.Milliseconds;
			}

			private const float PlayerCadance = 0.003f;
			private float CadenceShotsPerSec { get; set; }
			private float timeLastShot;

			public override void Handle(Entity2D entity)
			{
				var ship = entity as PlayerShip;
				if (!ship.IsFiring || !(Time.Current.Milliseconds - 1 / CadenceShotsPerSec > timeLastShot))
					return;

				var projectile = new Projectile(ship.projectileTexture, ship.DrawArea.Center, ship.Rotation);
				timeLastShot = Time.Current.Milliseconds;
				if (ship.ProjectileFired != null)
					ship.ProjectileFired.Invoke(projectile);
			}
		}

		public bool IsFiring { get; set; }
		public event Action<Projectile> ProjectileFired;
	}
}