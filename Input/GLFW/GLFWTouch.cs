using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.GLFW
{
	/// <summary>
	/// GLFW does currently not support touch, use another input framework or wait for GLFW3 support.
	/// </summary>
	public class GLFWTouch : Touch
	{
		public void Run() {}
		public void Dispose() {}
		public bool IsAvailable { get { return false; } }

		public Point GetPosition(int touchIndex)
		{
			return new Point();
		}

		public State GetState(int touchIndex)
		{
			return State.Released;
		}
	}
}