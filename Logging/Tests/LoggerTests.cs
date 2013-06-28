using System;
using System.Runtime.InteropServices;
using DeltaEngine.Logging.Basic;
using DeltaEngine.Networking;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	[Category("Slow")]
	public class LoggerTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[Test]
		public void LogInfoMessage()
		{
			Assert.IsNull(Logger.Current.LastMessage);
			Logger.Current.Info("Hello");
			Assert.AreEqual("Hello", Logger.Current.LastMessage.Text);
		}

		[Test]
		public void LogWarning()
		{
			Logger.Current.Warning("Ohoh");
			Assert.AreEqual("Ohoh", Logger.Current.LastMessage.Text);
			Logger.Current.Warning(new NullReferenceException());
			Assert.IsTrue(Logger.Current.LastMessage.Text.Contains("NullReferenceException"));
		}

		[Test]
		public void LogError()
		{
			Logger.Current.Error(new ExternalException());
			Assert.IsTrue(Logger.Current.LastMessage.Text.Contains("ExternalException"));
		}

		[Test, Category("Slow")]
		public void DisposeLogger()
		{
			Logger.Current.Info("Hello");
			Logger.Current.Dispose();
		}

		[Test, Category("Slow")]
		public void DisposeProviders()
		{
			Logger logger = new DefaultLogger(Resolve<Client>(), Resolve<Settings>());
			logger.Info("Hello");
			logger.Dispose();
		}
	}
}