using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to fetch the current game pad input values.
	/// </summary>
	public interface GamePad : InputDevice
	{
		Point GetLeftThumbStick();
		Point GetRightThumbStick();
		float GetLeftTrigger();
		float GetRightTrigger();
		State GetButtonState(GamePadButton button);
	}
}