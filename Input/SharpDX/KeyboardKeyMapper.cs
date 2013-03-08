using System;
using System.Collections.Generic;
using DInput = SharpDX.DirectInput;

namespace DeltaEngine.Input.SharpDX
{
	/// <summary>
	/// Helper class to map all the DirectInput keys to our Key enumeration.
	/// </summary>
	internal static class KeyboardKeyMapper
	{
		static KeyboardKeyMapper()
		{
			CreateKeyboardKeyMap();
		}

		/// <summary>
		/// No matching DInput key for Separator, Plus, Question,
		/// Tilde, ChatPadGreen, ChatPadOrange, Pipe, Quotes
		/// </summary>
		private static void CreateKeyboardKeyMap()
		{
			AddFunctionKeys();
			AddNumbers();
			AddLetters();
			AddSpecialKeys();
			AddLeftRightKeys("WindowsKey");
			AddLeftRightKeys("Shift");
			AddLeftRightKeys("Control");
			AddLeftRightKeys("Alt");
		}

		private static void AddLeftRightKeys(string directEndName, string deltaName = null)
		{
			deltaName = deltaName ?? directEndName;
			keyMap.Add(ParseDirectInputKeyByName("Left" + directEndName), ParseKeyByName(deltaName));
			keyMap.Add(ParseDirectInputKeyByName("Right" + directEndName), ParseKeyByName(deltaName));
		}

		private static void AddFunctionKeys()
		{
			for (int i = 1; i <= 12; i++)
				keyMap.Add(ParseDirectInputKeyByName("F" + i), ParseKeyByName("F" + i));
		}

		private static void AddNumbers()
		{
			for (int i = 0; i <= 9; i++)
			{
				keyMap.Add(ParseDirectInputKeyByName("D" + i), ParseKeyByName("D" + i));
				keyMap.Add(ParseDirectInputKeyByName("NumberPad" + i), ParseKeyByName("NumPad" + i));
			}
		}

		private static void AddLetters()
		{
			char currentCharacter = 'A';
			do
			{
				keyMap.Add(ParseDirectInputKeyByName(currentCharacter.ToString()),
					ParseKeyByName(currentCharacter.ToString()));
				currentCharacter++;
			} while (currentCharacter != ('Z' + 1));
		}

		private static void AddSpecialKeys()
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
			keyMap.Add(DInput.Key.Multiply, Key.Multiply);
			keyMap.Add(DInput.Key.Add, Key.Add);
			keyMap.Add(DInput.Key.Subtract, Key.Subtract);
			keyMap.Add(DInput.Key.Decimal, Key.Decimal);
			keyMap.Add(DInput.Key.Divide, Key.Divide);
			keyMap.Add(DInput.Key.NumberLock, Key.NumLock);
			keyMap.Add(DInput.Key.ScrollLock, Key.Scroll);
			keyMap.Add(DInput.Key.Semicolon, Key.Semicolon);
			keyMap.Add(DInput.Key.Comma, Key.Comma);
			keyMap.Add(DInput.Key.Minus, Key.Minus);
			keyMap.Add(DInput.Key.Period, Key.Period);
			keyMap.Add(DInput.Key.Backslash, Key.Backslash);
			keyMap.Add(DInput.Key.LeftBracket, Key.OpenBrackets);
			keyMap.Add(DInput.Key.RightBracket, Key.CloseBrackets);
		}

		private static Key ParseKeyByName(string name)
		{
			return (Key)Enum.Parse(typeof(Key), name);
		}

		private static DInput.Key ParseDirectInputKeyByName(string name)
		{
			return (DInput.Key)Enum.Parse(typeof(DInput.Key), name);
		}

		private static readonly Dictionary<DInput.Key, Key> keyMap =
	new Dictionary<DInput.Key, Key>();

		public static Key Translate(DInput.Key key)
		{
			return keyMap.ContainsKey(key) ? keyMap[key] : Key.None;
		}
	}
}