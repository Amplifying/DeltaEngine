using System;
using System.Collections.Generic;
using DeltaEngine.Platforms;
using DInput = SharpDX.DirectInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Native implementation of the Keyboard interface using DirectInput.
	/// </summary>
	public class SharpDXKeyboard : Keyboard
	{
		public SharpDXKeyboard(Window window)
		{
			windowHandle = window.Handle;
			CreateAndFillKeyStatesDictionary();
			CreateNativeKeyboard();
		}

		private readonly IntPtr windowHandle;

		private void CreateAndFillKeyStatesDictionary()
		{
			foreach (Key key in Enum.GetValues(typeof(Key)))
				keyStates.Add(key, State.Released);
		}

		private readonly Dictionary<Key, State> keyStates = new Dictionary<Key, State>();

		private void CreateNativeKeyboard()
		{
			nativeState = new DInput.KeyboardState();
			directInput = new DInput.DirectInput();
			nativeKeyboard = new DInput.Keyboard(directInput);
			nativeKeyboard.SetCooperativeLevel(windowHandle,
				DInput.CooperativeLevel.NonExclusive | DInput.CooperativeLevel.Background);
			nativeKeyboard.Acquire();
		}

		private DInput.KeyboardState nativeState;
		private DInput.DirectInput directInput;
		private DInput.Keyboard nativeKeyboard;

		public void Dispose()
		{
			if (nativeKeyboard != null)
			{
				nativeKeyboard.Unacquire();
				nativeKeyboard = null;
			}

			directInput = null;
		}

		public bool IsAvailable
		{
			get { return true; }
		}

		public State GetKeyState(Key key)
		{
			return keyStates[key];
		}

		public void Run()
		{
			nativeKeyboard.GetCurrentState(ref nativeState);
			Array keys = Enum.GetValues(typeof(DInput.Key));
			foreach (DInput.Key key in keys)
				UpdateKeyState(KeyboardKeyMapper.Translate(key), nativeState.IsPressed(key));
		}

		private void UpdateKeyState(Key key, bool nowPressed)
		{
			keyStates[key] = keyStates[key].UpdateOnNativePressing(nowPressed);
		}
	}
}