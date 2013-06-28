using System;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadButtonTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ConditionMatched()
		{
			var trigger = new GamePadButtonTrigger(GamePadButton.Y, State.Pressing);
			Assert.False(trigger.ConditionMatched(Input));
		}
	}
}