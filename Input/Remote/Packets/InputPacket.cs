using System;
using DeltaEngine.Networking;

namespace DeltaEngine.Input.Remote.Packets
{
	/// <summary>
	/// The InputPacket is a wrapper of a networking packet, containing base input information.
	/// </summary>
	[Serializable]
	public abstract class InputPacket : Message
	{
		protected InputPacket(InputDeviceType setType)
		{
			Type = setType;
		}

		public readonly InputDeviceType Type;
		public bool IsAvailable;
	}
}