using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Triggers
{
	internal class TimeTrigger : EntityHandler
	{
		public override void Handle(Entity entity)
		{
			var data = entity.Get<TimeTriggerData>();
			if (Time.Current.CheckEvery(data.Interval))
				entity.Set(entity.Get<Color>() == data.FirstColor ? data.SecondColor : data.FirstColor);
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.High; }
		}
	}
}