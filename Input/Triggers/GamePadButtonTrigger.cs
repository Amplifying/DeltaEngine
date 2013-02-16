using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Trigger implementation for Mouse events.
	/// </summary>
	public class GamePadButtonTrigger : Trigger
	{
		public GamePadButtonTrigger(GamePadButton button, State state)
		{
			this.button = button;
			this.state = state;
		}

		private readonly GamePadButton button;
		private readonly State state;

		public override bool ConditionMatched(InputCommands inputCommands)
		{
			return inputCommands.gamePad.IsAvailable && inputCommands.gamePad.GetButtonState(button) == state;
		}
	}
}