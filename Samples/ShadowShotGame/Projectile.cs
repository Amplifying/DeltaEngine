using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace ShadowShotGame
{
	public class Projectile : Sprite, IDisposable
	{
		public Projectile(Image projectileImage, Point centerPoint)
			: base(projectileImage, Rectangle.FromCenter(centerPoint, ProjectileSize))
		{
			RenderLayer = (int)Constants.RenderLayer.Rockets;
			Add(new SimplePhysics.Data { Gravity = Point.Zero, Velocity = new Point(0.0f, -1.0f), });
			Add<MovementHandler>();
		}

		private static readonly Size ProjectileSize = new Size(0.008f, 0.015f);

		internal class MovementHandler : EntityHandler
		{
			public MovementHandler(ScreenSpace screen)
			{
				this.screen = screen;
				Filter = entity => entity is Projectile;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity entity)
			{
				var projectile = entity as Projectile;
				projectile.DrawArea = CalculateFutureDrawArea(projectile);

				if (ObjectHasCrossedScreenBorder(projectile.DrawArea))
					projectile.IsActive = false;
			}

			private static Rectangle CalculateFutureDrawArea(Projectile projectile)
			{
				return
					new Rectangle(
						projectile.DrawArea.TopLeft +
							projectile.Get<SimplePhysics.Data>().Velocity * Time.Current.Delta,
						projectile.DrawArea.Size);
			}

			private bool ObjectHasCrossedScreenBorder(Rectangle objectArea)
			{
				return (objectArea.Right <= screen.Viewport.Left ||
					objectArea.Left >= screen.Viewport.Right || objectArea.Bottom <= screen.Viewport.Top ||
					objectArea.Top >= screen.Viewport.Bottom);
			}
		}

		public void Dispose()
		{
			IsActive = false;
		}
	}
}