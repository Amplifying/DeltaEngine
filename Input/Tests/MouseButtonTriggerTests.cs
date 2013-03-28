using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseButtonTriggerTests : TestStarter
	{
		[IntegrationTest]
		public void ConditionMatchedWithoutMouse(Type resolver)
		{
			Start(resolver, (InputCommands input, Time time) =>
			{
				var trigger = new MouseButtonTrigger(MouseButton.Right, State.Pressing);
				Assert.False(trigger.ConditionMatched(input, time));
			});
		}

		[Test]
		public void ConditionMatched()
		{
			var resolver = new TestResolver();
			var input = resolver.Resolve<InputCommands>();
			var time = resolver.Resolve<Time>();

			var trigger = new MouseButtonTrigger(MouseButton.Left, State.Releasing);
			resolver.SetMouseButtonState(MouseButton.Left, State.Releasing);
			Assert.True(trigger.ConditionMatched(input, time));

			trigger = new MouseButtonTrigger(MouseButton.Middle, State.Released);
			resolver.SetMouseButtonState(MouseButton.Middle, State.Released);
			Assert.True(trigger.ConditionMatched(input, time));

			trigger = new MouseButtonTrigger(MouseButton.Right, State.Pressing);
			resolver.SetMouseButtonState(MouseButton.Right, State.Pressing);
			Assert.True(trigger.ConditionMatched(input, time));

			trigger = new MouseButtonTrigger(MouseButton.X1, State.Released);
			resolver.SetMouseButtonState(MouseButton.X1, State.Released);
			Assert.True(trigger.ConditionMatched(input, time));

			trigger = new MouseButtonTrigger(MouseButton.X2, State.Released);
			resolver.SetMouseButtonState(MouseButton.X2, State.Released);
			Assert.True(trigger.ConditionMatched(input, time));
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
	}
}