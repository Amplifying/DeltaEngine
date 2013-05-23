using System;
using System.Runtime.InteropServices;
using DeltaEngine.Logging.Basic;
using DeltaEngine.Networking;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	[Category("Slow")]
	public class LoggerTests : TestWithMockResolver
	{
		//ncrunch: no coverage start
		[Test]
		public void LogInfoMessage()
		{
			Start(typeof(MockResolver), (Logger logger) =>
			{
				Assert.IsNull(logger.LastMessage);
				logger.Info("Hello");
				Assert.AreEqual("Hello", logger.LastMessage.Text);
			});
		}

		[Test]
		public void LogWarning()
		{
			Start(typeof(MockResolver), (Logger logger) =>
			{
				logger.Warning("Ohoh");
				Assert.AreEqual("Ohoh", logger.LastMessage.Text);
				logger.Warning(new NullReferenceException());
				Assert.IsTrue(logger.LastMessage.Text.Contains("NullReferenceException"));
			});
		}

		[Test]
		public void LogError()
		{
			Start(typeof(MockResolver), (Logger logger) =>
			{
				logger.Error(new ExternalException());
				Assert.IsTrue(logger.LastMessage.Text.Contains("ExternalException"));
			});
		}

		[Test]
		public void DisposeLogger()
		{
			Start(typeof(MockResolver), (Logger logger) =>
			{
				logger.Info("Hello");
				logger.Dispose();
			});
		}

		[Test]
		public void DisposeProviders()
		{
			Start(typeof(MockResolver), (Client client) =>
			{
				Logger logger = new DefaultLogger(client);
				logger.Info("Hello");
				logger.Dispose();
			});
		}
	}
}