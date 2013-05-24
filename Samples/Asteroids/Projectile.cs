using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.Sprites;

namespace Asteroids
{
	/// <summary>
	/// Game object representing the projectiles fired by the player
	/// </summary>
	public class Projectile : Sprite
	{
		public Projectile(Point startPosition, float angle)
			: base(
				AsteroidsGame.content.Load<Image>("projectile"),
				Rectangle.FromCenter(startPosition, new Size(.02f)), Color.White)
		{
			Add(new SimplePhysics.Data()
			{
				Gravity = Point.Zero,
				Velocity = new Point(MathExtensions.Sin(angle) * Constants.ProjectileVelocity,
					-MathExtensions.Cos(angle) * Constants.ProjectileVelocity)
			});
			Rotation = angle;
			Add<MoveAndDisposeOnBorderCollision>();
			RenderLayer = (int)Constants.RenderLayer.Rockets;
		}
	}
}