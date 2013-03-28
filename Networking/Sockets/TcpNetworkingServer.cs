using System.Net;

namespace DeltaEngine.Networking.Sockets
{
	/// <summary>
	/// TCP server implementation using raw sockets
	/// </summary>
	public class TcpNetworkingServer : Server
	{
		public TcpNetworkingServer(IPAddress serverAddress, int listenPort)
			: this(new TcpServerSocket(new IPEndPoint(serverAddress, listenPort)))
		{
		}

		public TcpNetworkingServer(TcpServerSocket socket)
		{
			this.socket = socket;
			SetUpSocketBindingAndRegisterCallback();
		}

		private readonly TcpServerSocket socket;

		private void SetUpSocketBindingAndRegisterCallback()
		{
			socket.ClientConnected += OnClientConnected;
			socket.StartListening();
			ListenPort = socket.ListenPort;
		}

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