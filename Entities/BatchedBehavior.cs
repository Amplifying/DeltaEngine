using System;
using System.Collections.Generic;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Similar to Behavior but receives the entities as a list rather than one at a time
	/// </summary>
	public abstract class BatchedBehavior<EntityType> : Handler
		where EntityType : Entity
	{
		protected internal Func<EntityType, bool> Filter { get; set; }

		protected internal Func<EntityType, IComparable> Order { get; set; }

		internal virtual void StartedHandling(EntityType entity) { }

		internal abstract void Handle(IEnumerable<EntityType> entities);

		internal virtual void StoppedHandling(EntityType entity) { }
	}
}
