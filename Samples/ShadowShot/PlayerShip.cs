using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace ShadowShot
{
	public class PlayerShip : Sprite, IDisposable
	{
		public PlayerShip(Image image, Rectangle drawArea)
			: base(image, drawArea)
		{
			timeLastShot = Time.Current.Milliseconds;
			Add(new Velocity2D(Point.Zero, Constants.MaximumObjectVelocity));
			Start<MovementHandler>();
			Start<ProjectileHandler>();
			RenderLayer = (int)Constants.RenderLayer.PlayerShip;
			ProjectileFired += SpawnProjectile;
		}

		private float timeLastShot;

		public void Accelerate(Point accelerateDirection)
		{
			var direction = new Point(accelerateDirection.X * Time.Current.Delta, accelerateDirection.Y);
			Get<Velocity2D>().Accelerate(direction);
		}

		private class MovementHandler : Behavior2D
		{
			public MovementHandler(ScreenSpace screen)
			{
				this.screen = screen;
				Filter = entity => entity is PlayerShip;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity2D entity)
			{
				var ship = (PlayerShip)entity;
				var nextRect = CalculateRectAfterMove(ship);
				MoveEntity(ship, nextRect);
				var velocity2D = ship.Get<Velocity2D>();
				velocity2D.velocity -= velocity2D.velocity * Constants.PlayerDecelFactor *
					Time.Current.Delta;
				ship.Set(velocity2D);
			}

			private static Rectangle CalculateRectAfterMove(PlayerShip entity)
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

				entity.Set(vel);
				entity.Set(rect);
			}
		}

		private class ProjectileHandler : Behavior2D
		{
			public ProjectileHandler()
			{
				Filter = entity => entity is PlayerShip;
			}

			public override void Handle(Entity2D entity)
			{
				var ship = entity as PlayerShip;
				foreach (Projectile projectile in ship.addProjectileList)
					if (projectile.IsActive)
						ship.ActiveProjectileList.Add(projectile);

				ship.addProjectileList.Clear();
			}
		}

		private readonly List<Projectile> addProjectileList = new List<Projectile>();
		public List<Projectile> ActiveProjectileList = new List<Projectile>();

		public event Action<Point> ProjectileFired;

		private void SpawnProjectile(Point point)
		{
			var projectile = new Projectile(ContentLoader.Load<Image>("projectile"), point);
			addProjectileList.Add(projectile);
		}

		public void Fire()
		{
			if (Time.Current.Milliseconds - 1 / PlayerCadance > timeLastShot)
			{
				timeLastShot = Time.Current.Milliseconds;
				if (ProjectileFired != null)
					ProjectileFired(DrawArea.Center);
			}
		}

		private const float PlayerCadance = 0.003f;

		public void Deccelerate()
		{
			Get<Velocity2D>().Accelerate(0.7f);
		}

		public void Dispose()
		{
			ProjectileFired -= SpawnProjectile;

			foreach (Projectile projectile in ActiveProjectileList)
				projectile.Dispose();
			IsActive = false;
		}
	}
}