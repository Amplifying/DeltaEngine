using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
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
		public PlayerShip(ContentLoader content)
			: base(
				content.Load<Image>(PlayerShipTextureName),
				new Rectangle(Point.Half, PlayerShipSize), Color.White)
		{
			Add(new Velocity2D(Point.Zero, MaximumPlayerVelocity));
			Add<PlayerMovementHandler>();
			Add<FullAutoFire>();
			RenderLayer = (int)AsteroidsRenderLayer.Player;
			projectileTexture = content.Load<Image>(ProjectileTextureName);
		}

		private const string PlayerShipTextureName = "ship2";
		public static readonly Size PlayerShipSize = new Size(.05f);
		private const float MaximumPlayerVelocity = .5f;
		private readonly Image projectileTexture;
		public event Action<Projectile> ProjectileFired;
		public bool IsFireing { get; set; }
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

		private class PlayerMovementHandler : EntityHandler
		{
			public PlayerMovementHandler(ScreenSpace screen)
			{
				this.screen = screen;
			}

			private readonly ScreenSpace screen;

			public override void Handle(List<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var nextRect = CalculateRectAfterMove(entity);
					MoveEntity(entity, nextRect);
					var velocity2D = entity.Get<Velocity2D>();
					velocity2D.velocity -= velocity2D.velocity * PlayerDecelFactor *
						Time.Current.Delta;
					entity.Set(velocity2D);
				}
			}

			private const float PlayerDecelFactor = 0.7f;

			private static Rectangle CalculateRectAfterMove(Entity entity)
			{
				return
					new Rectangle(
						entity.Get<Rectangle>().TopLeft + entity.Get<Velocity2D>().velocity * Time.Current.Delta,
						entity.Get<Rectangle>().Size);
			}

			private void MoveEntity(Entity entity, Rectangle rect)
			{
				StopAtBorder(entity);
				entity.Set(rect);
			}

			private void StopAtBorder(Entity entity)
			{
				var rect = entity.Get<Rectangle>();
				var vel = entity.Get<Velocity2D>();
				if (rect.Left < screen.Viewport.Left)
				{
					vel.velocity.X = 0.02f;
					rect.Left = screen.Viewport.Left;
				}
				if (rect.Right > screen.Viewport.Right)
				{
					vel.velocity.X = -0.02f;
					rect.Right = screen.Viewport.Right;
				}
				if (rect.Top < screen.Viewport.Top)
				{
					vel.velocity.Y = 0.02f;
					rect.Top = screen.Viewport.Top;
				}
				if (rect.Bottom > screen.Viewport.Bottom)
				{
					vel.velocity.Y = -0.02f;
					rect.Bottom = screen.Viewport.Bottom;
				}
				entity.Set(vel);
				entity.Set(rect);
			}
		}

		private class FullAutoFire : EntityHandler
		{
			public FullAutoFire()
			{
				CadenceShotsPerSec = PlayerCadance;
				timeLastShot = Time.Current.Milliseconds;
			}

			private const float PlayerCadance = 0.003f;
			public float CadenceShotsPerSec { get; private set; }
			private float timeLastShot;

			public override void Handle(List<Entity> entities)
			{
				foreach (PlayerShip entity in entities.OfType<PlayerShip>())
					if (entity.IsFireing && Time.Current.Milliseconds - 1 / CadenceShotsPerSec > timeLastShot)
					{
						var projectile = new Projectile(entity.projectileTexture, entity.DrawArea.Center,
							entity.Rotation);
						timeLastShot = Time.Current.Milliseconds;
						if (entity.ProjectileFired != null)
							entity.ProjectileFired.Invoke(projectile);
					}
			}
		}
	}
}