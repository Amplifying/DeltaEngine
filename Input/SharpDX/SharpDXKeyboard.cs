using System;
using System.Collections.Generic;
using DeltaEngine.Input.Devices;
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
			if (window != null)
				windowHandle = window.Handle;

			keyMapper = new KeyboardKeyMapper();
			IsAvailable = true;
			CreateNativeKeyboard();
			CreateAndFillKeyStatesDictionary();
		}

		private void CreateNativeKeyboard()
		{
			nativeState = new DInput.KeyboardState();
			directInput = new DInput.DirectInput();
			nativeKeyboard = new DInput.Keyboard(directInput);
			nativeKeyboard.SetCooperativeLevel(windowHandle,
				DInput.CooperativeLevel.NonExclusive | DInput.CooperativeLevel.Background);
			nativeKeyboard.Acquire();
		}
		
		private readonly KeyboardKeyMapper keyMapper;
		private readonly IntPtr windowHandle;
		private DInput.KeyboardState nativeState;
		private DInput.DirectInput directInput;
		private DInput.Keyboard nativeKeyboard;
		public bool IsAvailable { get; private set; }

		private void CreateAndFillKeyStatesDictionary()
		{
			keyStates = new Dictionary<Key, State>();
			foreach (Key key in Enum.GetValues(typeof(Key)))
				keyStates.Add(key, State.Released);
		}

		public void Dispose()
		{
			if (nativeKeyboard != null)
			{
				nativeKeyboard.Unacquire();
				nativeKeyboard = null;
			}

			directInput = null;
		}

		public State GetKeyState(Key key)
		{
			return keyStates[key];
		}

		private Dictionary<Key, State> keyStates;

		public void Run()
		{
			nativeKeyboard.GetCurrentState(ref nativeState);
			Array keys = Enum.GetValues(typeof(DInput.Key));
			foreach (DInput.Key key in keys)
				UpdateKey(key);
		}

		private void UpdateKey(DInput.Key key)
		{
			Key deltaKey = keyMapper.Translate(key);
			keyStates[deltaKey] = keyStates[deltaKey].UpdateOnNativePressing(nativeState.IsPressed(key));
		}
	}
}