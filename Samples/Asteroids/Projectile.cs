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
	/// Game object representing the projectiles fired by the player
	/// </summary>
	public class Projectile : Sprite
	{
		public Projectile(Image texture, Point startPosition, float angle)
			: base(texture, Rectangle.FromCenter(startPosition, new Size(.005f)))
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
			Start<MoveAndDisposeOnBorderCollision>();
		}

		private const float ProjectileVelocity = .5f;

		private class MoveAndDisposeOnBorderCollision : Behavior2D
		{
			public MoveAndDisposeOnBorderCollision(ScreenSpace screen)
			{
				this.screen = screen;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity2D entity)
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
	}
}