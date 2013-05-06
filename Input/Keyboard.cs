namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to fetch the current input values from a Keyboard device (virtual or real).
	/// </summary>
	public interface Keyboard : InputDevice
	{
		State GetKeyState(Key key);
	}
}