using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class KeyboardTriggerTests : TestStarter
	{
		[IntegrationTest]
		public void ConditionMatched(Type resolver)
		{
			Start(resolver, (InputCommands input, Time time) =>
			{
				var trigger = new KeyTrigger(Key.Y, State.Pressing);
				Assert.False(trigger.ConditionMatched(input, time));
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
	}
}