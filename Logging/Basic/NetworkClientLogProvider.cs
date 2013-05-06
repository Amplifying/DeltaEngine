using System;
using DeltaEngine.Networking;

namespace DeltaEngine.Logging.Basic
{
	public class NetworkClientLogProvider : LogProvider, IDisposable
	{
		public NetworkClientLogProvider(Client client, string serverAddress, int serverPort)
		{
			this.client = client;
			this.serverAddress = serverAddress;
			this.serverPort = serverPort;
		}

		private readonly Client client;
		private readonly string serverAddress;
		private readonly int serverPort;

		public void Log(Info info)
		{
			ConnectClientIfNotDoneYet();
			client.Send(info);
		}

		public bool IsConnected
		{
			get { return client.IsConnected; }
		}

		private void ConnectClientIfNotDoneYet()
		{
			if (!IsConnected)
				client.Connect(serverAddress, serverPort);
		}

		public void Log(Warning warning)
		{
			ConnectClientIfNotDoneYet();
			client.Send(warning);
		}

		public void Log(Error error)
		{
			ConnectClientIfNotDoneYet();
			client.Send(error);
		}

		public void Dispose()
		{
			client.Dispose();
		}
	}
}