using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseMovementTriggerTests : TestStarter
	{
		[IntegrationTest]
		public void ConditionMatchedWithoutMouse(Type resolver)
		{
			Start(resolver, (InputCommands input, Time time) =>
			{
				var trigger = new MouseMovementTrigger();
				Assert.False(trigger.ConditionMatched(input, time));
			});
		}

		[Test]
		public void ConditionMatched()
		{
			var resolver = new TestResolver();
			var input = resolver.Resolve<InputCommands>();
			var time = resolver.Resolve<Time>();
			var trigger = new MouseMovementTrigger();
			Assert.False(trigger.ConditionMatched(input, time));
			var mouse = resolver.Resolve<Mouse>();
			mouse.SetPosition(Point.Zero);
			Assert.True(trigger.ConditionMatched(input, time));
		}
	}
}