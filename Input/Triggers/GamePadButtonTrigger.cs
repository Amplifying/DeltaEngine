using System;

namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Trigger implementation for Mouse events.
	/// </summary>
	public class GamePadButtonTrigger : Trigger, IEquatable<GamePadButtonTrigger>
	{
		public GamePadButtonTrigger(GamePadButton button, State state)
		{
			this.button = button;
			this.state = state;
		}

		private readonly GamePadButton button;
		private readonly State state;

		public GamePadButton Button
		{
			get { return button; }
		}

		public override bool ConditionMatched(InputCommands input)
		{
			return input.gamePad.IsAvailable && input.gamePad.GetButtonState(button) == state;
		}

		public bool Equals(GamePadButtonTrigger other)
		{
			return other.Button == button && other.state == state;
		}

		public override bool Equals(object other)
		{
			return other is GamePadButtonTrigger && Equals((GamePadButtonTrigger)other);
		}

		public override int GetHashCode()
		{
			return ((int)button).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}