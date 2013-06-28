using System;
using DeltaEngine.Input.Remote.Packets;
using DeltaEngine.Networking;
using DeltaEngine.Networking.Sockets;

namespace DeltaEngine.Input.Remote
{
	public sealed class RemoteServer : IDisposable
	{
		public RemoteServer(InputPacketsAnalyser setAnalyser, int port)
		{
			analyser = setAnalyser;
			tcpServer = new TcpServer();
			tcpServer.Start(port);
			tcpServer.ClientDataReceived += OnMessageReceived;
		}

		private readonly TcpServer tcpServer;
		private readonly InputPacketsAnalyser analyser;

		private void OnMessageReceived(Client client, object data)
		{
			var message = (InputPacket)data;
			if (message == null)
				throw new NotAnInputPacket();

			analyser.HandleNewMessage(message);
			if (Received != null)
				Received();
		}

		private class NotAnInputPacket : Exception {}

		public event Action Received;

		public void Dispose()
		{
			tcpServer.Dispose();
		}

		public bool IsRunning
		{
			get { return tcpServer.IsRunning; }
		}
	}
}