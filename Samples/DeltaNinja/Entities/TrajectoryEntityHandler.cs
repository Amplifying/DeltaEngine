using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;
using System.Linq;
using System.Collections.Generic;

namespace DeltaNinja
{
	class TrajectoryEntityHandler : EntityHandler
	{
		public TrajectoryEntityHandler()
		{
			Filter = entity => entity is Entity2D;
		}

		public override void Handle(Entity entity)
		{
			var entity2D = entity as Entity2D;
			MoveEntity(entity2D);
		}

		private static void MoveEntity(Entity2D entity)
		{
			var physics = entity.Get<SimplePhysics.Data>();
			physics.Velocity += physics.Gravity * Time.Current.Delta;
			entity.Center += physics.Velocity * Time.Current.Delta;
			entity.Rotation += physics.RotationSpeed * Time.Current.Delta;
			physics.Elapsed += Time.Current.Delta;
			if (physics.Duration > 0.0f && physics.Elapsed >= physics.Duration)
				entity.IsActive = false;
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}
