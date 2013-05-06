using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native Windows implementation of the Touch interface.
	/// </summary>
	public class WindowsTouch : Touch
	{
		public WindowsTouch(Window window, ScreenSpace screen)
		{
			var positionTranslator = new CursorPositionTranslater(window, screen);
			touches = new TouchCollection(positionTranslator);
			hook = new TouchHook(window);
			IsAvailable = CheckIfWindows7OrHigher();
		}

		private readonly TouchHook hook;
		private readonly TouchCollection touches;

		public bool IsAvailable { get; protected set; }

		public void Dispose()
		{
			hook.Dispose();
		}

		private static bool CheckIfWindows7OrHigher()
		{
			Version version = Environment.OSVersion.Version;
			return version.Major >= 6 && version.Minor >= 1;
		}

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
			if (IsAvailable == false)
				return;
			
			var newTouches = new List<NativeTouchInput>(hook.nativeTouches.ToArray());
			touches.UpdateAllTouches(newTouches);
			hook.nativeTouches.Clear();
		}
	}
}