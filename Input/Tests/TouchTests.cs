using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void TestPositionAndState(Type resolver)
		{
			Start(resolver, (Touch touch) =>
			{
				Assert.NotNull(touch);
				Assert.True(touch.IsAvailable);
				Assert.AreEqual(Point.Half, touch.GetPosition(0));
				Assert.AreEqual(State.Released, touch.GetState(0));
			});
		}

		[VisualTest]
		public void GraphicalUnitTest(Type resolver)
		{
			Ellipse ellipse = null;
			Touch currentTouch = null;
			Start(resolver, (EntitySystem entitySystem, Touch touch) =>
			{
				currentTouch = touch;
				ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
				entitySystem.Add(ellipse);
			}, delegate
			{
				Point position = currentTouch.GetPosition(0);
				var drawArea = ellipse.DrawArea;
				drawArea.Left = position.X;
				drawArea.Top = position.Y;
				ellipse.DrawArea = drawArea;
			});
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