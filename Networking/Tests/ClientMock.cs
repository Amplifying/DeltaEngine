using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking.Tests
{
	public class ClientMock : Client
	{
		public ClientMock(ServerMock server)
		{
			this.server = server;
			if (server != null)
				Connect();
		}

		private ServerMock server;

		public bool IsConnected
		{
			get { return server != null; }
		}

		public void Connect()
		{
			server.ClientConnectedToServer(this);
		}

		public void Send(BinaryData message)
		{
			if (IsConnected)
				server.ReceivedMessage = message.ToByteArray();
		}

		public void Receive()
		{
			if (DataReceived != null)
				DataReceived(this, null);
		}

		public event Action<ClientConnection, BinaryData> DataReceived;

		public void Dispose()
		{
			if (IsConnected)
				Disconnect();

			server = null;
		}

		public void Disconnect()
		{
			if (IsConnected)
				server.ClientDisconnectedFromServer(this);

			if (Disconnected != null)
				Disconnected(this);
		}

		public event Action<ClientConnection> Disconnected;
	}
}