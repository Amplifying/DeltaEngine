using System;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseButtonTriggerTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void ConditionMatchedWithoutMouse(Type resolver)
		{
			Start(resolver, (InputCommands input) =>
			{
				var trigger = new MouseButtonTrigger(MouseButton.Right, State.Pressing);
				Assert.False(trigger.ConditionMatched(input));
			});
		}

		[Test]
		public void ConditionMatched()
		{
			Start(typeof(MockResolver), (InputCommands input) =>
			{
				CheckIfTriggerConditionMatches(MouseButton.Left, State.Releasing, mockResolver, input);
				CheckIfTriggerConditionMatches(MouseButton.Middle, State.Released, mockResolver, input);
				CheckIfTriggerConditionMatches(MouseButton.Right, State.Pressing, mockResolver, input);
				CheckIfTriggerConditionMatches(MouseButton.X1, State.Released, mockResolver, input);
				CheckIfTriggerConditionMatches(MouseButton.X2, State.Released, mockResolver, input);
			});
		}

		private static void CheckIfTriggerConditionMatches(MouseButton button, State state,
			MockResolver resolver, InputCommands input)
		{
			var trigger = new MouseButtonTrigger(button, state);
			resolver.input.SetMouseButtonState(trigger.Button, trigger.State);
			Assert.True(trigger.ConditionMatched(input));
		}

		[Test]
		public void CheckForEquility()
		{
			var trigger = new MouseButtonTrigger(MouseButton.Left, State.Pressing);
			var otherTrigger = new MouseButtonTrigger(MouseButton.Left, State.Released);
			Assert.AreNotEqual(trigger, otherTrigger);
			Assert.AreNotEqual(trigger.GetHashCode(), otherTrigger.GetHashCode());

			var copyOfTrigger = new MouseButtonTrigger(MouseButton.Left, State.Pressing);
			Assert.AreEqual(trigger, copyOfTrigger);
			Assert.AreEqual(trigger.GetHashCode(), copyOfTrigger.GetHashCode());
		}

		[Test]
		public void SetGetProperties()
		{
			var trigger = new MouseButtonTrigger(MouseButton.Left, State.Pressing);
			trigger.Button = MouseButton.Right;
			trigger.State = State.Pressed;
			Assert.AreNotEqual(trigger.Button, MouseButton.Left);
			Assert.AreNotEqual(trigger.State, State.Pressing);
			Assert.AreEqual(trigger.Button, MouseButton.Right);
			Assert.AreEqual(trigger.State, State.Pressed);
		}
	}
}