using System;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
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
			ColoredRectangle rect = null;
			Touch currentTouch = null;
			Start(resolver, (Renderer renderer, Touch touch) =>
			{
				currentTouch = touch;
				rect = new ColoredRectangle(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
				renderer.Add(rect);
			},
			delegate
			{
				Point position = currentTouch.GetPosition(0);
				rect.DrawArea.Left = position.X;
				rect.DrawArea.Top = position.Y;
			});
		}
	}
}
