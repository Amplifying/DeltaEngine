using System;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class NetworkLoggingTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void NetworkLoggingTest(Type resolver)
		{
			Start(resolver, (Logger logger) =>
			{
				logger.Info("Testing network logging info");
				logger.Warning("Testing network logging warning");
				Assert.Throws<TestingNetworkError>(() => { throw new TestingNetworkError(logger); });
			});
		}

		private class TestingNetworkError : Exception
		{
			public TestingNetworkError(Logger logger)
				: base("Testing network logging error")
			{
				logger.Error(this);
			}
		}
	}
}