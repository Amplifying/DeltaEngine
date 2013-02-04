using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Allows to detect when a simple touch happens.
	/// </summary>
	public class TouchPressTrigger : Trigger
	{
		public TouchPressTrigger(State state)
		{
			this.state = state;
		}

		private readonly State state;

		public override bool ConditionMatched(Input input)
		{
			return input.touch.GetState(0) == state;
		}
	}
}