using System.Collections.Generic;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// An EntityHandler that can be sent messages by other EntityHandlers
	/// </summary>
	public abstract class EntityListener : EntityHandler
	{
		public override void Handle(List<Entity> entities) {}

		public abstract void ReceiveMessage(Entity entity, object message);
	}
}