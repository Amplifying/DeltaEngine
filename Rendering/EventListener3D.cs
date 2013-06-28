using DeltaEngine.Entities;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// EventListeners for Entity3D classes
	/// </summary>
	public abstract class EventListener3D : EventListener
	{
		internal override void ReceiveMessage(Entity entity, object message)
		{
			ReceiveMessage((Entity3D)entity, message);
		}

		public abstract void ReceiveMessage(Entity3D entity, object message);
	}
}