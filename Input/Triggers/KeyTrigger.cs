using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Trigger implementation for Keyboard events.
	/// </summary>
	public class KeyTrigger : Trigger
	{
		public KeyTrigger(Key key, State state)
		{
			this.key = key;
			this.state = state;
		}

		private readonly Key key;
		private readonly State state;

		public Key Key
		{
			get { return key; }
		}

		public override bool ConditionMatched(Input input)
		{
			return input.keyboard.IsAvailable && input.keyboard.GetKeyState(key) == state;
		}
	}
}