using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// For adding Behavior to Entity3D classes
	/// </summary>
	public abstract class Behavior3D : Behavior<Entity>
	{
		internal override void StartedHandling(Entity entity)
		{
			StartedHandling((Entity3D)entity);
		}

		public virtual void StartedHandling(Entity3D entity3D) { }

		internal override void Handle(Entity entity)
		{
			Handle((Entity3D)entity);
		}

		public abstract void Handle(Entity3D entity3D);

		internal override void StoppedHandling(Entity entity)
		{
			StoppedHandling((Entity3D)entity);
		}

		public virtual void StoppedHandling(Entity3D entity3D) { }
	}
}