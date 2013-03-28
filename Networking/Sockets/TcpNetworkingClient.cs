using System;
using System.Net;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Networking.Sockets
{
	/// <summary>
	/// TCP client implementation using raw sockets
	/// </summary>
	public class TcpNetworkingClient : Client
	{
		public TcpNetworkingClient(string serverAddress, int serverPort)
			: this(serverAddress.ToEndPoint(serverPort))
		{
		}

		public TcpNetworkingClient(IPEndPoint serverAddress)
		{
			this.serverAddress = serverAddress;
			InitTcpSocket();
		}

		private void InitTcpSocket()
		{
			socket = new TcpSocket();
			socket.DataReceived += OnDataReceived;
		}

		private readonly IPEndPoint serverAddress;
		private TcpSocket socket;
		public event Action<ClientConnection, BinaryData> DataReceived;

		protected virtual void OnDataReceived(ClientConnection sender, BinaryData receivedData)
		{
			if (DataReceived != null)
				DataReceived(sender, receivedData);
		}

		public void Connect()
		{
			socket.ConnectAndWaitForData(serverAddress);
		}

		public void Send(BinaryData message)
		{
			if (IsConnected)
				socket.Send(message);
		}

		public bool IsConnected
		{
			get { return socket != null && socket.IsConnected; }
		}

		public void Disconnect()
		{
			if (socket != null)
			{
				socket.Dispose();
				socket = null;
			}

			if (Disconnected != null)
				Disconnected(this);
		}

		public void Dispose()
		{
			if (IsConnected)
				Disconnect();
		}

		public event Action<ClientConnection> Disconnected;
	}
}