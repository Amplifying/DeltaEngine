namespace DeltaEngine.Entities
{
	/// <summary>
	/// An EntityHandler that can be sent messages by other EntityHandlers
	/// </summary>
	public abstract class EntityListener : EntityHandler
	{
		public override void Handle(Entity entity) {}

		public abstract void ReceiveMessage(Entity entity, object message);
	}
}