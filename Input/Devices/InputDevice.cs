using System;
using DeltaEngine.Core;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// All input devices (keyboard, mouse, touch, gamepad) will be updated each frame as Runners.
	/// Only available devices will be included into Commands and event trigger checks.
	/// </summary>
	public interface InputDevice : Runner, IDisposable
	{
		bool IsAvailable { get; }
	}
}