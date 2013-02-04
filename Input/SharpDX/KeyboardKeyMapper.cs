using System.Collections.Generic;
using DeltaEngine.Input.Devices;
using DInput = SharpDX.DirectInput;

namespace DeltaEngine.Input.SharpDX
{
	internal static class KeyboardKeyMapper
	{
		static KeyboardKeyMapper()
		{
			CreateKeyboardKeyMap();
		}

		private static void CreateKeyboardKeyMap()
		{
			KeyMap.Add(DInput.Key.Unknown, Key.None);
			KeyMap.Add(DInput.Key.Tab, Key.Tab);
			KeyMap.Add(DInput.Key.Return, Key.Enter);
			KeyMap.Add(DInput.Key.Pause, Key.Pause);
			KeyMap.Add(DInput.Key.Capital, Key.CapsLock);
			KeyMap.Add(DInput.Key.Escape, Key.Escape);
			KeyMap.Add(DInput.Key.Space, Key.Space);
			KeyMap.Add(DInput.Key.PageUp, Key.PageUp);
			KeyMap.Add(DInput.Key.PageDown, Key.PageDown);
			KeyMap.Add(DInput.Key.End, Key.End);
			KeyMap.Add(DInput.Key.Home, Key.Home);
			KeyMap.Add(DInput.Key.Left, Key.CursorLeft);
			KeyMap.Add(DInput.Key.UpArrow, Key.CursorUp);
			KeyMap.Add(DInput.Key.Right, Key.CursorRight);
			KeyMap.Add(DInput.Key.Down, Key.CursorDown);
			KeyMap.Add(DInput.Key.PrintScreen, Key.PrintScreen);
			KeyMap.Add(DInput.Key.Insert, Key.Insert);
			KeyMap.Add(DInput.Key.Delete, Key.Delete);
			KeyMap.Add(DInput.Key.D0, Key.D0);
			KeyMap.Add(DInput.Key.D1, Key.D1);
			KeyMap.Add(DInput.Key.D2, Key.D2);
			KeyMap.Add(DInput.Key.D3, Key.D3);
			KeyMap.Add(DInput.Key.D4, Key.D4);
			KeyMap.Add(DInput.Key.D5, Key.D5);
			KeyMap.Add(DInput.Key.D6, Key.D6);
			KeyMap.Add(DInput.Key.D7, Key.D7);
			KeyMap.Add(DInput.Key.D8, Key.D8);
			KeyMap.Add(DInput.Key.D9, Key.D9);
			KeyMap.Add(DInput.Key.A, Key.A);
			KeyMap.Add(DInput.Key.B, Key.B);
			KeyMap.Add(DInput.Key.C, Key.C);
			KeyMap.Add(DInput.Key.D, Key.D);
			KeyMap.Add(DInput.Key.E, Key.E);
			KeyMap.Add(DInput.Key.F, Key.F);
			KeyMap.Add(DInput.Key.G, Key.G);
			KeyMap.Add(DInput.Key.H, Key.H);
			KeyMap.Add(DInput.Key.I, Key.I);
			KeyMap.Add(DInput.Key.J, Key.J);
			KeyMap.Add(DInput.Key.K, Key.K);
			KeyMap.Add(DInput.Key.L, Key.L);
			KeyMap.Add(DInput.Key.M, Key.M);
			KeyMap.Add(DInput.Key.N, Key.N);
			KeyMap.Add(DInput.Key.O, Key.O);
			KeyMap.Add(DInput.Key.P, Key.P);
			KeyMap.Add(DInput.Key.Q, Key.Q);
			KeyMap.Add(DInput.Key.R, Key.R);
			KeyMap.Add(DInput.Key.S, Key.S);
			KeyMap.Add(DInput.Key.T, Key.T);
			KeyMap.Add(DInput.Key.U, Key.U);
			KeyMap.Add(DInput.Key.V, Key.V);
			KeyMap.Add(DInput.Key.W, Key.W);
			KeyMap.Add(DInput.Key.X, Key.X);
			KeyMap.Add(DInput.Key.Y, Key.Y);
			KeyMap.Add(DInput.Key.Z, Key.Z);
			KeyMap.Add(DInput.Key.LeftWindowsKey, Key.WindowsKey);
			KeyMap.Add(DInput.Key.RightWindowsKey, Key.WindowsKey);
			KeyMap.Add(DInput.Key.NumberPad0, Key.NumPad0);
			KeyMap.Add(DInput.Key.NumberPad1, Key.NumPad1);
			KeyMap.Add(DInput.Key.NumberPad2, Key.NumPad2);
			KeyMap.Add(DInput.Key.NumberPad3, Key.NumPad3);
			KeyMap.Add(DInput.Key.NumberPad4, Key.NumPad4);
			KeyMap.Add(DInput.Key.NumberPad5, Key.NumPad5);
			KeyMap.Add(DInput.Key.NumberPad6, Key.NumPad6);
			KeyMap.Add(DInput.Key.NumberPad7, Key.NumPad7);
			KeyMap.Add(DInput.Key.NumberPad8, Key.NumPad8);
			KeyMap.Add(DInput.Key.NumberPad9, Key.NumPad9);
			KeyMap.Add(DInput.Key.Multiply, Key.Multiply);
			KeyMap.Add(DInput.Key.Add, Key.Add);
			KeyMap.Add(DInput.Key.Subtract, Key.Subtract);
			KeyMap.Add(DInput.Key.Decimal, Key.Decimal);
			KeyMap.Add(DInput.Key.Divide, Key.Divide);
			KeyMap.Add(DInput.Key.F1, Key.F1);
			KeyMap.Add(DInput.Key.F2, Key.F2);
			KeyMap.Add(DInput.Key.F3, Key.F3);
			KeyMap.Add(DInput.Key.F4, Key.F4);
			KeyMap.Add(DInput.Key.F5, Key.F5);
			KeyMap.Add(DInput.Key.F6, Key.F6);
			KeyMap.Add(DInput.Key.F7, Key.F7);
			KeyMap.Add(DInput.Key.F8, Key.F8);
			KeyMap.Add(DInput.Key.F9, Key.F9);
			KeyMap.Add(DInput.Key.F10, Key.F10);
			KeyMap.Add(DInput.Key.F11, Key.F11);
			KeyMap.Add(DInput.Key.F12, Key.F12);
			KeyMap.Add(DInput.Key.NumberLock, Key.NumLock);
			KeyMap.Add(DInput.Key.ScrollLock, Key.Scroll);
			KeyMap.Add(DInput.Key.LeftShift, Key.Shift);
			KeyMap.Add(DInput.Key.RightShift, Key.Shift);
			KeyMap.Add(DInput.Key.LeftControl, Key.Control);
			KeyMap.Add(DInput.Key.RightControl, Key.Control);
			KeyMap.Add(DInput.Key.LeftAlt, Key.Alt);
			KeyMap.Add(DInput.Key.RightAlt, Key.Alt);
			KeyMap.Add(DInput.Key.Semicolon, Key.Semicolon);
			KeyMap.Add(DInput.Key.Comma, Key.Comma);
			KeyMap.Add(DInput.Key.Minus, Key.Minus);
			KeyMap.Add(DInput.Key.Period, Key.Period);
			KeyMap.Add(DInput.Key.Backslash, Key.Backslash);
			KeyMap.Add(DInput.Key.LeftBracket, Key.OpenBrackets);
			KeyMap.Add(DInput.Key.RightBracket, Key.CloseBrackets);
			
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

		private static readonly Dictionary<DInput.Key, Key> KeyMap =
			new Dictionary<DInput.Key, Key>();
		
		public static Key Translate(DInput.Key key)
		{
			return KeyMap.ContainsKey(key) ? KeyMap[key] : Key.None;
		}
	}
}