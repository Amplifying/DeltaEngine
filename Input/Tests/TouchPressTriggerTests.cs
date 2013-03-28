using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchPressTriggerTests : TestStarter
	{
		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input, Time time) =>
			{
				var trigger = new TouchPressTrigger(State.Pressing);
				Assert.False(trigger.ConditionMatched(input, time));
			});
		}
	}
}
