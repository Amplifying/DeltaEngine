using System;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Gets all affected entities injected each frame via Handle (Renderers, Updaters, etc.)
	/// </summary>
	public abstract class EntityHandler
	{
		public Func<Entity, bool> Filter { get; set; }

		public Func<Entity, IComparable> Order { get; set; }

		public abstract void Handle(Entity entity);

		public virtual EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.Normal; }
		}
	}
}