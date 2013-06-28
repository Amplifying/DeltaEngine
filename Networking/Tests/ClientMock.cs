using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Networking.Tests
{
	public class ClientMock : Client
	{
		public ClientMock(ServerMock server)
		{
			this.server = server;
			if (server != null)
				Connect("Target", 0);
		}

		private ServerMock server;

		public bool IsConnected
		{
			get { return server != null; }
		}

		public void Connect(string serverAddress, int serverPort)
		{
			server.ClientConnectedToServer(this);
			if (Connected != null)
				Connected();
		}

		public string TargetAddress
		{
			get { return "Target:0"; }
		}

		public void Send(object message)
		{
			if (IsConnected)
				server.ReceivedMessage = message.ToByteArrayWithTypeInformation();
		}

		public void Receive()
		{
			if (DataReceived != null)
				DataReceived(null);
		}

		public event Action<object> DataReceived;
		public event Action Connected;

		public void Dispose()
		{
			if (IsConnected)
				server.ClientDisconnectedFromServer(this);

			if (Disconnected != null)
				Disconnected();

			server = null;
		}

		public event Action Disconnected;
	}
}