using DeltaEngine.Input.Remote.Packets;
using NUnit.Framework;

namespace DeltaEngine.Input.Remote.Tests
{
	internal class RemoteKeyboardTests
	{
		[Test]
		public void Creation()
		{
			Assert.DoesNotThrow(delegate
			{
				new RemoteKeyboard();
			});
		}

		[Test]
		public void HandleNewPacket()
		{
			var keyboard = new RemoteKeyboard();
			InputPacketsAnalyser analyser = CreateAnalyser(keyboard);
			analyser.HandleNewMessage(CreateTestPacket());

			Assert.True(keyboard.IsAvailable);
			Assert.AreEqual(keyboard.GetKeyState(Key.BackSpace), State.JustPressed);
			Assert.AreEqual(keyboard.GetKeyState(Key.CursorDown), State.IsPressed);
		}
		
		private InputPacketsAnalyser CreateAnalyser(RemoteKeyboard keyboard)
		{
			var analyser = new InputPacketsAnalyser();
			analyser.SetActiveKeyboard(keyboard);
			return analyser;
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
			var keyboard = new RemoteKeyboard();
			keyboard.Run();
		}
	}
}