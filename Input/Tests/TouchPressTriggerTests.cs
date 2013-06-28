using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchPressTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ConditionMatched()
		{
			var trigger = new TouchPressTrigger(State.Pressing);
			Assert.False(trigger.ConditionMatched(Input));
		}

		[Test]
		public void SetGetProperties()
		{
			var trigger = new TouchPressTrigger(State.Pressing) { State = State.Pressed };
			Assert.AreNotEqual(trigger.State, State.Pressing);
			Assert.AreEqual(trigger.State, State.Pressed);
		}
	}
}
