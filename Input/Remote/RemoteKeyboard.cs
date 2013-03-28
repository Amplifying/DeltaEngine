using System.Collections.Generic;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Remote.Packets;

namespace DeltaEngine.Input.Remote
{
	/// <summary>
	/// Remote Keyboard implementation receiving input information from a networking client.
	/// </summary>
	public class RemoteKeyboard : Keyboard
	{
		public RemoteKeyboard()
		{
			states = new Dictionary<Key, State>();
		}

		public State GetKeyState(Key key)
		{
			return states[key];
		}

		public void Run() {}

		internal void HandleNewPacket(KeyboardPacket packet)
		{
			IsAvailable = packet.IsAvailable;
			packet.UpdateStatesFromDataBytes(states);
		}

		private readonly Dictionary<Key, State> states;
		public bool IsAvailable { get; private set; }

		public void Dispose() {}
	}
}