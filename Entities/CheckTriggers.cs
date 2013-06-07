using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Entities
{
	/// <summary>
	/// Checks all triggers each frame
	/// </summary>
	public class CheckTriggers : EntityHandler
	{
		public override void Handle(Entity entity)
		{
			var triggers = entity.Get<List<Trigger>>();
			foreach (Trigger trigger in triggers.Where(t => t.Condition(entity)))
				trigger.Fire(entity);
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.High; } //ncrunch: no coverage
		}
	}
}