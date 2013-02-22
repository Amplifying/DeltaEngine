using System;
using System.Collections.Generic;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Triggers;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
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

		[Test]
		public void TestXnaTouchLogic()
		{
			var window = new TestResolver().Resolve<Window>();
			var screen = new QuadraticScreenSpace(window);
			var touch = new MockTouch(window, screen) { TouchCollection = GetFirstTouchCollection() };
			touch.Run();
			touch.TouchCollection = GetSecondTouchCollection();
			touch.Run();
			touch.TouchCollection = GetThirtTouchCollection();
			touch.Run();
			Assert.True(touch.IsAvailable);
			Assert.AreEqual(touch.GetPosition(0), new Point(0f, 0.125f));
			Assert.AreEqual(touch.GetState(0), State.Released);
		}

		private TouchCollection GetFirstTouchCollection()
		{
			var collection = new List<TouchLocation>
			{
				new TouchLocation(0, TouchLocationState.Pressed,
					new Vector2(0f, 0f)),
				new TouchLocation(3, TouchLocationState.Pressed, new Vector2(0.1f, 0.1f)),
			};

			return new TouchCollection(collection.ToArray());
		}

		private TouchCollection GetSecondTouchCollection()
		{
			var collection = new List<TouchLocation>
			{
				new TouchLocation(0, TouchLocationState.Released,
					new Vector2(0f, 0f)),
				new TouchLocation(-1, TouchLocationState.Released,new Vector2(0f, 0f))
			};

			return new TouchCollection(collection.ToArray());
		}

		private TouchCollection GetThirtTouchCollection()
		{
			var collection = new List<TouchLocation>
			{
				new TouchLocation(100, TouchLocationState.Moved,new Vector2(0f, 0f))
			};

			return new TouchCollection(collection.ToArray());
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
