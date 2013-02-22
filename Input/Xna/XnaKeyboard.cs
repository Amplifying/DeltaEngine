using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Input.Devices;
using Microsoft.Xna.Framework.Input;
using Keyboard = DeltaEngine.Input.Devices.Keyboard;
using NativeKeyboard = Microsoft.Xna.Framework.Input.Keyboard;
using XnaKeys = Microsoft.Xna.Framework.Input.Keys;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native implementation of the Keyboard interface using Xna
	/// </summary>
	public class XnaKeyboard : Keyboard
	{
		public XnaKeyboard()
		{
			IsAvailable = true;
			CreateAndFillKeyStatesDictionary();
		}

		public bool IsAvailable { get; private set; }

		private void CreateAndFillKeyStatesDictionary()
		{
			keyStates = new Dictionary<Key, State>();
			foreach (Key key in Key.A.GetEnumValues())
				keyStates.Add(key, State.Released);
		}

		private Dictionary<Key, State> keyStates;

		public State GetKeyState(Key key)
		{
			return keyStates[key];
		}

		public void Run()
		{
			var keyboardState = NativeKeyboard.GetState();
			UpdateKeyStates(ref keyboardState);
		}

		private void UpdateKeyStates(ref KeyboardState newState)
		{
			var keys = new List<Key>(keyStates.Keys);
			foreach (Key key in keys)
				UpdateKeyState(key, ref newState);
		}

		private void UpdateKeyState(Key key, ref KeyboardState newState)
		{
			bool isXnaKeyPressed = newState.IsKeyDown((XnaKeys)key);
			keyStates[key] = keyStates[key].UpdateOnNativePressing(isXnaKeyPressed);
		}

		public void Dispose() {}
	}
}