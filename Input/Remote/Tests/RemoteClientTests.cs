using System.Threading;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Remote.Packets;
using NUnit.Framework;

namespace DeltaEngine.Input.Remote.Tests
{
	internal class RemoteClientTests
	{
		[TestFixtureSetUp]
		public void CreateRemoteServer()
		{
			var mouse = new RemoteMouse();
			var touch = new RemoteTouch();
			var keyboard = new RemoteKeyboard();
			var analyser = new InputPacketsAnalyser();
			analyser.Mouse = mouse;
			analyser.Touch = touch;
			analyser.Keyboard = keyboard;
			var server = new RemoteServer(analyser, ServerPort);
			objects = new TestObjects
			{ Mouse = mouse, Touch = touch, Keyboard = keyboard, Server = server };
		}

		private TestObjects objects;
		private const int ServerPort = 8585;

		[TestFixtureTearDown]
		public void Shutdown()
		{
			if (objects != null)
				objects.Server.Dispose();
			objects = null;
		}

		[Test, Category("Slow"), Ignore]
		public void SendMousePacket()
		{
			using (var client = CreatedConnectedClient())
			{
				WaitForServer();
				Assert.True(objects.Server.IsRunning);
				Assert.True(client.IsConnected);
				SendMousePacketToServer(client);
				WaitForServer();
				Assert.True(objects.Mouse.IsAvailable);
				Assert.AreEqual(objects.Mouse.Position, new Point(0.4f, 0.1f));
				Assert.AreEqual(objects.Mouse.ScrollWheelValue, 15);
			}
		}

		private static RemoteClient CreatedConnectedClient()
		{
			var client = new RemoteClient(ServerAddress, ServerPort);
			client.ConnectToServerAsync();
			return client;
		}

		private const string ServerAddress = "127.0.0.1";

		private static void WaitForServer(int milliseconds = 10)
		{
			Thread.Sleep(milliseconds);
		}

		private static void SendMousePacketToServer(RemoteClient client)
		{
			MousePacket packet = RemoteMouseTests.CreateTestPacket();
			client.SendPackets(packet);
		}

		[Test, Category("Slow"), Ignore]
		public void SendTouchPacket()
		{
			using (var client = CreatedConnectedClient())
			{
				WaitForServer();
				Assert.True(objects.Server.IsRunning);
				SendTouchPacketToServer(client);
				WaitForServer();
				Assert.True(objects.Touch.IsAvailable);
				Assert.AreEqual(objects.Touch.GetPosition(4), new Point(0.5f, 0.3f));
				Assert.AreEqual(objects.Touch.GetState(0), State.Pressing);
				Assert.AreEqual(objects.Touch.GetState(1), State.Released);
			}
		}

		private static void SendTouchPacketToServer(RemoteClient client)
		{
			TouchPacket packet = RemoteTouchTests.CreateTestPacket();
			client.SendPackets(packet);
		}

		[Test, Category("Slow"), Ignore]
		public void SendKeyboardPacket()
		{
			using (var client = CreatedConnectedClient())
			{
				WaitForServer();
				Assert.True(objects.Server.IsRunning);
				SendKeyboardPacketToServer(client);
				WaitForServer();
				Assert.True(objects.Keyboard.IsAvailable);
				Assert.AreEqual(objects.Keyboard.GetKeyState(Key.Backspace), State.Pressing);
				Assert.AreEqual(objects.Keyboard.GetKeyState(Key.CursorDown), State.Pressed);
			}
		}

		private static void SendKeyboardPacketToServer(RemoteClient client)
		{
			KeyboardPacket packet = RemoteKeyboardTests.CreateTestPacket();
			client.SendPackets(packet);
		}

		private class TestObjects
		{
			public RemoteServer Server;
			public RemoteMouse Mouse;
			public RemoteTouch Touch;
			public RemoteKeyboard Keyboard;
		}
	}
}