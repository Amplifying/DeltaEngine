using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// Holds physics related data
	/// </summary>
	public class PhysicsObject
	{
		public float Elapsed { get; set; }
		public float Duration { get; set; }
		public float RotationSpeed { get; set; }
		public Point Velocity { get; set; }
		public Point Gravity { get; set; }
	}
}