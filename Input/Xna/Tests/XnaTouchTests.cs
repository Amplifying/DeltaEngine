using System.Collections.Generic;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using NUnit.Framework;
using Point = DeltaEngine.Datatypes.Point;

namespace DeltaEngine.Input.Xna.Tests
{
	public class XnaTouchTests
	{
		[Test]
		public void TestXnaTouchLogic()
		{
			var window = new MockResolver().rendering.Window;
			var screen = new PixelScreenSpace(window);
			var touch = new XnaMockTouch(window, screen) { TouchCollection = GetFirstTouchCollection() };
			touch.Run();
			touch.TouchCollection = GetSecondTouchCollection();
			touch.Run();
			touch.TouchCollection = GetThirtTouchCollection();
			touch.Run();
			Assert.True(touch.IsAvailable);
			Assert.AreEqual(Point.Zero, touch.GetPosition(0));
			Assert.AreEqual(State.Released, touch.GetState(0));
		}

		private static TouchCollection GetFirstTouchCollection()
		{
			var collection = new List<TouchLocation>
			{
				new TouchLocation(0, TouchLocationState.Pressed, new Vector2(0f, 0f)),
				new TouchLocation(3, TouchLocationState.Pressed, new Vector2(0.1f, 0.1f)),
			};

			return new TouchCollection(collection.ToArray());
		}

		private static TouchCollection GetSecondTouchCollection()
		{
			var collection = new List<TouchLocation>
			{
				new TouchLocation(0, TouchLocationState.Released, new Vector2(0f, 0f)),
				new TouchLocation(-1, TouchLocationState.Released, new Vector2(0f, 0f))
			};

			return new TouchCollection(collection.ToArray());
		}

		private static TouchCollection GetThirtTouchCollection()
		{
			var collection = new List<TouchLocation>
			{
				new TouchLocation(100, TouchLocationState.Moved, new Vector2(0f, 0f))
			};

			return new TouchCollection(collection.ToArray());
		}
	}
}
