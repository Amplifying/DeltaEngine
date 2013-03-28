using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseHoverTriggerTests : TestStarter
	{
		[IntegrationTest]
		public void HoverTriggersIfMouseDoesntMove(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				bool triggered = false;
				input.AddMouseHover(mouse => { triggered = true; });
				testResolver.SetMousePosition(Point.Zero);
				testResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.False(triggered);
				testResolver.AdvanceTimeAndExecuteRunners(1.0f);
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
				testResolver.SetMousePosition(Point.Zero);
				testResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.False(triggered);
				testResolver.SetMousePosition(Point.Half);
				testResolver.AdvanceTimeAndExecuteRunners(1.0f);
				Assert.False(triggered);
			});
		}
	}
}