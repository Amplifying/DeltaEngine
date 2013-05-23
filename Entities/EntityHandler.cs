using System.Collections.Generic;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Gets all affected entities injected each frame via Handle (Renderers, Updaters, etc.)
	/// </summary>
	public interface EntityHandler
	{
		void Handle(List<Entity> entities);
		EntityHandlerPriority Priority { get; }
	}
}