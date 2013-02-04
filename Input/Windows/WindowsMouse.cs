using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using SysPoint = System.Drawing.Point;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native mouse implementation using a windows hook and invokes.
	/// </summary>
	public class WindowsMouse : Mouse
	{
		public WindowsMouse(CursorPositionTranslater positionTranslater)
		{
			hook = new MouseHook();
			this.positionTranslater = positionTranslater;
		}

		internal readonly MouseHook hook;
		private readonly CursorPositionTranslater positionTranslater;

		public void Dispose()
		{
			hook.Dispose();
		}

		public bool IsAvailable { get { return true; } }
		public Point Position { get; private set; }
		public int ScrollWheelValue { get; private set; }
		public State LeftButton { get; private set; }
		public State MiddleButton { get; private set; }
		public State RightButton { get; private set; }
		public State X1Button { get; private set; }
		public State X2Button { get; private set; }

		public State GetButtonState(MouseButton button)
		{
			if (button == MouseButton.Left)
				return LeftButton;
			if (button == MouseButton.Right)
				return RightButton;
			if (button == MouseButton.Middle)
				return MiddleButton;
			if (button == MouseButton.X1)
				return X1Button;
			return LeftButton;
		}

		public void SetPosition(Point newPosition)
		{
			positionTranslater.SetCursorPosition(newPosition);
		}

		public void Run()
		{
			UpdateMousePosition();
			UpdateMouseValues();
		}

		private void UpdateMousePosition()
		{
			Position = positionTranslater.GetCursorPosition();
		}

		private void UpdateMouseValues()
		{
			ScrollWheelValue = hook.ScrollWheelValue;
			LeftButton = hook.ProcessButtonQueue(LeftButton, MouseButton.Left);
			MiddleButton = hook.ProcessButtonQueue(MiddleButton, MouseButton.Middle);
			RightButton = hook.ProcessButtonQueue(RightButton, MouseButton.Right);
			X1Button = hook.ProcessButtonQueue(X1Button, MouseButton.X1);
			X2Button = hook.ProcessButtonQueue(X2Button, MouseButton.X2);
		}
	}
}