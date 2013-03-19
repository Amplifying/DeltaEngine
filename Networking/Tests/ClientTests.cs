using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
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
			var serverMessage = server.ReceivedMessage.ToBinaryData() as TestMessage;
			byte[] byteArray = serverMessage.ToArrayWithLengthHeader();
			Assert.AreEqual(19, byteArray.Length);
		}

		[Test]
		public void SendTestMessageToServer()
		{
			var server = new ServerMock();
			Assert.IsNull(server.ReceivedMessage);
			new ClientMock(server).Send(new TestMessage("Hi"));
			var serverMessage = server.ReceivedMessage.ToBinaryData() as TestMessage;
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

		[Test]
		public void DisposeBinaryData()
		{
			BinaryDataFactory binaryDataFactory = new BinaryDataFactory();
			binaryDataFactory.Dispose();
		}

		[Test]
		public void AssemblyLoadedInCurrentDomain()
		{
			AssemblyLoadEventArgs assemblyEvent = new AssemblyLoadEventArgs(Assembly.GetAssembly(GetType()));
			BinaryDataFactory binaryDataFactory = new BinaryDataFactory();
			binaryDataFactory.OnAssemblyLoadInCurrentDomain(null, assemblyEvent);

			Assert.That(binaryDataFactory.shortNames.Count, Is.GreaterThanOrEqualTo(4));
			Assert.That(binaryDataFactory.typeMap.Count, Is.GreaterThanOrEqualTo(4));
		}
	}
}