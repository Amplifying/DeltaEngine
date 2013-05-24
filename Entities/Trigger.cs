using System;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Can be attached to an Entity and represents a condition which will trigger an action.
	/// eg. Making an Entity bounce when it reaches the edge of the screen.
	/// </summary>
	public class Trigger
	{
		public Trigger(Func<Entity, bool> condition)
		{
			Condition = condition;
		}

		public Func<Entity, bool> Condition { get; set; }

		internal void Fire(Entity entity)
		{
			if (Fired != null)
				Fired(entity);
		}

		public event Action<Entity> Fired;
	}
}