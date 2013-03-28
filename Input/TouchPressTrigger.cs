using System;
using DeltaEngine.Core;

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

		private readonly State state;

		public override bool ConditionMatched(InputCommands input, Time time)
		{
			return input.Touch.GetState(0) == state;
		}

		public bool Equals(TouchPressTrigger other)
		{
			return other.state == state;
		}

		public override int GetHashCode()
		{
			return ((int)state).GetHashCode();
		}
	}
}