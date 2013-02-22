using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.Triggers
{
	public class MouseMoveTrigger : Trigger
	{
		public override bool ConditionMatched(Input input)
		{
			if (!input.mouse.IsAvailable)
				return false;

			bool conditionMatched = input.mouse.Position != Position;
			Position = input.mouse.Position;
			return conditionMatched;
		}

		public Point Position { get; private set; }
	}
}