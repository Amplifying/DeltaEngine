using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// Implements the common properties to share some code in all derived classes.
	/// </summary>
	public abstract class BaseGamePad : GamePad
	{
		protected BaseGamePad()
		{
			states = new State[GamePadButton.A.GetCount()];
			for (int i = 0; i < states.Length; i++)
				states[i] = State.Released;
		}

		protected readonly State[] states;

		public Point GetLeftThumbStick()
		{
			return leftThumbStick;
		}

		protected Point leftThumbStick;

		public Point GetRightThumbStick()
		{
			return rightThumbStick;
		}

		protected Point rightThumbStick;

		public float GetLeftTrigger()
		{
			return leftTrigger;
		}

		protected float leftTrigger;

		public float GetRightTrigger()
		{
			return rightTrigger;
		}

		protected float rightTrigger;

		public State GetButtonState(GamePadButton button)
		{
			return states[(int)button];
		}

		public abstract bool IsAvailable { get; }
		public abstract void Run();
        public abstract void Dispose();

		protected void UpdateButton(GamePadButton button, bool nowPressed)
		{
			int buttonIndex = (int)button;
			states[buttonIndex] = states[buttonIndex].UpdateOnNativePressing(nowPressed);
		}
	}
}