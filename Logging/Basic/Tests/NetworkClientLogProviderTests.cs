using System;
using System.Net;
using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class NetworkClientLogProviderTests
	{
		[TestFixtureSetUp]
		public void StartLogServer()
		{
			server = new LocalhostLogServer();
			var logServiceAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), LocalhostLogServer.Port);
			logClient = new NetworkClientLogProvider(logServiceAddress);
		}

		private LocalhostLogServer server;
		private NetworkClientLogProvider logClient;

		[TestFixtureTearDown]
		public void ShutdownLogServer()
		{
			if (server != null)
			{
				server.Dispose();
				server = null;
			}

			if (logClient != null)
			{
				logClient.Dispose();
				logClient = null;
			}
		}

		[Test, Category("Slow")]
		public void ConnectionStatus()
		{
			Assert.IsTrue(logClient.IsConnected);
		}

		[Test, Category("Slow")]
		public void LogInfoMessage()
		{
			logClient.Log(new Info("Hello"));
			ExpectThatServerHasReceivedMessage("Hello");
		}

		private void ExpectThatServerHasReceivedMessage(string messageText)
		{
			Assert.That(() => server.LastMessage.Text, Is.EqualTo(messageText).After(30, 5));
		}

		[Test, Category("Slow")]
		public void LogWarning()
		{
			logClient.Log(new Warning("Ohoh"));
			ExpectThatServerHasReceivedMessage("Ohoh");

			logClient.Log(new Warning(new NullReferenceException()));
			ExpectThatServerLastMessageContains("NullReferenceException");
		}

		private void ExpectThatServerLastMessageContains(string messageText)
		{
			Assert.That(() => server.LastMessage.Text.Contains(messageText),
				Is.EqualTo(true).After(50, 5));
		}

		[Test, Category("Slow")]
		public void LogError()
		{
			logClient.Log(new Error(new ArgumentException()));
			ExpectThatServerLastMessageContains("ArgumentException");
		}
	}
}