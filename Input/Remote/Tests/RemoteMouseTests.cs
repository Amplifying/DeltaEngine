using DeltaEngine.Datatypes;
using DeltaEngine.Input.Remote.Packets;
using NUnit.Framework;

namespace DeltaEngine.Input.Remote.Tests
{
	internal class RemoteMouseTests
	{
		[Test]
		public void Creation()
		{
			Assert.DoesNotThrow(delegate
			{
				new RemoteMouse();
			});
		}

		[Test]
		public void HandleNewPacket()
		{
			var mouse = new RemoteMouse();
			InputPacketsAnalyser analyser = CreateAnalyser(mouse);
			analyser.HandleNewMessage(CreateTestPacket());

			Assert.True(mouse.IsAvailable);
			Assert.AreEqual(mouse.Position, new Point(0.4f, 0.1f));
			Assert.AreEqual(mouse.ScrollWheelValue, 15);
		}

		private InputPacketsAnalyser CreateAnalyser(RemoteMouse mouse)
		{
			var analyser = new InputPacketsAnalyser();
			analyser.SetActiveMouse(mouse);
			return analyser;
		}

		internal static MousePacket CreateTestPacket()
		{
			return new MousePacket { IsAvailable = true, X = 0.4f, Y = 0.1f, ScrollWheelValue = 15 };
		}

		[Test]
		public void RunAndSetPositionCoverage()
		{
			var mouse = new RemoteMouse();
			mouse.Run();
			mouse.SetPosition(Point.Zero);
		}
	}
}