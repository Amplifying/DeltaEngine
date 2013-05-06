using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseHoverTriggerTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void HoverTriggersIfMouseDoesntMove(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				bool triggered = false;
				input.AddMouseHover(mouse => { triggered = true; });
				mockResolver.input.SetMousePosition(Point.Zero);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.False(triggered);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.True(triggered);
			});
		}

		[IntegrationTest]
		public void HoverDoesntTriggerIfMouseMoves(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				bool triggered = false;
				input.AddMouseHover(mouse => { triggered = true; });
				mockResolver.input.SetMousePosition(Point.Zero);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.False(triggered);
				mockResolver.input.SetMousePosition(Point.Half);
				mockResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.False(triggered);
			});
		}
	}
}