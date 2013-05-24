using System;

namespace DeltaEngine.Input
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

		private State state;

		public State State
		{
			get { return state; }
			set { state = value; }
		}

		public override bool ConditionMatched(InputCommands input)
		{
			return input.Touch.GetState(0) == state;
		}

		public bool Equals(TouchPressTrigger other)
		{
			return other.state == state;
		}

		public override int GetHashCode()
		{
			//// ReSharper disable NonReadonlyFieldInGetHashCode
			return ((int)state).GetHashCode();
		}
	}
}