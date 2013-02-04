using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using Microsoft.Xna.Framework.Input.Touch;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native Xna implementation of the Touch interface.
	/// </summary>
	public class XnaTouch : Touch
	{
		public XnaTouch(Window window, ScreenSpace screen)
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

		public void Run()
		{
			TouchCollection newTouches = TouchPanel.GetState();
			var locations = new List<TouchLocation>();
			for (int index = 0; index < newTouches.Count; index++)
				locations.Add(newTouches[index]); //ncrunch: no coverage

			IsAvailable = newTouches.IsConnected;
			touches.UpdateAllTouches(locations);
		}

		public bool IsAvailable { get; private set; }

		public void Dispose() {}
	}
}