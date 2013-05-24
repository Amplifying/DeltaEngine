using System;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamepadTriggerTests : TestWithAllFrameworks
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

		[Test]
		public void SetGetProperties()
		{
			var trigger = new GamePadButtonTrigger(GamePadButton.A, State.Pressing);
			trigger.Button = GamePadButton.B;
			trigger.State = State.Pressed;
			Assert.AreNotEqual(trigger.Button, GamePadButton.A);
			Assert.AreNotEqual(trigger.State, State.Pressing);
			Assert.AreEqual(trigger.Button, GamePadButton.B);
			Assert.AreEqual(trigger.State, State.Pressed);
		}
	}
}