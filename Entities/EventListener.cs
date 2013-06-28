namespace DeltaEngine.Entities
{
	/// <summary>
	/// A Handler that can be sent messages by other Handlers
	/// </summary>
	public abstract class EventListener : Handler
	{
		internal abstract void ReceiveMessage(Entity entity, object message);

		public virtual void ReceiveMessage(object message) { }
	}
}