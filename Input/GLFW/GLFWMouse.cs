using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.ScreenSpaces;
using Pencil.Gaming;

namespace DeltaEngine.Input.GLFW
{
	/// <summary>
	/// Native implementation of the Mouse interface using Xna
	/// </summary>
	public class GLFWMouse : Mouse
	{
		public GLFWMouse(QuadraticScreenSpace screen)
		{
			this.screen = screen;
		}

		private readonly QuadraticScreenSpace screen;

		public override bool IsAvailable
		{
			get { return true; }
		}

		public override void Dispose() {}

		public override void Run()
		{
			MouseState newState = MouseState.GetMouseState();
			UpdateValuesFromState(ref newState);
		}

		public override void SetPosition(Point newPosition)
		{
			newPosition = screen.ToPixelSpace(newPosition);
			Glfw.SetMousePos((int)newPosition.X, (int)newPosition.Y);
		}

		private void UpdateValuesFromState(ref MouseState newState)
		{
			Position = screen.FromPixelSpace(new Point(newState.X, newState.Y));
			ScrollWheelValue = newState.ScrollWheel;
			UpdateButtonStates(ref newState);
		}

		private void UpdateButtonStates(ref MouseState newState)
		{
			LeftButton = LeftButton.UpdateOnNativePressing(newState.LeftButton);
			MiddleButton = MiddleButton.UpdateOnNativePressing(newState.MiddleButton);
			RightButton = RightButton.UpdateOnNativePressing(newState.RightButton);
		}
	}
}