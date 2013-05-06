using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Transitions
{
	/// <summary>
	/// Allows a Shape to change the color of its outline over time
	/// </summary>
	public class TransitionOutlineColor
	{
		public Color Start { get; set; }
		public Color End { get; set; }
	}
}