namespace DeltaEngine.Rendering.Transitions
{
	/// <summary>
	/// How far through a transition (changing size, position, color, etc.) a Sprite or Shape is
	/// </summary>
	public class Transition
	{
		public float Elapsed { get; set; }
		public float Duration { get; set; }
		public bool IsEntityToBeRemovedWhenFinished { get; set; }
	}
}