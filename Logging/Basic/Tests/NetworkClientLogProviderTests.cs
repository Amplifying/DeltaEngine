using System;
using System.Threading;
using DeltaEngine.Networking.Sockets;
using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	public class NetworkClientLogProviderTests
	{
		[TestFixtureSetUp]
		public void StartLogServer()
		{
			server = new LocalhostLogServer(new TcpServer());
			server.Start();
			provider = new NetworkClientLogProvider(new TcpSocket(), LocalHost, LocalhostLogServer.Port);
			provider.Log(new Info("First logging request"));
			WaitForClientResponse();
		}

		private LocalhostLogServer server;
		private NetworkClientLogProvider provider;
		private const string LocalHost = "127.0.0.1";

		private static void WaitForClientResponse(int milliseconds = 10)
		{
			Thread.Sleep(milliseconds);
		}

		[Test, Category("Slow")]
		public void ConnectionStatus()
		{
			Assert.IsTrue(provider.IsConnected);
		}

		[Test, Category("Slow")]
		public void LogInfoMessage()
		{
			provider.Log(new Info("Hello"));
			ExpectThatServerHasReceivedMessage("Hello");
		}

		private void ExpectThatServerHasReceivedMessage(string messageText)
		{
			Assert.That(() => server.LastMessage.Text, Is.EqualTo(messageText).After(100, 5));
		}

		[Test, Category("Slow")]
		public void LogWarning()
		{
			provider.Log(new Warning("Ohoh"));
			ExpectThatServerHasReceivedMessage("Ohoh");

			provider.Log(new Warning(new NullReferenceException()));
			ExpectThatServerLastMessageContains("NullReferenceException");
		}

		private void ExpectThatServerLastMessageContains(string messageText)
		{
			Assert.That(() => server.LastMessage.Text.Contains(messageText),
				Is.EqualTo(true).After(100, 5));
		}

		[Test, Category("Slow")]
		public void LogError()
		{
			provider.Log(new Error(new ArgumentException()));
			ExpectThatServerLastMessageContains("ArgumentException");
		}

		[TestFixtureTearDown]
		public void ShutdownLogServer()
		{
			server.Dispose();
			provider.Dispose();
		}
	}
}