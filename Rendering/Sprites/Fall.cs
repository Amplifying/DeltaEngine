using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// Causes a PhysicsSprite to move and fall under gravity.
	/// When the duration is complete it removes the Entity from the Entity System
	/// </summary>
	public class Fall : EntityHandler
	{
		public Fall(EntitySystem entitySystem)
		{
			this.entitySystem = entitySystem;
		}

		private readonly EntitySystem entitySystem;

		public void Handle(List<Entity> entities)
		{
			foreach (PhysicsSprite sprite in entities.OfType<PhysicsSprite>())
				MoveSprite(sprite);
		}

		private void MoveSprite(PhysicsSprite sprite)
		{
			sprite.Velocity += sprite.Gravity * Time.Current.Delta;
			sprite.Center += sprite.Velocity * Time.Current.Delta;
			sprite.Rotation += sprite.RotationSpeed * Time.Current.Delta;
			var physics = sprite.Get<PhysicsObject>();
			physics.Elapsed += Time.Current.Delta;
			if (sprite.PhysicsDuration > 0.0f && physics.Elapsed >= sprite.PhysicsDuration)
				entitySystem.Remove(sprite);
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}