using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to fetch the current game pad input values.
	/// </summary>
	public abstract class GamePad : InputDevice
	{
		protected GamePad(GamePadNumber number)
		{
			Number = number;
		}

		protected GamePadNumber Number { get; private set; }
		public abstract bool IsAvailable { get; }

		public abstract Point GetLeftThumbStick();
		public abstract Point GetRightThumbStick();
		public abstract float GetLeftTrigger();
		public abstract float GetRightTrigger();
		public abstract State GetButtonState(GamePadButton button);
		public abstract void Vibrate(float strength);
		public abstract void Run();
		public abstract void Dispose();
	}
}