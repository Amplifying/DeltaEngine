using System;
using System.Runtime.InteropServices;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class LoggerTests : TestStarter
	{
		[IntegrationTest]
		public void LogInfoMessage(Type resolver)
		{
			Start(resolver, (Logger logger) =>
			{
				Assert.IsNull(logger.LastMessage);
				logger.Info("Hello");
				Assert.AreEqual("Hello", logger.LastMessage.Text);
			});

		}

		[IntegrationTest]
		public void LogWarning(Type resolver)
		{

			Start(resolver, (Logger logger) =>
			{
				logger.Warning("Ohoh");
				Assert.AreEqual("Ohoh", logger.LastMessage.Text);
				logger.Warning(new NullReferenceException());
				Assert.IsTrue(logger.LastMessage.Text.Contains("NullReferenceException"));
			});
		}

		[IntegrationTest]
		public void LogError(Type resolver)
		{

			Start(resolver, (Logger logger) =>
			{
				logger.Error(new ExternalException());
				Assert.IsTrue(logger.LastMessage.Text.Contains("ExternalException"));
			});
		}
	}
}