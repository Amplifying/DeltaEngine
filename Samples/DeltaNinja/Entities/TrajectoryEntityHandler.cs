using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;

namespace DeltaNinja.Entities
{
	class TrajectoryEntityHandler : Behavior2D
	{
		public override void Handle(Entity2D entity)
		{
			var sprite = entity as MovingSprite;
			if (sprite == null) return;
			if (sprite.IsPaused) return;
			MoveEntity(sprite);
		}

		private static void MoveEntity(MovingSprite sprite)
		{
			var physics = sprite.Get<SimplePhysics.Data>();
			physics.Velocity += physics.Gravity * Time.Current.Delta;
			sprite.Center += physics.Velocity * Time.Current.Delta;
			sprite.Rotation += physics.RotationSpeed * Time.Current.Delta;
			physics.Elapsed += Time.Current.Delta;
			if (physics.Duration > 0.0f && physics.Elapsed >= physics.Duration)
				sprite.IsActive = false;
		}

		public override Priority Priority
		{
			get { return Priority.First; }
		}
	}
}
