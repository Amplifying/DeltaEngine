using System;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Triggers;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamepadTriggerTests : TestStarter
	{
		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var trigger = new GamePadButtonTrigger(GamePadButton.A, State.Released);
				Assert.IsTrue(trigger.ConditionMatched(input));
			});
		}

		[Test]
		public void CheckForEquility()
		{
			var trigger = new GamePadButtonTrigger(GamePadButton.A, State.Pressing);
			var otherTrigger = new GamePadButtonTrigger(GamePadButton.B, State.Released);
			Assert.AreNotEqual(trigger, otherTrigger);
			Assert.AreNotEqual(trigger.GetHashCode(), otherTrigger.GetHashCode());

			var copyOfTrigger = new GamePadButtonTrigger(GamePadButton.A, State.Pressing);
			Assert.AreEqual(trigger, copyOfTrigger);
			Assert.AreEqual(trigger.GetHashCode(), copyOfTrigger.GetHashCode());
		}
	}
}