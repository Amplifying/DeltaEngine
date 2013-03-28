using System.Net;
using DeltaEngine.Networking.Sockets;

namespace DeltaEngine.Logging.Basic
{
	public class NetworkClientLogProvider : TcpNetworkingClient, LogProvider
	{
		public NetworkClientLogProvider(string logServerDnsAddress, int serverPort)
			: this(new IPEndPoint(Dns.GetHostEntry(logServerDnsAddress).AddressList[0], serverPort)) { }

		public NetworkClientLogProvider(IPEndPoint serverAddress)
			: base(serverAddress)
		{
			Connect();
		}

		public void Log(Info info)
		{
			Send(info);
		}

		public void Log(Warning warning)
		{
			Send(warning);
		}

		public void Log(Error error)
		{
			Send(error);
		}
	}
}