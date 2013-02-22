using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using MouseState = Microsoft.Xna.Framework.Input.MouseState;
using NativeMouse = Microsoft.Xna.Framework.Input.Mouse;

namespace DeltaEngine.Input.Xna
{
	/// <summary>
	/// Native implementation of the Mouse interface using Xna
	/// </summary>
	public class XnaMouse : BaseMouse
	{
		public XnaMouse(Window window, QuadraticScreenSpace screen)
		{
			this.screen = screen;
			if (window != null)
				NativeMouse.WindowHandle = window.Handle;
		}

		private readonly QuadraticScreenSpace screen;

		public override bool IsAvailable
		{
			get { return true; }
		}

		public override void Run()
		{
			MouseState newState = NativeMouse.GetState();
			UpdateValuesFromState(ref newState);
		}

		public override void Dispose()
		{
			NativeMouse.WindowHandle = IntPtr.Zero;
		}

		public override void SetPosition(Point newPosition)
		{
			newPosition = screen.ToPixelSpace(newPosition);
			NativeMouse.SetPosition((int)newPosition.X, (int)newPosition.Y);
		}

		private void UpdateValuesFromState(ref MouseState newState)
		{
			Position = new Point(newState.X, newState.Y);
			Position = screen.FromPixelSpace(Position);
			ScrollWheelValue = newState.ScrollWheelValue;
			UpdateButtonStates(ref newState);
		}

		private void UpdateButtonStates(ref MouseState newState)
		{
			LeftButton = LeftButton.UpdateOnNativePressing(newState.LeftButton == ButtonState.Pressed);
			MiddleButton =
				MiddleButton.UpdateOnNativePressing(newState.MiddleButton == ButtonState.Pressed);
			RightButton = RightButton.UpdateOnNativePressing(newState.RightButton == ButtonState.Pressed);
			X1Button = X1Button.UpdateOnNativePressing(newState.XButton1 == ButtonState.Pressed);
			X2Button = X2Button.UpdateOnNativePressing(newState.XButton2 == ButtonState.Pressed);
		}
	}
}