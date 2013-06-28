using DeltaEngine.Input.Remote.Packets;
using NUnit.Framework;

namespace DeltaEngine.Input.Remote.Tests
{
	internal class RemoteKeyboardTests
	{
		[Test]
		public void Creation()
		{
			Assert.DoesNotThrow(() => new RemoteKeyboard());
		}

		[Test]
		public void HandleNewPacket()
		{
			var keyboard = new RemoteKeyboard();
			InputPacketsAnalyser analyser = CreateAnalyser(keyboard);
			analyser.HandleNewMessage(CreateTestPacket());

			Assert.True(keyboard.IsAvailable);
			Assert.AreEqual(keyboard.GetKeyState(Key.Backspace), State.Pressing);
			Assert.AreEqual(keyboard.GetKeyState(Key.CursorDown), State.Pressed);
		}
		
		private InputPacketsAnalyser CreateAnalyser(RemoteKeyboard keyboard)
		{
			return new InputPacketsAnalyser { Keyboard = keyboard };
		}

		internal static KeyboardPacket CreateTestPacket()
		{
			var packet = new KeyboardPacket { IsAvailable = true };
			packet.Data[1] = 2;
			packet.Data[15] = 3;
			return packet;
		}

		[Test]
		public void RunCoverage()
		{
			new RemoteKeyboard().Run();
		}
	}
}