using System;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadButtonTriggerTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var trigger = new GamePadButtonTrigger(GamePadButton.Y, State.Pressing);
				Assert.False(trigger.ConditionMatched(input));
			});
		}
	}
}
