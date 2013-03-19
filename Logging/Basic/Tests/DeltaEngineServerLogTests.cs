using System;
using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class DeltaEngineServerLogTests
	{
		[Test, Category("Slow")]
		public void LogToRealLogServer()
		{
			using (var logClient = new NetworkClientLogProvider("deltaengine.net", 777))
				logClient.Log(new Info("Hello TestWorld from " + Environment.MachineName));
		}
	}
}