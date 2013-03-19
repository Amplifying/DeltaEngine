using System.Net;
using System.Threading;
using DeltaEngine.Datatypes;
using DeltaEngine.Networking.Tests;
using NUnit.Framework;

namespace DeltaEngine.Networking.Sockets.Tests
{
	public class SocketTests
	{
		[Test, Category("Slow")]
		public void ConnectAndDisposeClientConnection()
		{
			server.ClientConnected +=
				connection => Assert.AreEqual(1, server.NumberOfConnectedClients);
			server.ClientDisconnected +=
				connection => Assert.AreEqual(0, server.NumberOfConnectedClients);

			var client = CreatedConnectedClient();
			client.Dispose();
			WaitForServerResponse();
		}

		[Test, Category("Slow")]
		public void ConnectClientAndDisposeServer()
		{
			using (var client = CreatedConnectedClient())
			{
				ShutDownServer();
				Assert.IsFalse(client.IsConnected);
			}
		}

		private void ShutDownServer()
		{
			server.Dispose();
			server = null;
			WaitForServerResponse();
		}

		[Test, Category("Slow")]
		public void ConnectClientWithoutServer()
		{
			ShutDownServer();
			using (var client = CreatedConnectedClient())
			{
				client.Send(new Color(3, 3, 3));
				Assert.IsFalse(client.IsConnected);
			}
		}

		private TcpNetworkingServer server;

		[SetUp]
		public void CreateLocalEchoServer()
		{
			var endPoint = new IPEndPoint(IPAddress.Any, 8585);
			server = new EchoServer(new TcpServerSocket(endPoint));
		}

		[TearDown]
		public void DisposeConnectionsSockets()
		{
			if (server != null)
				server.Dispose();
		}

		private Client CreatedConnectedClient()
		{
			var client = new TcpNetworkingClient("127.0.0.1", 8585);
			client.Connect();

			return client;
		}

		[Test, Category("Slow")]
		public void ConnectionWithServerShouldWork()
		{
			using (var client = CreatedConnectedClient())
			{
				Assert.IsTrue(client.IsConnected);
				Assert.IsTrue(server.IsRunning);
			}
		}

		[Test, Category("Slow")]
		public void SendSingleMessageToServer()
		{
			using (var client = CreatedConnectedClient())
			{
				client.Send(new TestMessage("TestMessage"));
			}
		}

		[Test, Category("Slow")]
		public void SendMessagesToServer()
		{
			using (var client = CreatedConnectedClient())
			{
				client.DataReceived += (clientConnection, message) => { clientReceivedResponseCount++; };
				server.ClientDataReceived += (clientConnection, data) => { serverReceivedMessageCount++; };
				WaitForServerResponse();
				Assert.AreEqual(1, server.NumberOfConnectedClients);

				SendTestMessageAndCheckMessageCount(client, 1);
				SendTestMessageAndCheckMessageCount(client, 2);
				SendTestMessageAndCheckMessageCount(client, 3);
			}
		}

		private int serverReceivedMessageCount;
		private int clientReceivedResponseCount;

		[SetUp]
		public void InitializeMessageCounter()
		{
			serverReceivedMessageCount = 0;
			clientReceivedResponseCount = 0;
		}

		private void SendTestMessageAndCheckMessageCount(Client client, int expectedMessageCount)
		{
			client.Send(new TestMessage("TestMessage"));
			WaitForServerResponse();
			Assert.AreEqual(1, server.NumberOfConnectedClients);
			Assert.AreEqual(expectedMessageCount, serverReceivedMessageCount);
			Assert.AreEqual(expectedMessageCount, clientReceivedResponseCount);
		}

		private void WaitForServerResponse(int milliseconds = 30)
		{
			Thread.Sleep(milliseconds);
		}
	}

	public class EchoServer : TcpNetworkingServer
	{
		public EchoServer(TcpServerSocket socket)
			: base(socket)
		{
			ClientDataReceived += (socket1, data) =>
			{
				socket1.Send(new TestMessage("TestMessage"));
			};
		}
	}
}