using System;
using DeltaEngine.Core;

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

		private readonly GamePadButton button;
		private readonly State state;

		public override bool ConditionMatched(InputCommands input, Time time)
		{
			return input.gamePad.IsAvailable && input.gamePad.GetButtonState(button) == state;
		}

		public bool Equals(GamePadButtonTrigger other)
		{
			return other.button == button && other.state == state;
		}

		public override int GetHashCode()
		{
			return ((int)button).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}