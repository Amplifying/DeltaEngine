using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class ServerTests
	{
		[Test]
		public void ListenForClients()
		{
			Assert.IsTrue(server.IsRunning);
		}

		private readonly Server server = new ServerMock();

		[Test]
		public void AcceptClients()
		{
			Assert.AreEqual(0, server.ListenPort);
			Assert.AreEqual(0, server.NumberOfConnectedClients);
			Assert.IsNotNull(new ClientMock(server as ServerMock));
			Assert.AreEqual(1, server.NumberOfConnectedClients);
		}
	}
}