using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using SysPoint = System.Drawing.Point;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Used to translate courser and touch from and to client coordinates. Also used for SharpDX.
	/// </summary>
	public class CursorPositionTranslater
	{
		public CursorPositionTranslater(Window window, ScreenSpace screen)
		{
			this.window = window;
			this.screen = screen;
		}

		private readonly Window window;
		private readonly ScreenSpace screen;

		public void SetCursorPosition(Point newPosition)
		{
			var newScreenPosition = ToSysPoint(ToScreenPositionFromScreenSpace(newPosition));
			NativeMethods.SetCursorPos(newScreenPosition.X, newScreenPosition.Y);
		}

		private static SysPoint ToSysPoint(Point position)
		{
			return new SysPoint((int)Math.Round(position.X), (int)Math.Round(position.Y));
		}

		internal Point ToScreenPositionFromScreenSpace(Point newPosition)
		{
			newPosition = screen.ToPixelSpace(newPosition);
			var newScreenPosition = ToSysPoint(newPosition);
			if (window.Handle != IntPtr.Zero)
				NativeMethods.ClientToScreen(window.Handle, ref newScreenPosition);

			return FromSysPoint(newScreenPosition);
		}

		private static Point FromSysPoint(SysPoint newPosition)
		{
			return new Point(newPosition.X, newPosition.Y);
		}

		public Point GetCursorPosition()
		{
			var newPosition = new SysPoint();
			NativeMethods.GetCursorPos(ref newPosition);
			var screenspace = FromScreenPositionToScreenSpace(FromSysPoint(newPosition));
			return new Point((float)Math.Round(screenspace.X, 3), (float)Math.Round(screenspace.Y, 3));
		}

		internal Point FromScreenPositionToScreenSpace(Point newPosition)
		{
			var screenPoint = ToSysPoint(newPosition);
			if (window.Handle != IntPtr.Zero)
				NativeMethods.ScreenToClient(window.Handle, ref screenPoint);

			return screen.FromPixelSpace(FromSysPoint(screenPoint));
		}
	}
}