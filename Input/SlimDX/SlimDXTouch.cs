using DeltaEngine.Datatypes;

namespace DeltaEngine.Input.SlimDX
{
	/// <summary>
	/// SlimDX does not support any touch input devices.
	/// </summary>
	public class SlimDXTouch : Touch
	{
		public void Run() { }
		public void Dispose() { }
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