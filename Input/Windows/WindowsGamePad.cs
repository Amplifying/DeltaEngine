using DeltaEngine.Datatypes;
using DeltaEngine.Input.Devices;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native Windows implementation of the Touch interface.
	/// </summary>
	public class WindowsGamePad : GamePad
	{
		public void Run() {}

		public bool IsAvailable { get; private set; }

		public Point GetPosition(int touchIndex)
		{
			return Point.Zero;
		}

		public State GetState(int touchIndex)
		{
			return State.Released;
		}

		public void Dispose() {}

		public Point GetLeftThumbStick()
		{
			return Point.Zero;
		}

		public Point GetRightThumbStick()
		{
			return Point.Zero;
		}

		public float GetLeftTrigger()
		{
			return 0;
		}

		public float GetRightTrigger()
		{
			return 0;
		}

		public State GetButtonState(GamePadButton button)
		{
			return State.Released;
		}
	}
}