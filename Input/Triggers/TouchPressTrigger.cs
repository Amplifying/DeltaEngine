using System;

namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Allows to detect when a simple touch happens.
	/// </summary>
	public class TouchPressTrigger : Trigger, IEquatable<TouchPressTrigger>
	{
		public TouchPressTrigger(State state)
		{
			this.state = state;
		}

		private readonly State state;

		public override bool ConditionMatched(InputCommands input)
		{
			return input.touch.GetState(0) == state;
		}

		public bool Equals(TouchPressTrigger other)
		{
			return other.state == state;
		}

		public override bool Equals(object other)
		{
			return other is TouchPressTrigger && Equals((TouchPressTrigger)other);
		}

		public override int GetHashCode()
		{
			return ((int)state).GetHashCode();
		}
	}
}