using System;

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

		private MouseButton button;
		private State state;
	
		public MouseButton Button
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
			return input.Mouse.IsAvailable && input.Mouse.GetButtonState(button) == state;
		}

		public bool Equals(MouseButtonTrigger other)
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