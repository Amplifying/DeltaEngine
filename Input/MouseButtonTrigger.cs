using System;
using DeltaEngine.Core;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows to track mouse button presses.
	/// </summary>
	public class MouseButtonTrigger : Trigger, IEquatable<MouseButtonTrigger>
	{
		public MouseButtonTrigger(MouseButton button, State state)
		{
			this.button = button;
			this.state = state;
		}

		private readonly MouseButton button;
		private readonly State state;

		public override bool ConditionMatched(InputCommands input, Time time)
		{
			return input.Mouse.IsAvailable && input.Mouse.GetButtonState(button) == state;
		}

		public bool Equals(MouseButtonTrigger other)
		{
			return other.button == button && other.state == state;
		}

		public override int GetHashCode()
		{
			return ((int)button).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}