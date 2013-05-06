using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseMovementTriggerTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void ConditionMatchedWithoutMouse(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var trigger = new MouseMovementTrigger();
				Assert.False(trigger.ConditionMatched(input));
			});
		}

		[Test]
		public void ConditionMatched()
		{
			Start(typeof(MockResolver), (InputCommands input, Mouse mouse) =>
			{
				var trigger = new MouseMovementTrigger();
				Assert.False(trigger.ConditionMatched(input));
				mouse.SetPosition(Point.Zero);
				Assert.True(trigger.ConditionMatched(input));
			});
		}
	}
}