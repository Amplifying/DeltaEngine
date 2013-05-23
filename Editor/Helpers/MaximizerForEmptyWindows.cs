using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DeltaEngine.Editor.Helpers
{
	class MaximizerForEmptyWindows
	{
		public MaximizerForEmptyWindows(Window window)
		{
			this.window = window;
		}

		private readonly Window window;

		public void ToggleMaximize()
		{
			if (isMaximized)
				RestoreWindowLocation();
			else
				MaximizeWindow();
		}

		public bool isMaximized;

		private void RestoreWindowLocation()
		{
			isMaximized = false;
			window.ResizeMode = ResizeMode.CanResize;
			window.Top = currentWindowBounds.Top;
			window.Left = currentWindowBounds.Left;
			window.Width = currentWindowBounds.Width;
			window.Height = currentWindowBounds.Height;
		}

		private Rect currentWindowBounds;

		public void MaximizeWindow()
		{
			window.WindowState = WindowState.Normal;
			isMaximized = true;
			window.ResizeMode = ResizeMode.NoResize;
			SaveWindowLocation();
			SetWindowMaximized();
		}

		private void SaveWindowLocation()
		{
			currentWindowBounds = new Rect(window.Left, window.Top, window.Width, window.Height);
		}

		private void SetWindowMaximized()
		{
			var screenWorkAreas = screens.GetDisplayWorkAreas();
			foreach (var screen in screenWorkAreas)
				if (window.Left >= screen.left && window.Left < screen.right && window.Top >= screen.top &&
					window.Top < screen.bottom)
				{
					window.Top = screen.top;
					window.Left = screen.left;
					window.Width = screen.right - screen.left;
					window.Height = screen.bottom - screen.top;
				}
		}

		private readonly NativeScreens screens = new NativeScreens();
	}
}
