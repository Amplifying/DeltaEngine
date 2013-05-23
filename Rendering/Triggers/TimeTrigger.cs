using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Triggers
{
	internal class TimeTrigger : Trigger2D
	{
		public override void Update(Entity entity)
		{
			var data = entity.Get<TimeTriggerData>();
			if (Time.Current.CheckEvery(data.Interval))
				entity.Set(entity.Get<Color>() == data.FirstColor ? data.SecondColor : data.FirstColor);
		}
	}
}