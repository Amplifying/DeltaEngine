using System;
using System.Net;
using DeltaEngine.Networking;
using DeltaEngine.Networking.Sockets;

namespace DeltaEngine.Logging.Basic.Tests
{
	internal class LocalhostLogServer : IDisposable
	{
		public LocalhostLogServer(Server server)
		{
			this.server = server;
			server.ClientDataReceived += (client, message) => { LastMessage = message as Info; };
		}

		private readonly Server server;

		public Info LastMessage { get; private set; }

		public void Start()
		{
			server.Start(Port);
		}

		public const int Port = 33333;

		public void Dispose()
		{
			server.Dispose();
		}
	}
}