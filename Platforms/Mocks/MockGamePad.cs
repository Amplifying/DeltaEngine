using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockGamePad : GamePad
	{
		public MockGamePad()
			: base(GamePadNumber.Any)
		{
			gamePadButtonStates = new State[GamePadButton.A.GetCount()];
		}

		private static State[] gamePadButtonStates;

		public override bool IsAvailable
		{
			get { return true; }
		}

		public override Point GetLeftThumbStick()
		{
			return Point.Zero;
		}

		public override Point GetRightThumbStick()
		{
			return Point.Zero;
		}

		public override float GetLeftTrigger()
		{
			return 0.0f;
		}

		public override float GetRightTrigger()
		{
			return 0.0f;
		}

		public override State GetButtonState(GamePadButton button)
		{
			return gamePadButtonStates[(int)button];
		}

		public void SetGamePadState(GamePadButton button, State state)
		{
			gamePadButtonStates[(int)button] = state;
		}

		public override void Vibrate(float strength) { }
		public override void Run() { }
		public override void Dispose() { }
	}
}
