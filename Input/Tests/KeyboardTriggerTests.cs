using System;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class KeyboardTriggerTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var trigger = new KeyTrigger(Key.Y, State.Pressing);
				Assert.False(trigger.ConditionMatched(input));
			});
		}

		[Test]
		public void CheckForEquility()
		{
			var trigger = new KeyTrigger(Key.A, State.Pressing);
			var otherTrigger = new KeyTrigger(Key.Alt, State.Released);
			Assert.AreNotEqual(trigger, otherTrigger);
			Assert.AreNotEqual(trigger.GetHashCode(), otherTrigger.GetHashCode());
			var copyOfTrigger = new KeyTrigger(Key.A, State.Pressing);
			Assert.AreEqual(trigger, copyOfTrigger);
			Assert.AreEqual(trigger.GetHashCode(), copyOfTrigger.GetHashCode());
		}

		[Test]
		public void SetGetProperties()
		{
			var trigger = new KeyTrigger(Key.A, State.Pressing);
			trigger.Key = Key.B;
			trigger.State = State.Pressed;
			Assert.AreNotEqual(trigger.Key, Key.A);
			Assert.AreNotEqual(trigger.State, State.Pressing);
			Assert.AreEqual(trigger.Key, Key.B);
			Assert.AreEqual(trigger.State, State.Pressed);
		}
	}
}