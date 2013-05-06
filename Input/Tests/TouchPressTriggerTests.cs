using System;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchPressTriggerTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var trigger = new TouchPressTrigger(State.Pressing);
				Assert.False(trigger.ConditionMatched(input));
			});
		}

		[Test]
		public void SetGetProperties()
		{
			var trigger = new TouchPressTrigger(State.Pressing);
			trigger.State = State.Pressed;
			Assert.AreNotEqual(trigger.State, State.Pressing);
			Assert.AreEqual(trigger.State, State.Pressed);
		}
	}
}
