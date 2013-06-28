using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseMovementTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ConditionMatchedWithoutMouse()
		{
			var trigger = new MouseMovementTrigger();
			Assert.False(trigger.ConditionMatched(Input));
		}

		[Test]
		public void ConditionMatched()
		{
			var trigger = new MouseMovementTrigger();
			Assert.False(trigger.ConditionMatched(Input));
			Resolve<Mouse>().SetPosition(Point.Zero);
			Assert.True(trigger.ConditionMatched(Input));
		}
	}
}