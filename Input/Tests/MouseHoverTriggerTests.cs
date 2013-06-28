using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseHoverTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void HoverTriggersIfMouseDoesntMove()
		{
			bool triggered = false;
			Input.AddMouseHover(mouse => { triggered = true; });
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.Zero);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.False(triggered);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.True(triggered);
		}

		[Test]
		public void HoverDoesntTriggerIfMouseMoves()
		{
			bool triggered = false;
			Input.AddMouseHover(mouse => { triggered = true; });
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.Zero);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.False(triggered);
			Resolve<MockMouse>().SetMousePositionNextFrame(Point.Half);
			resolver.AdvanceTimeAndExecuteRunners(1.0f);
			Assert.False(triggered);
		}
	}
}