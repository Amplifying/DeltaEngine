using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// EventListeners for Entity2D classes
	/// </summary>
	public abstract class EventListener2D : EventListener
	{
		internal override void ReceiveMessage(Entity entity, object message)
		{
			ReceiveMessage((Entity2D)entity, message);
		}

		public abstract void ReceiveMessage(Entity2D entity, object message);
	}
}
