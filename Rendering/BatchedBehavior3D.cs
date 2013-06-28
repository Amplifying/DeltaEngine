using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// For adding Batched Behavior to Entity2D classes
	/// </summary>
	public abstract class BatchedBehavior3D : BatchedBehavior<Entity>
	{
		internal override void StartedHandling(Entity entity)
		{
			StartedHandling((Entity3D)entity);
		}

		public virtual void StartedHandling(Entity3D entity3D) { }

		internal override void Handle(IEnumerable<Entity> entities)
		{
			Handle(entities.Cast<Entity3D>());
		}

		public abstract void Handle(IEnumerable<Entity3D> entity3Ds);

		internal override void StoppedHandling(Entity entity)
		{
			StoppedHandling((Entity3D)entity);
		}

		public virtual void StoppedHandling(Entity3D entity3D) { }
	}
}