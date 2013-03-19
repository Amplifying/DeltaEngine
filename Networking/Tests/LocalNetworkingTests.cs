using System.Net;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	internal class LocalNetworkingTests
	{
		[Test]
		public void ConnectAndDisconnectWithClientAtServer()
		{
			using (var server = StartLocalServer())
			using (var client = new LocalClient())
			{
				Assert.AreEqual(0, server.ConnectedClientsCount);

				ConnectClientToServerAndCheckState(client, server);
				Assert.AreEqual(1, server.ConnectedClientsCount);

				client.Disconnect();
				Assert.IsFalse(client.IsConnected);
				Assert.AreEqual(0, server.ConnectedClientsCount);
			}
		}

		private LocalServer StartLocalServer(int listenPort = 40000)
		{
			var startedServer = LocalServer.Resolve(new IPEndPoint(IPAddress.Loopback, listenPort));
			return startedServer;
		}

		private void ConnectClientToServerAndCheckState(LocalClient client, LocalServer server)
		{
			client.Connect(server.Address);
			Assert.IsTrue(client.IsConnected);
		}

		[Test]
		public void CheckForClientDoubleConnection()
		{
			using (var server = StartLocalServer(40100))
			using (var client = new LocalClient())
			{
				Assert.AreEqual(0, server.ConnectedClientsCount);

				ConnectClientToServerAndCheckState(client, server);
				Assert.AreEqual(1, server.ConnectedClientsCount);

				ConnectClientToServerAndCheckState(client, server);
				Assert.AreEqual(1, server.ConnectedClientsCount);
			}
		}

		[Test]
		public void ConnectWithTwoClientsToServer()
		{
			using (var server = StartLocalServer(40200))
			using (var client1 = new LocalClient())
			using (var client2 = new LocalClient())
			{
				Assert.AreEqual(0, server.ConnectedClientsCount);

				ConnectClientToServerAndCheckState(client1, server);
				Assert.AreEqual(1, server.ConnectedClientsCount);

				ConnectClientToServerAndCheckState(client2, server);
				Assert.AreEqual(2, server.ConnectedClientsCount);
			}
		}
	}
}