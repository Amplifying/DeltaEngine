using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// Provides a way to fetch the current input values from a Touch device.
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