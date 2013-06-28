using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;

namespace ShadowShot
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
			Start<SimplePhysics.Fall>();
			Start<MoveAndDisposeOnBorderCollision>();
		}

		private class MoveAndDisposeOnBorderCollision : Behavior2D
		{
			public MoveAndDisposeOnBorderCollision(ScreenSpace screen)
			{
				this.screen = screen;
				Filter = entity => entity is Asteroid;
			}

			private readonly ScreenSpace screen;

			public override void Handle(Entity2D asteroid)
			{
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
			Stop<SimplePhysics.Fall>();
			Stop<MoveAndDisposeOnBorderCollision>();
			IsActive = false;
		}
	}
}