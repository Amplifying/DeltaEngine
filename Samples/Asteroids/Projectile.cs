using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// Game object representing the projectiles fired by the player
	/// </summary>
	public class Projectile : Sprite
	{
		public Projectile(Image texture, Point startPosition, float angle)
			: base(texture, Rectangle.FromCenter(startPosition, new Size(.02f)), Color.White)
		{
			Rotation = angle;
			RenderLayer = (int)AsteroidsRenderLayer.Rockets;
			Add(new SimplePhysics.Data
			{
				Gravity = Point.Zero,
				Velocity =
					new Point(MathExtensions.Sin(angle) * ProjectileVelocity,
						-MathExtensions.Cos(angle) * ProjectileVelocity)
			});
			Add<MoveAndDisposeOnBorderCollision>();
		}

		private const float ProjectileVelocity = .5f;

		private class MoveAndDisposeOnBorderCollision : EntityHandler
		{
			public MoveAndDisposeOnBorderCollision(ScreenSpace screen)
			{
				this.screen = screen;
			}

			private readonly ScreenSpace screen;

			public override void Handle(List<Entity> entities)
			{
				foreach (Projectile projectile in entities.OfType<Projectile>())
				{
					projectile.DrawArea =
						new Rectangle(
							projectile.DrawArea.TopLeft +
								projectile.Get<SimplePhysics.Data>().Velocity * Time.Current.Delta,
							projectile.DrawArea.Size);

					if (ObjectHasCrossedScreenBorder(projectile.DrawArea))
						projectile.IsActive = false;
				}
			}

			private bool ObjectHasCrossedScreenBorder(Rectangle objectArea)
			{
				return (objectArea.Right <= screen.Viewport.Left ||
					objectArea.Left >= screen.Viewport.Right || objectArea.Bottom <= screen.Viewport.Top ||
					objectArea.Top >= screen.Viewport.Bottom);
			}
		}
	}
}