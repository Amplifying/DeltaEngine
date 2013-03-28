using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking
{
	public interface ClientConnection : IDisposable
	{
		bool IsConnected { get; }
		void Send(BinaryData message);
		event Action<ClientConnection, BinaryData> DataReceived;
		event Action<ClientConnection> Disconnected;
	}
}