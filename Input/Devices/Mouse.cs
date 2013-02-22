using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// Provides the mouse position, mouse button states and allows to set the mouse position.
	/// </summary>
	public interface Mouse : InputDevice
	{
		Point Position { get; }
		int ScrollWheelValue { get; }
		State LeftButton { get; }
		State MiddleButton { get; }
		State RightButton { get; }
		State X1Button { get; }
		State X2Button { get; }
		State GetButtonState(MouseButton button);
		void SetPosition(Point newPosition);
	}
}