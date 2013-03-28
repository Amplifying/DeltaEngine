using System.Net.Sockets;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class TcpSocketTests
	{
		[Test]
		public void CeckForEquality()
		{
			var socketOne = new TcpSocket();
			var socketTwo = new TcpSocket();
			Assert.AreNotEqual(socketTwo, socketOne);
			Assert.AreEqual(socketOne, socketOne);
			Assert.AreEqual(new TcpSocket((Socket)socketOne), socketOne);
		}
	}
}
