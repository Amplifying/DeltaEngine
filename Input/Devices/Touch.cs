using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// Provides a way to fetch the current input values from a Touch device.
	/// </summary>
	public interface Touch : InputDevice
	{
		Point GetPosition(int touchIndex);
		State GetState(int touchIndex);
	}
}