using System;

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

		private Key key;
		private State state;

		public Key Key
		{
			get { return key; }
			set { key = value; }
		}

		public State State
		{
			get { return state; }
			set { state = value; }
		}

		public override bool ConditionMatched(InputCommands input)
		{
			return input.keyboard.IsAvailable && input.keyboard.GetKeyState(key) == state;
		}

		public bool Equals(KeyTrigger other)
		{
			return other.key == key && other.state == state;
		}

		public override int GetHashCode()
		{
			//// ReSharper disable NonReadonlyFieldInGetHashCode
			return ((int)key).GetHashCode() ^ ((int)state).GetHashCode();
		}
	}
}