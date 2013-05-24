using System.Collections.Generic;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Gets all affected entities injected each frame via Handle (Renderers, Updaters, etc.)
	/// </summary>
	public abstract class EntityHandler
	{
		public abstract void Handle(List<Entity> entities);

		public virtual EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.Normal; }
		}
	}
}