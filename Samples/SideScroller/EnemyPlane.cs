using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;

namespace SideScroller
{
	public class EnemyPlane : Plane
	{
		public EnemyPlane(Image enemyTexture, Point initialPosition)
			: base(enemyTexture, initialPosition, Color.Pink)
		{
			Hitpoints = 5;
			verticalDecelerationFactor = 3.0f;
			verticalAccelerationFactor = 1.5f;
			RenderLayer = (int)DefRenderLayer.Player;
			Add(new Velocity2D(new Point(-0.5f, 0), MaximumSpeed));
			Start<EnemyHandler>();
		}

		public void CheckIfHitAndReact(Point playerShotStartPosition)
		{
			if (Math.Abs(playerShotStartPosition.Y - Center.Y) < 0.1f)
				Hitpoints--;
		}

		private class EnemyHandler : Behavior2D
		{
			public EnemyHandler(ScreenSpace screenSpace)
			{
				this.screenSpace = screenSpace;
				Filter = entity => entity is EnemyPlane;
			}

			private readonly ScreenSpace screenSpace;

			public override void Handle(Entity2D entity)
			{
				var enemy = entity as EnemyPlane;

				if (enemy.defeated)
				{
					enemy.AccelerateVertically(0.02f);
					if (enemy.DrawArea.Top > screenSpace.Viewport.Bottom)
						enemy.IsActive = false;
				}
				else
					FireShotIfRightTime(enemy);

				var newRect = CalculateRectAfterMove(enemy);
				MoveEntity(enemy, newRect);
				var velocity2D = enemy.Get<Velocity2D>();
				velocity2D.velocity.Y -= velocity2D.velocity.Y * enemy.verticalDecelerationFactor *
					Time.Current.Delta;
				enemy.Set(velocity2D);
				enemy.Rotation = RotationAccordingToVerticalSpeed(velocity2D.velocity);

				if (enemy.DrawArea.Right < screenSpace.Viewport.Left)
					entity.IsActive = false;
			}

			private static Rectangle CalculateRectAfterMove(Entity entity)
			{
				var pointAfterVerticalMovement =
					new Point(
						entity.Get<Rectangle>().TopLeft.X +
							entity.Get<Velocity2D>().velocity.X * Time.Current.Delta,
						entity.Get<Rectangle>().TopLeft.Y +
							entity.Get<Velocity2D>().velocity.Y * Time.Current.Delta);

				return new Rectangle(pointAfterVerticalMovement, entity.Get<Rectangle>().Size);
			}

			private static void MoveEntity(Entity entity, Rectangle rect)
			{
				entity.Set(rect);
			}

			private static float RotationAccordingToVerticalSpeed(Point vel)
			{
				return - 50 * vel.Y / MaximumSpeed;
			}

			private static void FireShotIfRightTime(EnemyPlane entity)
			{
				if (entity.timeLastShot - Time.Current.Milliseconds > 1)
				{
					entity.timeLastShot = Time.Current.Milliseconds;
					entity.EnemyFiredShot(entity.Center);
				}
			}
		}

		internal float timeLastShot;
		public event Action<Point> EnemyFiredShot;
	}
}