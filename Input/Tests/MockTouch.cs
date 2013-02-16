using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Input.Xna;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using Microsoft.Xna.Framework.Input.Touch;

namespace DeltaEngine.Input.Tests
{
	public class MockTouch : Touch
	{
		public MockTouch(Window window, ScreenSpace screen)
		{
			TouchPanel.WindowHandle = window.Handle;
			IsAvailable = TouchPanel.GetCapabilities().IsConnected;
			touches = new TouchCollectionUpdater(screen);
		}

		private readonly TouchCollectionUpdater touches;

		public Point GetPosition(int touchIndex)
		{
			return touches.locations[touchIndex];
		}

		public State GetState(int touchIndex)
		{
			return touches.states[touchIndex];
		}

		public TouchCollection TouchCollection { get; set; }

		public void Run()
		{
			var locations = new List<TouchLocation>();
			for (int index = 0; index < TouchCollection.Count; index++)
				locations.Add(TouchCollection[index]);

			IsAvailable = TouchCollection.IsConnected;
			touches.UpdateAllTouches(locations);
		}

		public bool IsAvailable { get; private set; }

		public void Dispose() { } //ncrunch: no coverage
	}
}