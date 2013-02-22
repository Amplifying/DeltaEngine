using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;
using SysPoint = System.Drawing.Point;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native mouse implementation using a windows hook and invokes.
	/// </summary>
	public class WindowsMouse : BaseMouse
	{
		public WindowsMouse(CursorPositionTranslater positionTranslater)
		{
			hook = new MouseHook();
			this.positionTranslater = positionTranslater;
		}

		internal readonly MouseHook hook;
		private readonly CursorPositionTranslater positionTranslater;

		public override void Dispose()
		{
			hook.Dispose();
		}

		public override bool IsAvailable
		{
			get { return true; }
		}

		public override void SetPosition(Point newPosition)
		{
			positionTranslater.SetCursorPosition(newPosition);
		}

		public override void Run()
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