using System;

namespace DeltaEngine.Input.Triggers
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

		public Key Key
		{
			get { return key; }
		}

		public State State
		{
			get { return state; }
		}

		public override bool ConditionMatched(InputCommands input)
		{
			return input.keyboard.IsAvailable && input.keyboard.GetKeyState(key) == state;
		}

		public bool Equals(KeyTrigger other)
		{
			return other.Key == key && other.state == state;
		}

		public override bool Equals(object other)
		{
			return other is KeyTrigger && Equals((KeyTrigger)other);
		}

		public override int GetHashCode()
		{
			return ((int)key).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}