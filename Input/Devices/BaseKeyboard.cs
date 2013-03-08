using System;
using System.Collections.Generic;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// Implements the common properties to share some code in all derived classes.
	/// </summary>
	public abstract class BaseKeyboard : Keyboard
	{
		protected BaseKeyboard()
		{
			keyStates = new Dictionary<Key, State>();
			CreateAndFillKeyStatesDictionary();
		}

		private void CreateAndFillKeyStatesDictionary()
		{
			foreach (Key key in Enum.GetValues(typeof(Key)))
				keyStates.Add(key, State.Released);
		}

		public State GetKeyState(Key key)
		{
			return keyStates[key];
		}

		protected readonly Dictionary<Key, State> keyStates;

		public virtual bool IsAvailable
		{
			get { return true; }
		}

		public abstract void Run();
        public abstract void Dispose();

		protected void UpdateKeyState(Key key, bool nowPressed)
		{
			keyStates[key] = keyStates[key].UpdateOnNativePressing(nowPressed);
		}
	}
}