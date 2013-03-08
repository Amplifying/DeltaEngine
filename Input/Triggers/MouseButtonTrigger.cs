using System;
using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Triggers
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

		public MouseButton Button
		{
			get { return button; }
		}

		public State State
		{
			get { return state; }
		}

		public override bool ConditionMatched(InputCommands input)
		{
			return input.mouse.IsAvailable && input.mouse.GetButtonState(button) == state;
		}

		public bool Equals(MouseButtonTrigger other)
		{
			return other.Button == Button && other.state == state;
		}

		public override bool Equals(object other)
		{
			return other is MouseButtonTrigger && Equals((MouseButtonTrigger)other);
		}

		public override int GetHashCode()
		{
			return ((int)button).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}