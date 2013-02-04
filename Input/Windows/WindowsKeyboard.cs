using System;
using System.Collections.Generic;
using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native keyboard implementation using a windows hook.
	/// </summary>
	public class WindowsKeyboard : WindowsHook, Keyboard
	{
		public WindowsKeyboard()
			: base(KeyboardHookId)
		{
			pressedKeys = new List<Key>();
			releasedKeys = new List<Key>();
			IsAvailable = true;
			CreateAndFillKeyStatesDictionary();
		}

		private void CreateAndFillKeyStatesDictionary()
		{
			keyStates = new Dictionary<Key, State>();
			foreach (Key key in Enum.GetValues(typeof(Key)))
				keyStates.Add(key, State.Released);
		}

		private readonly List<Key> pressedKeys;
		private readonly List<Key> releasedKeys;
		public bool IsAvailable { get; private set; }

		public State GetKeyState(Key key)
		{
			return keyStates[key];
		}

		private Dictionary<Key, State> keyStates;

		public void Run()
		{
			UpdateKeyStates();
			pressedKeys.Clear();
			releasedKeys.Clear();
		}

		private void UpdateKeyStates()
		{
			var keys = new List<Key>(keyStates.Keys);
			foreach (Key key in keys)
				keyStates[key] = ProcessReleasedAndPressedListsForKey(key);
		}

		private State ProcessReleasedAndPressedListsForKey(Key key)
		{
			bool isReleased = releasedKeys.Contains(key);
			bool isPressed = pressedKeys.Contains(key);
			return UpdateInputState(keyStates[key], isReleased, isPressed);
		}

		private State UpdateInputState(State previousState, bool isReleased, bool isPressed)
		{
			if (previousState == State.Pressing && isReleased == false)
				return State.Pressed;
			if (isPressed && isReleased == false)
				return previousState != State.Pressed ? State.Pressing : previousState;
			if (isReleased)
				return State.Releasing;

			return previousState == State.Releasing ? State.Released : previousState;
		}

		protected internal override void HandleProcMessage(IntPtr wParam, IntPtr lParam, int msg)
		{
			var keyCode = (Key)wParam.ToInt32();
			if (IsKeyPressed(lParam.ToInt32()))
				pressedKeys.Add(keyCode);
			else
				releasedKeys.Add(keyCode);
		}

		private static bool IsKeyPressed(int lParam)
		{
			return ((uint)(lParam & 0x80000000) >> 0xFF) != 1;
		}
	}
}