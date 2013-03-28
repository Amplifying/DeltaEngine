using System;
using DeltaEngine.Core;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Trigger implementation for Keyboard events.
	/// </summary>
	public class KeyTrigger : Trigger, IEquatable<KeyTrigger>
	{
		public KeyTrigger(Key key, State state)
		{
			this.key = key;
			this.state = state;
		}

		private readonly Key key;
		private readonly State state;

		public override bool ConditionMatched(InputCommands input, Time time)
		{
			return input.keyboard.IsAvailable && input.keyboard.GetKeyState(key) == state;
		}

		public bool Equals(KeyTrigger other)
		{
			return other.key == key && other.state == state;
		}

		public override int GetHashCode()
		{
			return ((int)key).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}