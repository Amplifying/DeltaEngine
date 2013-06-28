using System;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class NetworkLoggingTests : TestWithMocksOrVisually
	{
		[Test]
		public void NetworkLoggingTest()
		{
			Logger.Current.Info("Testing network logging info");
			Logger.Current.Warning("Testing network logging warning");
			Assert.Throws<TestingNetworkError>(() => { throw new TestingNetworkError(); });
		}

		private class TestingNetworkError : Exception
		{
			public TestingNetworkError()
				: base("Testing network logging error")
			{
				Logger.Current.Error(this);
			}
		}
	}
}