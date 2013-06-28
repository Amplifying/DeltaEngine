using System;
using DeltaEngine.Networking;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class DeltaEngineServerLogTests : TestWithMocksOrVisually
	{
		[Test, Category("Slow")]
		public void LogToRealLogServer()
		{
			var logClient = new NetworkClientLogProvider(Resolve<Client>(), LogServerAddress,
				LogServerPort);
			logClient.Log(new Info("Hello TestWorld from " + Environment.MachineName));
		}

		private const string LogServerAddress = "deltaengine.net";
		private const int LogServerPort = 777;
	}
}