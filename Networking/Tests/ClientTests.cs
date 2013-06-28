using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
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
				client.Send(new TextMessage(""));
			}
		}

		[Test]
		public void ConvertBinaryDataToArray()
		{
			var server = new ServerMock();
			Assert.IsNull(server.ReceivedMessage);
			new ClientMock(server).Send(new TextMessage("Hi"));
			var serverMessage = server.ReceivedMessage.ToBinaryData() as TextMessage;
			byte[] byteArray = serverMessage.ToByteArrayWithLengthHeader();
			const int LenghtOfNetworkMessage = 4;
			const int StringLenghtByte = 1;
			const int StringIsNullBooleanByte = 1;
			int classNameLenght = "TestMessage".Length + StringLenghtByte;
			int textLenght = "Hi".Length + StringLenghtByte + StringIsNullBooleanByte;
			Assert.AreEqual(LenghtOfNetworkMessage + classNameLenght + textLenght, byteArray.Length);
		}

		[Test]
		public void SendTestMessageToServer()
		{
			var server = new ServerMock();
			Assert.IsNull(server.ReceivedMessage);
			new ClientMock(server).Send(new TextMessage("Hi"));
			var serverMessage = server.ReceivedMessage.ToBinaryData() as TextMessage;
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
				client.DataReceived += (binaryData) => eventTriggered = true;
				client.Receive();
				Assert.IsTrue(eventTriggered);
			}
		}
	}
}