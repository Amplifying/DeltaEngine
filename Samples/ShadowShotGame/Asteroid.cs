using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace ShadowShotGame
{
	public class Asteroid : Sprite, IDisposable
	{
		public Asteroid(Image asteroidImage, Rectangle drawArea)
			: base(asteroidImage, drawArea)
		{
			RenderLayer = (int)Constants.RenderLayer.Asteroids;
			Add(new SimplePhysics.Data
			{
				Gravity = Point.Zero,
				Velocity = new Point(0.0f, 0.1f),
				RotationSpeed = 30.0f
			});
			Add<SimplePhysics.Fall>();
			Add<MoveAndDisposeOnBorderCollision>();
		}

		internal class MoveAndDisposeOnBorderCollision : EntityHandler
		{
			public MoveAndDisposeOnBorderCollision(ScreenSpace screen)
			{
				this.screen = screen;
				Filter = entity => entity is Asteroid;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity entity)
			{
				var asteroid = entity as Asteroid;
				if (ObjectHasCrossedScreenBorder(asteroid.DrawArea))
					asteroid.IsActive = false;
			}

			private bool ObjectHasCrossedScreenBorder(Rectangle objectArea)
			{
				return (objectArea.Top >= screen.Viewport.Bottom);
			}
		}

		public void Dispose()
		{
			Remove<SimplePhysics.Data>();
			Remove<SimplePhysics.Fall>();
			Remove<MoveAndDisposeOnBorderCollision>();
			IsActive = false;
		}
	}
}