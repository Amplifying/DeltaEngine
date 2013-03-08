using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Triggers;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests.Triggers
{
	public class MouseMovementTriggerTests : TestStarter
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
			var resolver = new TestResolver();
			var input = resolver.Resolve<InputCommands>();
			var trigger = new MouseMovementTrigger();
			Assert.False(trigger.ConditionMatched(input));
			var mouse = resolver.Resolve<Mouse>();
			mouse.SetPosition(Point.Zero);
			Assert.True(trigger.ConditionMatched(input));
		}
	}
}