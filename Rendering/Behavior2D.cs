using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// For adding Behavior to Entity2D classes
	/// </summary>
	public abstract class Behavior2D : Behavior<Entity>
	{
		internal override void StartedHandling(Entity entity)
		{
			StartedHandling((Entity2D)entity);
		}

		public virtual void StartedHandling(Entity2D entity2D) {}

		internal override void Handle(Entity entity)
		{
			Handle((Entity2D)entity);
		}

		public abstract void Handle(Entity2D entity2D);

		internal override void StoppedHandling(Entity entity)
		{
			StoppedHandling((Entity2D)entity);
		}

		public virtual void StoppedHandling(Entity2D entity2D) {}
	}
}