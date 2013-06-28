using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockMouse : Mouse
	{
		public MockMouse()
		{
			Position = Point.Half;
		}

		public override bool IsAvailable
		{
			get { return true; }
		}

		public override void SetPosition(Point newPosition)
		{
			Position = newPosition;
		}

		public void SetMousePositionNextFrame(Point newMousePosition)
		{
			setNewPosition = newMousePosition;
		}

		private Point setNewPosition = Point.Unused;

		public void SetMouseButtonStateNextFrame(MouseButton button, State state)
		{
			setNewButton = true;
			newButton = button;
			newButtonState = state;
		}

		private bool setNewButton;
		private MouseButton newButton;
		private State newButtonState;

		public override void Run()
		{
			if (setNewPosition != Point.Unused)
				Position = setNewPosition;
			if (setNewButton)
				SetButtonState(newButton, newButtonState);
		}

		public void SetButtonState(MouseButton button, State state)
		{
			setNewButton = false;
			if (button == MouseButton.Right)
				RightButton = state;
			else if (button == MouseButton.Middle)
				MiddleButton = state;
			else if (button == MouseButton.X1)
				X1Button = state;
			else if (button == MouseButton.X2)
				X2Button = state;
			else
				LeftButton = state;
		}

		public override void Dispose() {}
	}
}