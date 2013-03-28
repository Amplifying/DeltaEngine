using DeltaEngine.Datatypes;
using DeltaEngine.Input.Remote.Packets;
using NUnit.Framework;

namespace DeltaEngine.Input.Remote.Tests
{
	internal class RemoteTouchTests
	{
		[Test]
		public void Creation()
		{
			Assert.DoesNotThrow(delegate
			{
				new RemoteTouch();
			});
		}

		[Test]
		public void HandleNewPacket()
		{
			var touch = new RemoteTouch();
			InputPacketsAnalyser analyser = CreateAnalyser(touch);
			analyser.HandleNewMessage(CreateTestPacket());

			Assert.True(touch.IsAvailable);
			Assert.AreEqual(touch.GetState(0), State.JustPressed);
			Assert.AreEqual(touch.GetState(1), State.Released);
			Assert.AreEqual(touch.GetPosition(4), new Point(0.5f, 0.3f));
		}

		private InputPacketsAnalyser CreateAnalyser(RemoteTouch touch)
		{
			var analyser = new InputPacketsAnalyser();
			analyser.SetActiveTouch(touch);
			return analyser;
		}

		internal static TouchPacket CreateTestPacket()
		{
			var packet = new TouchPacket { IsAvailable = true };
			packet.States[0] = State.JustPressed;
			packet.States[1] = State.Released;
			packet.Positions[8] = 0.5f;
			packet.Positions[9] = 0.3f;
			return packet;
		}

		[Test]
		public void RunCoverage()
		{
			var touch = new RemoteTouch();
			touch.Run();
		}
	}
}