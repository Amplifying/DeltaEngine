using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class ClientTests
	{
		[Test]
		public void ConnectToServer()
		{
			var server = new ServerMock();
			using (var client = new ClientMock(server))
			{
				Assert.IsTrue(client.IsConnected);
			}
		}

		[Test]
		public void SendTestMessageWithoutServerShouldNotCrash()
		{
			using (var client = new ClientMock(null))
			{
				Assert.IsFalse(client.IsConnected);
				client.Send(new TestMessage(""));
			}
		}

		[Test]
		public void ConvertBinaryDataToArray()
		{
			var server = new ServerMock();
			Assert.IsNull(server.ReceivedMessage);
			new ClientMock(server).Send(new TestMessage("Hi"));
			var serverMessage = server.ReceivedMessage.ToBinaryData<TestMessage>();
			byte[] byteArray = serverMessage.ToByteArrayWithLengthHeader();
			Assert.AreEqual(19, byteArray.Length);
		}

		[Test]
		public void SendTestMessageToServer()
		{
			var server = new ServerMock();
			Assert.IsNull(server.ReceivedMessage);
			new ClientMock(server).Send(new TestMessage("Hi"));
			var serverMessage = server.ReceivedMessage.ToBinaryData<TestMessage>();
			Assert.IsNotNull(serverMessage);
			Assert.AreEqual("Hi", serverMessage.Text);
		}

		[Test]
		public void ReceiveCallback()
		{
			var server = new ServerMock();
			using (var client = new ClientMock(server))
			{
				bool eventTriggered = false;
				client.DataReceived += (clientConnection, binaryData) => eventTriggered = true;
				client.Receive();
				Assert.IsTrue(eventTriggered);
			}
		}
	}
}