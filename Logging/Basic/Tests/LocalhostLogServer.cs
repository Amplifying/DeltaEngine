using System.Net;
using DeltaEngine.Datatypes;
using DeltaEngine.Networking;
using DeltaEngine.Networking.Sockets;

namespace DeltaEngine.Logging.Basic.Tests
{
	internal class LocalhostLogServer : TcpNetworkingServer
	{
		public LocalhostLogServer()
			: base(new TcpServerSocket(new IPEndPoint(IPAddress.Loopback, Port)))
		{
		}

		public const int Port = 33333;

		protected override void OnClientDataReceived(ClientConnection client, BinaryData message)
		{
			LastMessage = message as Info;
			base.OnClientDataReceived(client, message);
		}

		public Info LastMessage { get; private set; }
	}
}