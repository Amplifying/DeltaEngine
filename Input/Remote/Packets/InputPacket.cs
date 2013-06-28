using DeltaEngine.Networking;

namespace DeltaEngine.Input.Remote.Packets
{
	/// <summary>
	/// The InputPacket is a wrapper of a networking packet, containing base input information.
	/// </summary>
	public abstract class InputPacket : TextMessage
	{
		protected InputPacket(InputDeviceType setType)
		{
			Type = setType;
		}

		public readonly InputDeviceType Type;
		public bool IsAvailable;
	}
}