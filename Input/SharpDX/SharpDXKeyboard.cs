using System;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DInput = SharpDX.DirectInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Native implementation of the Keyboard interface using DirectInput.
	/// </summary>
	public class SharpDXKeyboard : BaseKeyboard
	{
		public SharpDXKeyboard(Window window)
		{
			windowHandle = window.Handle;
			CreateNativeKeyboard();
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

		private readonly IntPtr windowHandle;
		private DInput.KeyboardState nativeState;
		private DInput.DirectInput directInput;
		private DInput.Keyboard nativeKeyboard;

		public override void Dispose()
		{
			if (nativeKeyboard != null)
			{
				nativeKeyboard.Unacquire();
				nativeKeyboard = null;
			}

			directInput = null;
		}

		public override void Run()
		{
			nativeKeyboard.GetCurrentState(ref nativeState);
			Array keys = Enum.GetValues(typeof(DInput.Key));
			foreach (DInput.Key key in keys)
				UpdateKeyState(KeyboardKeyMapper.Translate(key), nativeState.IsPressed(key));
		}
	}
}