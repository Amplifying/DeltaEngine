using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using Pencil.Gaming;

namespace DeltaEngine.Input.GLFW
{
	/// <summary>
	/// Native implementation of the Keyboard interface using GLFW
	/// </summary>
	public class GLFWKeyboard : Keyboard
	{
		public GLFWKeyboard()
		{
			IsAvailable = true;
			CreateAndFillKeyStatesDictionary();
		}

		public bool IsAvailable { get; private set; }

		public void Dispose()
		{
			IsAvailable = false;
		}

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

		public void HandleInput(ref string inputText) {}

		public void Run()
		{
			UpdateKeyStates();
		}

		private void UpdateKeyStates()
		{
			var keys = new List<Key>(keyStates.Keys);
			foreach (Key key in keys)
				UpdateKeyState(key);
		}

		private void UpdateKeyState(Key key)
		{
			Pencil.Gaming.Key pencilKey;
			bool isKeyPressed = Enum.TryParse(key.ToString(), out pencilKey)
				? Glfw.GetKey(pencilKey) : Glfw.GetKey((Pencil.Gaming.Key)key);

			keyStates[key] = keyStates[key].UpdateOnNativePressing(isKeyPressed);
		}
	}
}