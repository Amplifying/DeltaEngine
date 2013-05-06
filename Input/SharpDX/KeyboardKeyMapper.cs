using System;
using System.Collections.Generic;
using System.Globalization;
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
			KeyMap.Add(ParseDirectInputKeyByName("Left" + directEndName), ParseKeyByName(deltaName));
			KeyMap.Add(ParseDirectInputKeyByName("Right" + directEndName), ParseKeyByName(deltaName));
		}

		private static void AddFunctionKeys()
		{
			for (int i = 1; i <= 12; i++)
				KeyMap.Add(ParseDirectInputKeyByName("F" + i), ParseKeyByName("F" + i));
		}

		private static void AddNumbers()
		{
			for (int i = 0; i <= 9; i++)
			{
				KeyMap.Add(ParseDirectInputKeyByName("D" + i), ParseKeyByName("D" + i));
				KeyMap.Add(ParseDirectInputKeyByName("NumberPad" + i), ParseKeyByName("NumPad" + i));
			}
		}

		private static void AddLetters()
		{
			char currentCharacter = 'A';
			do
			{
				KeyMap.Add(
					ParseDirectInputKeyByName(currentCharacter.ToString(CultureInfo.InvariantCulture)),
					ParseKeyByName(currentCharacter.ToString(CultureInfo.InvariantCulture)));
				currentCharacter++;
			} while (currentCharacter != ('Z' + 1));
		}

		private static void AddSpecialKeys()
		{
			KeyMap.Add(DInput.Key.Unknown, Key.None);
			KeyMap.Add(DInput.Key.Tab, Key.Tab);
			KeyMap.Add(DInput.Key.Return, Key.Enter);
			KeyMap.Add(DInput.Key.Pause, Key.Pause);
			KeyMap.Add(DInput.Key.Capital, Key.CapsLock);
			KeyMap.Add(DInput.Key.Escape, Key.Escape);
			KeyMap.Add(DInput.Key.Space, Key.Space);
			AddHomeEndDeleteBlockKeys();
			AddArrowKeys();
			KeyMap.Add(DInput.Key.PrintScreen, Key.PrintScreen);
			AddMathKeys();
			KeyMap.Add(DInput.Key.NumberLock, Key.NumLock);
			KeyMap.Add(DInput.Key.ScrollLock, Key.Scroll);
			KeyMap.Add(DInput.Key.Semicolon, Key.Semicolon);
			KeyMap.Add(DInput.Key.Comma, Key.Comma);
			KeyMap.Add(DInput.Key.Period, Key.Period);
			KeyMap.Add(DInput.Key.Backslash, Key.Backslash);
			KeyMap.Add(DInput.Key.LeftBracket, Key.OpenBrackets);
			KeyMap.Add(DInput.Key.RightBracket, Key.CloseBrackets);
		}

		private static void AddMathKeys()
		{
			KeyMap.Add(DInput.Key.Multiply, Key.Multiply);
			KeyMap.Add(DInput.Key.Add, Key.Add);
			KeyMap.Add(DInput.Key.Minus, Key.Minus);
			KeyMap.Add(DInput.Key.Subtract, Key.Subtract);
			KeyMap.Add(DInput.Key.Decimal, Key.Decimal);
			KeyMap.Add(DInput.Key.Divide, Key.Divide);
		}

		private static void AddHomeEndDeleteBlockKeys()
		{
			KeyMap.Add(DInput.Key.PageUp, Key.PageUp);
			KeyMap.Add(DInput.Key.PageDown, Key.PageDown);
			KeyMap.Add(DInput.Key.Insert, Key.Insert);
			KeyMap.Add(DInput.Key.Delete, Key.Delete);
			KeyMap.Add(DInput.Key.End, Key.End);
			KeyMap.Add(DInput.Key.Home, Key.Home);
		}

		private static void AddArrowKeys()
		{
			KeyMap.Add(DInput.Key.Left, Key.CursorLeft);
			KeyMap.Add(DInput.Key.UpArrow, Key.CursorUp);
			KeyMap.Add(DInput.Key.Right, Key.CursorRight);
			KeyMap.Add(DInput.Key.Down, Key.CursorDown);
		}

		private static Key ParseKeyByName(string name)
		{
			return (Key)Enum.Parse(typeof(Key), name);
		}

		private static DInput.Key ParseDirectInputKeyByName(string name)
		{
			return (DInput.Key)Enum.Parse(typeof(DInput.Key), name);
		}

		private static readonly Dictionary<DInput.Key, Key> KeyMap =
			new Dictionary<DInput.Key, Key>();

		public static Key Translate(DInput.Key key)
		{
			return KeyMap.ContainsKey(key) ? KeyMap[key] : Key.None;
		}
	}
}