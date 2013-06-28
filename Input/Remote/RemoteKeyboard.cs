using System.Collections.Generic;
using DeltaEngine.Core;
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
			FillKeyStatesDictionary();
		}

		private readonly Dictionary<Key, State> states;

		private void FillKeyStatesDictionary()
		{
			foreach (Key key in Key.A.GetEnumValues())
				states.Add(key, State.Released);
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

		public bool IsAvailable { get; private set; }

		public void Dispose() {}
	}
}