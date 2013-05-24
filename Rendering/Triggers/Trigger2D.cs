using System.Collections.Generic;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Triggers
{
	public abstract class Trigger2D : EntityHandler
	{
		public override void Handle(List<Entity> entities)
		{
			foreach (var entity in entities)
				Update(entity);
		}

		public abstract void Update(Entity entity);

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.High; }
		}
	}
}