using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Triggers
{
	class TimeTriggerData
	{
		public TimeTriggerData(Color firstColor, Color secondColor, float interval)
		{
			FirstColor = firstColor;
			SecondColor = secondColor;
			Interval = interval;
		}
			public Color FirstColor;
			public Color SecondColor;
			public float Interval;
	}
}
