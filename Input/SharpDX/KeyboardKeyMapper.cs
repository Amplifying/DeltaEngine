using System.Collections.Generic;
using DeltaEngine.Input.Devices;
using DInput = SharpDX.DirectInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Helper class to map all the DirectInput keys to our Key enumeration.
	/// </summary>
	internal class KeyboardKeyMapper
	{
		public KeyboardKeyMapper()
		{
			CreateKeyboardKeyMap();
		}

		private void CreateKeyboardKeyMap()
		{
			keyMap.Add(DInput.Key.Unknown, Key.None);
			keyMap.Add(DInput.Key.Tab, Key.Tab);
			keyMap.Add(DInput.Key.Return, Key.Enter);
			keyMap.Add(DInput.Key.Pause, Key.Pause);
			keyMap.Add(DInput.Key.Capital, Key.CapsLock);
			keyMap.Add(DInput.Key.Escape, Key.Escape);
			keyMap.Add(DInput.Key.Space, Key.Space);
			keyMap.Add(DInput.Key.PageUp, Key.PageUp);
			keyMap.Add(DInput.Key.PageDown, Key.PageDown);
			keyMap.Add(DInput.Key.End, Key.End);
			keyMap.Add(DInput.Key.Home, Key.Home);
			keyMap.Add(DInput.Key.Left, Key.CursorLeft);
			keyMap.Add(DInput.Key.UpArrow, Key.CursorUp);
			keyMap.Add(DInput.Key.Right, Key.CursorRight);
			keyMap.Add(DInput.Key.Down, Key.CursorDown);
			keyMap.Add(DInput.Key.PrintScreen, Key.PrintScreen);
			keyMap.Add(DInput.Key.Insert, Key.Insert);
			keyMap.Add(DInput.Key.Delete, Key.Delete);
			keyMap.Add(DInput.Key.D0, Key.D0);
			keyMap.Add(DInput.Key.D1, Key.D1);
			keyMap.Add(DInput.Key.D2, Key.D2);
			keyMap.Add(DInput.Key.D3, Key.D3);
			keyMap.Add(DInput.Key.D4, Key.D4);
			keyMap.Add(DInput.Key.D5, Key.D5);
			keyMap.Add(DInput.Key.D6, Key.D6);
			keyMap.Add(DInput.Key.D7, Key.D7);
			keyMap.Add(DInput.Key.D8, Key.D8);
			keyMap.Add(DInput.Key.D9, Key.D9);
			keyMap.Add(DInput.Key.A, Key.A);
			keyMap.Add(DInput.Key.B, Key.B);
			keyMap.Add(DInput.Key.C, Key.C);
			keyMap.Add(DInput.Key.D, Key.D);
			keyMap.Add(DInput.Key.E, Key.E);
			keyMap.Add(DInput.Key.F, Key.F);
			keyMap.Add(DInput.Key.G, Key.G);
			keyMap.Add(DInput.Key.H, Key.H);
			keyMap.Add(DInput.Key.I, Key.I);
			keyMap.Add(DInput.Key.J, Key.J);
			keyMap.Add(DInput.Key.K, Key.K);
			keyMap.Add(DInput.Key.L, Key.L);
			keyMap.Add(DInput.Key.M, Key.M);
			keyMap.Add(DInput.Key.N, Key.N);
			keyMap.Add(DInput.Key.O, Key.O);
			keyMap.Add(DInput.Key.P, Key.P);
			keyMap.Add(DInput.Key.Q, Key.Q);
			keyMap.Add(DInput.Key.R, Key.R);
			keyMap.Add(DInput.Key.S, Key.S);
			keyMap.Add(DInput.Key.T, Key.T);
			keyMap.Add(DInput.Key.U, Key.U);
			keyMap.Add(DInput.Key.V, Key.V);
			keyMap.Add(DInput.Key.W, Key.W);
			keyMap.Add(DInput.Key.X, Key.X);
			keyMap.Add(DInput.Key.Y, Key.Y);
			keyMap.Add(DInput.Key.Z, Key.Z);
			keyMap.Add(DInput.Key.LeftWindowsKey, Key.WindowsKey);
			keyMap.Add(DInput.Key.RightWindowsKey, Key.WindowsKey);
			keyMap.Add(DInput.Key.NumberPad0, Key.NumPad0);
			keyMap.Add(DInput.Key.NumberPad1, Key.NumPad1);
			keyMap.Add(DInput.Key.NumberPad2, Key.NumPad2);
			keyMap.Add(DInput.Key.NumberPad3, Key.NumPad3);
			keyMap.Add(DInput.Key.NumberPad4, Key.NumPad4);
			keyMap.Add(DInput.Key.NumberPad5, Key.NumPad5);
			keyMap.Add(DInput.Key.NumberPad6, Key.NumPad6);
			keyMap.Add(DInput.Key.NumberPad7, Key.NumPad7);
			keyMap.Add(DInput.Key.NumberPad8, Key.NumPad8);
			keyMap.Add(DInput.Key.NumberPad9, Key.NumPad9);
			keyMap.Add(DInput.Key.Multiply, Key.Multiply);
			keyMap.Add(DInput.Key.Add, Key.Add);
			keyMap.Add(DInput.Key.Subtract, Key.Subtract);
			keyMap.Add(DInput.Key.Decimal, Key.Decimal);
			keyMap.Add(DInput.Key.Divide, Key.Divide);
			keyMap.Add(DInput.Key.F1, Key.F1);
			keyMap.Add(DInput.Key.F2, Key.F2);
			keyMap.Add(DInput.Key.F3, Key.F3);
			keyMap.Add(DInput.Key.F4, Key.F4);
			keyMap.Add(DInput.Key.F5, Key.F5);
			keyMap.Add(DInput.Key.F6, Key.F6);
			keyMap.Add(DInput.Key.F7, Key.F7);
			keyMap.Add(DInput.Key.F8, Key.F8);
			keyMap.Add(DInput.Key.F9, Key.F9);
			keyMap.Add(DInput.Key.F10, Key.F10);
			keyMap.Add(DInput.Key.F11, Key.F11);
			keyMap.Add(DInput.Key.F12, Key.F12);
			keyMap.Add(DInput.Key.NumberLock, Key.NumLock);
			keyMap.Add(DInput.Key.ScrollLock, Key.Scroll);
			keyMap.Add(DInput.Key.LeftShift, Key.Shift);
			keyMap.Add(DInput.Key.RightShift, Key.Shift);
			keyMap.Add(DInput.Key.LeftControl, Key.Control);
			keyMap.Add(DInput.Key.RightControl, Key.Control);
			keyMap.Add(DInput.Key.LeftAlt, Key.Alt);
			keyMap.Add(DInput.Key.RightAlt, Key.Alt);
			keyMap.Add(DInput.Key.Semicolon, Key.Semicolon);
			keyMap.Add(DInput.Key.Comma, Key.Comma);
			keyMap.Add(DInput.Key.Minus, Key.Minus);
			keyMap.Add(DInput.Key.Period, Key.Period);
			keyMap.Add(DInput.Key.Backslash, Key.Backslash);
			keyMap.Add(DInput.Key.LeftBracket, Key.OpenBrackets);
			keyMap.Add(DInput.Key.RightBracket, Key.CloseBrackets);
			
			// No matching DInput key
			// KeyboardKey.Separator
			// KeyboardKey.Plus
			// KeyboardKey.Question
			// KeyboardKey.Tilde
			// KeyboardKey.ChatPadGreen
			// KeyboardKey.ChatPadOrange
			// KeyboardKey.Pipe
			// KeyboardKey.Quotes
		}

		private readonly Dictionary<DInput.Key, Key> keyMap =
			new Dictionary<DInput.Key, Key>();
		
		public Key Translate(DInput.Key key)
		{
			return keyMap.ContainsKey(key) ? keyMap[key] : Key.None;
		}
	}
}