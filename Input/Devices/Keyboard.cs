using System;
using DeltaEngine.Core;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// Provides a way to fetch the current input values from a Keyboard device (virtual or real).
	/// </summary>
	public interface Keyboard : InputDevice
	{
		State GetKeyState(Key key);
	}
}