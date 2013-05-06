using NUnit.Framework;

namespace DeltaEngine.Networking.Sockets.Tests
{
	public class TcpNetworkingServerTests
	{
		[Test]
		public void IsRunning()
		{
			var tcpServer = new TcpServer();
			tcpServer.Start(Port);
			Assert.IsTrue(tcpServer.IsRunning);
			tcpServer.Dispose();
		}

		private const int Port = 1;

		[Test]
		public void ThrowException()
		{
			Assert.Throws<TcpServer.ServerSocketMustBeTcpServerSocket>(() => new TcpServer().Start(null));
		}
	}
}
