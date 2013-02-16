using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Allows to track mouse button presses.
	/// </summary>
	public class MouseButtonTrigger : Trigger
	{
		public MouseButtonTrigger(MouseButton button, State state)
		{
			this.button = button;
			this.state = state;
		}

		private readonly MouseButton button;
		private readonly State state;

		public MouseButton Button
		{
			get { return button; }
		}

		public override bool ConditionMatched(InputCommands inputCommands)
		{
			return inputCommands.mouse.IsAvailable && inputCommands.mouse.GetButtonState(button) == state;
		}
	}
}