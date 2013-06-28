using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseButtonTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ConditionMatchedWithoutMouse()
		{
			var trigger = new MouseButtonTrigger(MouseButton.Right, State.Pressing);
			Assert.False(trigger.ConditionMatched(Input));
		}

		[Test]
		public void ConditionMatched()
		{
			CheckIfTriggerConditionMatches(MouseButton.Left, State.Releasing);
			CheckIfTriggerConditionMatches(MouseButton.Middle, State.Released);
			CheckIfTriggerConditionMatches(MouseButton.Right, State.Pressing);
			CheckIfTriggerConditionMatches(MouseButton.X1, State.Released);
			CheckIfTriggerConditionMatches(MouseButton.X2, State.Released);
		}

		private void CheckIfTriggerConditionMatches(MouseButton button, State state)
		{
			var trigger = new MouseButtonTrigger(button, state);
			Resolve<MockMouse>().SetButtonState(trigger.Button, trigger.State);
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