using DeltaEngine.Core;
using DeltaEngine.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Rendering.Sprites
{
	public class RotateSpriteByRotationSpeed : EntityHandler
	{
		public void Handle(List<Entity> entities)
		{
			foreach (PhysicsSprite sprite in entities.OfType<PhysicsSprite>())
				sprite.Rotation = sprite.Rotation + sprite.RotationSpeed * Time.Current.Delta;
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}
