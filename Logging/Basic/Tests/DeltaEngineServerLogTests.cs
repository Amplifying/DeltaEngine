using System;
using DeltaEngine.Networking;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class DeltaEngineServerLogTests : TestWithMockResolver
	{
		[Test, Category("Slow")]
		public void LogToRealLogServer()
		{
			Start(typeof(MockResolver), (Client client) =>
			{
				var logClient = new NetworkClientLogProvider(client, LogServerAddress, LogServerPort);
				logClient.Log(new Info("Hello TestWorld from " + Environment.MachineName));
			});
		}

		private const string LogServerAddress = "deltaengine.net";
		private const int LogServerPort = 777;
	}
}