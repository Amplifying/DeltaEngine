using System;

namespace DeltaEngine.Input
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

		private GamePadButton button;
		private State state;

		public GamePadButton Button
		{
			get { return button; }
			set { button = value; }
		}

		public State State
		{
			get { return state; }
			set { state = value; }
		}

		public override bool ConditionMatched(InputCommands input)
		{
			return input.gamePad.IsAvailable && input.gamePad.GetButtonState(button) == state;
		}

		public bool Equals(GamePadButtonTrigger other)
		{
			return other.button == button && other.state == state;
		}

		public override int GetHashCode()
		{
			//// ReSharper disable NonReadonlyFieldInGetHashCode
			return ((int)button).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}