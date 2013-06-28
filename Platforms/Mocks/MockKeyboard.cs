using DeltaEngine.Input;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockKeyboard : Keyboard
	{
		public MockKeyboard()
		{
			keyboardStates = new State[(int)Key.NumberOfKeys + 1];
		}

		private static State[] keyboardStates;

		public void SetKeyboardState(Key key, State state)
		{
			keyboardStates[(int)key] = state;
		}

		public bool IsAvailable
		{
			get { return true; }
		}

		public State GetKeyState(Key key)
		{
			return keyboardStates[(int)key];
		}

		public void Run() {}
		public void Dispose() {}
	}
}