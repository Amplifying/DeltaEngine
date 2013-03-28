using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native Windows implementation of the GamePad interface. Not implemented yet!
	/// </summary>
	public class WindowsGamePad : GamePad
	{
		public void Run() {}

		public bool IsAvailable
		{
			get { return false; }
		}

		public static Point GetPosition(int touchIndex)
		{
			return Point.Zero;
		}

		public static State GetState(int touchIndex)
		{
			return State.Released;
		}

		public void Dispose() { }

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