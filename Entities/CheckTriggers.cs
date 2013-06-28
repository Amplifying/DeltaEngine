using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Checks all triggers each frame
	/// </summary>
	internal class CheckTriggers : Behavior<Entity>
	{
		internal override void Handle(Entity entity)
		{
			var triggers = entity.Get<List<Trigger>>();
			foreach (Trigger trigger in triggers.Where(t => t.Condition(entity)))
				trigger.Fire(entity);
		}

		public override Priority Priority
		{
			get { return Priority.High; } //ncrunch: no coverage
		}
	}
}