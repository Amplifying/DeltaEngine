using System;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;
using Color = DeltaEngine.Datatypes.Color;
using Point = DeltaEngine.Datatypes.Point;
using Rectangle = DeltaEngine.Datatypes.Rectangle;

namespace DeltaEngine.Input.Tests
{
	public class TouchTests : TestStarter
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
			Rect rect = null;
			Touch currentTouch = null;
			Start(resolver, (Renderer renderer, Touch touch) =>
			{
				currentTouch = touch;
				rect = new Rect(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
				renderer.Add(rect);
			},
			delegate
			{
				Point position = currentTouch.GetPosition(0);
				rect.DrawArea.Left = position.X;
				rect.DrawArea.Top = position.Y;
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
