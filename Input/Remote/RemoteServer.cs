using System.Net;
using DeltaEngine.Networking;

namespace DeltaEngine.Input.Remote
{
	public sealed class RemoteServer : TcpNetworkingServer
	{
		public RemoteServer(InputPacketsAnalyser setAnalyser, int port)
			: base(new TcpNetworkingSocket(IPAddress.Any, port))
		{
			analyser = setAnalyser;
			Received += OnMessageReceived;
		}

		private readonly InputPacketsAnalyser analyser;

		private void OnMessageReceived(Message message)
		{
			analyser.HandleNewMessage(message);
		}
	}
}