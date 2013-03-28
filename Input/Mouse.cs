using DeltaEngine.Datatypes;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides the mouse position, mouse button states and allows to set the mouse position.
	/// </summary>
	public abstract class Mouse : InputDevice
	{
		public abstract bool IsAvailable { get; }
		public Point Position { get; protected set; }
		public int ScrollWheelValue { get; protected set; }
		public State LeftButton { get; protected set; }
		public State MiddleButton { get; protected set; }
		public State RightButton { get; protected set; }
		public State X1Button { get; protected set; }
		public State X2Button { get; protected set; }

		public State GetButtonState(MouseButton button)
		{
			if (button == MouseButton.Right)
				return RightButton;
			if (button == MouseButton.Middle)
				return MiddleButton;
			if (button == MouseButton.X1)
				return X1Button;
			if (button == MouseButton.X2)
				return X2Button;

			return LeftButton;
		}

		public abstract void SetPosition(Point newPosition);
		public abstract void Run();
		public abstract void Dispose();
	}
}