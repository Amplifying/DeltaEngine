using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchTests : TestWithMocksOrVisually
	{
		[Test]
		public void TestPositionAndState()
		{
			var touch = Resolve<Touch>();
			if (!touch.IsAvailable)
				return;

			Assert.NotNull(touch);
			Assert.True(touch.IsAvailable);
			Assert.AreEqual(State.Released, touch.GetState(0));
		}

		[Test]
		public void GraphicalUnitTest()
		{
			Touch currentTouch = Resolve<Touch>();
			Ellipse ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			RunCode = () =>
			{
				Point position = currentTouch.GetPosition(0);
				var drawArea = ellipse.DrawArea;
				drawArea.Left = position.X;
				drawArea.Top = position.Y;
				ellipse.DrawArea = drawArea;
			};
		}

		[Test]
		public void CheckForEquility()
		{
			var trigger = new TouchPressTrigger(State.Pressing);
			var otherTrigger = new TouchPressTrigger(State.Released);
			Assert.AreNotEqual(trigger, otherTrigger);
			Assert.AreNotEqual(trigger.GetHashCode(), otherTrigger.GetHashCode());
			var copyOfTrigger = new TouchPressTrigger(State.Pressing);
			Assert.AreEqual(trigger, copyOfTrigger);
			Assert.AreEqual(trigger.GetHashCode(), copyOfTrigger.GetHashCode());
		}
	}
}