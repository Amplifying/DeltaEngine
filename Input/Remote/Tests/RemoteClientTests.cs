using System;
using System.Threading;
using System.Threading.Tasks;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Remote.Packets;
using DeltaEngine.Tools.RemoteInputController;
using NUnit.Framework;

namespace DeltaEngine.Input.Remote.Tests
{
	internal class RemoteClientTests
	{
		[Test, Category("Slow")]
		public void SendMousePacket()
		{
			RunTestCodeWithServerAndClientConnection(objects =>
			{
				SendMousePacketToServer(objects.Client);

				while(objects.NumberOfReceivedMessages < 1)
					Thread.Sleep(1);

				Assert.True(objects.Mouse.IsAvailable);
				Assert.AreEqual(objects.Mouse.Position, new Point(0.4f, 0.1f));
				Assert.AreEqual(objects.Mouse.ScrollWheelValue, 15);
			});
		}
		
		[Test, Category("Slow")]
		public void SendTouchPacket()
		{
			RunTestCodeWithServerAndClientConnection(objects =>
			{
				SendTouchPacketToServer(objects.Client);

				while (objects.NumberOfReceivedMessages < 1)
					Thread.Sleep(1);

				Assert.True(objects.Touch.IsAvailable);
				Assert.AreEqual(objects.Touch.GetPosition(4), new Point(0.5f, 0.3f));
				Assert.AreEqual(objects.Touch.GetState(0), State.JustPressed);
				Assert.AreEqual(objects.Touch.GetState(1), State.Released);
			});
		}

		[Test, Category("Slow")]
		public void SendKeyboardPacket()
		{
			RunTestCodeWithServerAndClientConnection(objects =>
			{
				SendKeyboardPacketToServer(objects.Client);

				while (objects.NumberOfReceivedMessages < 1)
					Thread.Sleep(1);

				Assert.True(objects.Keyboard.IsAvailable);
				Assert.AreEqual(objects.Keyboard.GetKeyState(Key.BackSpace), State.JustPressed);
				Assert.AreEqual(objects.Keyboard.GetKeyState(Key.CursorDown), State.IsPressed);
			});
		}
		
		private void SendMousePacketToServer(RemoteClient client)
		{
			MousePacket packet = RemoteMouseTests.CreateTestPacket();
			client.SendPackets(new InputPacket[] { packet });
		}

		private void SendTouchPacketToServer(RemoteClient client)
		{
			TouchPacket packet = RemoteTouchTests.CreateTestPacket();
			client.SendPackets(new InputPacket[] { packet });
		}

		private void SendKeyboardPacketToServer(RemoteClient client)
		{
			KeyboardPacket packet = RemoteKeyboardTests.CreateTestPacket();
			client.SendPackets(new InputPacket[] { packet });
		}

		public static RemoteClient CreateTestClient()
		{
			return new RemoteClient("127.0.0.1", TestPort);
		}
		
		private const int TestPort = 12345;
		
		private void RunTestCodeWithServerAndClientConnection(TestCodeDelegate deleg)
		{
			var mouse = new RemoteMouse();
			var touch = new RemoteTouch();
			var keyboard = new RemoteKeyboard();
			var analyser = new InputPacketsAnalyser();
			analyser.SetActiveMouse(mouse);
			analyser.SetActiveTouch(touch);
			analyser.SetActiveKeyboard(keyboard);
			var server = new RemoteServer(analyser, TestPort);
			RemoteClient client = CreateTestClient();
			Task connection = client.ConnectToServerAsync();
			connection.Wait();

			var data = new TestObjects
			{ Client = client, Mouse = mouse, Touch = touch, Keyboard = keyboard };
			server.Received += delegate
			{
				data.NumberOfReceivedMessages++;
			};

			try
			{
				if (deleg != null)
					deleg(data);
			}
			catch (Exception ex)
			{
				Assert.Fail(ex.ToString());
			}
			finally
			{
				client.Dispose();
				server.Dispose();
			}
		}

		private delegate void TestCodeDelegate(TestObjects objects);

		private class TestObjects
		{
			public RemoteClient Client;
			public RemoteMouse Mouse;
			public RemoteTouch Touch;
			public RemoteKeyboard Keyboard;
			public int NumberOfReceivedMessages;
		}
	}
}