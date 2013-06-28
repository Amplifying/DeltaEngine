using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockTouch : Touch
	{
		public MockTouch()
		{
			position = Point.Half;
			TouchStates = new State[MaxNumberOfTouchIndices];
		}

		internal static State[] TouchStates;
		private const int MaxNumberOfTouchIndices = 10;
		private static Point position;

		public bool IsAvailable
		{
			get { return true; }
		}

		public Point GetPosition(int touchIndex)
		{
			return position;
		}

		public static void SetTouchState(int touchIndex, State state, Point newTouchPosition)
		{
			position = newTouchPosition;
			TouchStates[touchIndex] = state;
		}

		public State GetState(int touchIndex)
		{
			return TouchStates[touchIndex];
		}

		public void Run() { }
		public void Dispose() { }
	}
}