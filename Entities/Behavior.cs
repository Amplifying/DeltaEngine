using System;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Gets all affected entities injected each frame via Handle (Renderers, Updaters, etc.)
	/// </summary>
	public abstract class Behavior<EntityType> : Handler
		where EntityType : Entity
	{
		protected internal Func<EntityType, bool> Filter { get; set; }
		protected internal Func<EntityType, IComparable> Order { get; set; }
		internal virtual void StartedHandling(EntityType entity) {}
		internal abstract void Handle(EntityType entity);
		internal virtual void StoppedHandling(EntityType entity) {}
	}
}