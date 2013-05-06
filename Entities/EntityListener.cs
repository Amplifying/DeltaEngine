namespace DeltaEngine.Entities
{
	/// <summary>
	/// An EntityHandler that can be sent messages by other EntityHandlers
	/// </summary>
	public interface EntityListener : EntityHandler
	{
		void ReceiveMessage(Entity entity, object message);
	}
}