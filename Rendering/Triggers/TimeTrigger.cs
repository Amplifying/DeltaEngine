using DeltaEngine.Core;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Triggers
{
	internal class TimeTrigger : Behavior2D
	{
		public override void Handle(Entity2D entity)
		{
			var data = entity.Get<TimeTriggerData>();
			if (Time.Current.CheckEvery(data.Interval))
				entity.Color = entity.Color == data.FirstColor ? data.SecondColor : data.FirstColor;
		}

		public override Priority Priority
		{
			get { return Priority.High; }
		}
	}
}