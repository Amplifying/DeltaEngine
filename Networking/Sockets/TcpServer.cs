using System;
using System.Net;
using System.Net.Sockets;

namespace DeltaEngine.Networking.Sockets
{
	/// <summary>
	/// TCP server using raw sockets via TcpServerSocket and a list of TcpSockets for the clients
	/// </summary>
	public sealed class TcpServer : Server
	{
		public override void Start(int listenPort)
		{
			Start(new TcpServerSocket(new IPEndPoint(IPAddress.Any, listenPort)));
		}

		public override void Start(Client serverSocket)
		{
			socket = (TcpServerSocket)serverSocket;
			if (socket == null)
				throw new ServerSocketMustBeTcpServerSocket();

			SetUpSocketBindingAndRegisterCallback();
		}

		private TcpServerSocket socket;

		public class ServerSocketMustBeTcpServerSocket : Exception { }

		private void SetUpSocketBindingAndRegisterCallback()
		{
			socket.ClientConnected += OnClientConnected;
			socket.DataReceived += data => LastMessageReceived = data;
			socket.StartListening();
			ListenPort = socket.ListenPort;
		}

		public object LastMessageReceived { get; private set; }

		public override bool IsRunning
		{
			get { return socket.IsListening; }
		}

		public override void Dispose()
		{
			socket.Dispose();
			base.Dispose();
		}
	}
}