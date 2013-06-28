using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// For adding Batched Behavior to Entity2D classes
	/// </summary>
	public abstract class BatchedBehavior2D : BatchedBehavior<Entity>
	{
		internal override void StartedHandling(Entity entity)
		{
			StartedHandling((Entity2D)entity);
		}

		public virtual void StartedHandling(Entity2D entity2D) { }

		internal override void Handle(IEnumerable<Entity> entities)
		{
			Handle(entities.Cast<Entity2D>());
		}

		public abstract void Handle(IEnumerable<Entity2D> entity2Ds);

		internal override void StoppedHandling(Entity entity)
		{
			StoppedHandling((Entity2D)entity);
		}

		public virtual void StoppedHandling(Entity2D entity2D) { }
	}
}