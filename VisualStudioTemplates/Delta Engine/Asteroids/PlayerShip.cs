using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class PlayerShip : Sprite
	{
		public PlayerShip() : base(new Material(Shader.Position2DColorUv, "ship1"), new 
			Rectangle(Point.Half, PlayerShipSize))
		{
			Add(new Velocity2D(Point.Zero, MaximumPlayerVelocity));
			Start<PlayerMovementHandler>();
			Start<FullAutoFire>();
			RenderLayer = (int)AsteroidsRenderLayer.Player;
			projectileTexture = new Material(Shader.Position2DColorUv, "projectile");
		}

		private const string PlayerShipTextureName = "ship1";
		public static readonly Size PlayerShipSize = new Size(.05f);
		private const float MaximumPlayerVelocity = .5f;
		private readonly Material projectileTexture;
		private const string ProjectileTextureName = "projectile";

		public void ShipAccelerate()
		{
			Get<Velocity2D>().Accelerate(PlayerAcceleration * Time.Delta, Rotation);
		}

		private const float PlayerAcceleration = 1;

		public void SteerLeft()
		{
			Rotation -= PlayerTurnSpeed * Time.Delta;
		}

		public void SteerRight()
		{
			Rotation += PlayerTurnSpeed * Time.Delta;
		}

		private const float PlayerTurnSpeed = 160;
		private class PlayerMovementHandler : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var entity2D = entity as PlayerShip;
					var nextRect = CalculateRectAfterMove(entity2D, Time.Delta);
					MoveEntity(entity2D, nextRect);
					var velocity2D = entity.Get<Velocity2D>();
					velocity2D.velocity -= velocity2D.velocity * PlayerDecelFactor * Time.Delta;
					entity.Set(velocity2D);
				}
			}

			private const float PlayerDecelFactor = 0.7f;

			private static Rectangle CalculateRectAfterMove(Entity2D entity, float deltaT)
			{
				return new Rectangle(entity.DrawArea.TopLeft + entity.Get<Velocity2D>().velocity * 
					deltaT, entity.Size);
			}

			private static void MoveEntity(PlayerShip entity, Rectangle rect)
			{
				StopAtBorder(entity);
				entity.Set(rect);
			}

			private static void StopAtBorder(PlayerShip entity)
			{
				var drawArea = entity.DrawArea;
				var vel = entity.Get<Velocity2D>();
				var borders = ScreenSpace.Current.Viewport;
				CheckStopRightBorder(ref drawArea, vel, borders);
				CheckStopLeftBorder(ref drawArea, vel, borders);
				CheckStopTopBorder(ref drawArea, vel, borders);
				CheckStopBottomBorder(ref drawArea, vel, borders);
				entity.Set(vel);
				entity.Set(drawArea);
			}

			private static void CheckStopRightBorder(ref Rectangle rect, Velocity2D vel, Rectangle 
				borders)
			{
				if (rect.Right <= borders.Right)
					return;

				vel.velocity.X = -0.02f;
				rect.Right = borders.Right;
			}

			private static void CheckStopLeftBorder(ref Rectangle rect, Velocity2D vel, Rectangle borders)
			{
				if (rect.Left >= borders.Left)
					return;

				vel.velocity.X = 0.02f;
				rect.Left = borders.Left;
			}

			private static void CheckStopTopBorder(ref Rectangle rect, Velocity2D vel, Rectangle borders)
			{
				if (rect.Top >= borders.Top)
					return;

				vel.velocity.Y = 0.02f;
				rect.Top = borders.Top;
			}

			private static void CheckStopBottomBorder(ref Rectangle rect, Velocity2D vel, Rectangle 
				borders)
			{
				if (rect.Bottom <= borders.Bottom)
					return;

				vel.velocity.Y = -0.02f;
				rect.Bottom = borders.Bottom;
			}
		}

		private class FullAutoFire : UpdateBehavior
		{
			public FullAutoFire()
			{
				CadenceShotsPerSec = PlayerCadance;
				timeLastShot = GlobalTime.Current.Milliseconds;
			}

			private const float PlayerCadance = 0.003f;

			private float CadenceShotsPerSec
			{
				get;
				set;
			}

			private float timeLastShot;

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var ship = entity as PlayerShip;
					if (!ship.IsFiring || !(GlobalTime.Current.Milliseconds - 1 / CadenceShotsPerSec > 
						timeLastShot))
						return;

					var projectile = new Projectile(ship.projectileTexture, ship.DrawArea.Center, 
						ship.Rotation);
					timeLastShot = GlobalTime.Current.Milliseconds;
					if (ship.ProjectileFired != null)
						ship.ProjectileFired.Invoke(projectile);
				}
			}
		}
		public bool IsFiring
		{
			get;
			set;
		}

		public event Action<Projectile> ProjectileFired;
	}
}