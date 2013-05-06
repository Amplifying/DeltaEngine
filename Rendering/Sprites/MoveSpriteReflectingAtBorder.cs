using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Sprites
{
	public class MoveSpriteReflectingAtBorder : EntityHandler
	{
		public MoveSpriteReflectingAtBorder(ScreenSpace screen)
		{
			this.screen = screen;
		}

		private readonly ScreenSpace screen;

		public void Handle(List<Entity> entities)
		{
			foreach (var sprite in entities.OfType<PhysicsSprite>())
				Move(sprite);
		}

		private void Move(PhysicsSprite sprite)
		{
			sprite.DrawArea =
				new Rectangle(sprite.DrawArea.TopLeft + sprite.Velocity * Time.Current.Delta,
					sprite.DrawArea.Size);
			Point vel = sprite.Velocity;
			vel.ReflectIfHittingBorder(sprite.DrawArea, screen.Viewport);
			sprite.Velocity = vel;
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}