using System;
using DeltaEngine.Input.Remote.Packets;
using DeltaEngine.Networking.Sockets;

namespace DeltaEngine.Input.Remote
{
	public class RemoteClient : IDisposable
	{
		public RemoteClient(string serverIP, int serverPort)
		{
			tcpSocket = new TcpSocket();
			this.serverIP = serverIP;
			this.serverPort = serverPort;
		}

		private readonly TcpSocket tcpSocket;
		private readonly string serverIP;
		private readonly int serverPort;

		public void ConnectToServerAsync()
		{
			tcpSocket.Connect(serverIP, serverPort);
		}

		public void SendPackets(params InputPacket[] packets)
		{
			for (int i = 0; i < packets.Length; i++)
				tcpSocket.Send(packets[i]);
		}

		public void Dispose()
		{
			tcpSocket.Dispose();
		}

		public bool IsConnected
		{
			get { return tcpSocket.IsConnected; }
		}
	}
}
