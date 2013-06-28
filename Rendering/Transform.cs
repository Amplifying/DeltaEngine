using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// This class represents the position and the orientation in the 3D space.
	/// </summary>
	public class Transform
	{
		public Transform()
			: this(Vector.Zero, Vector.Zero) {}

		public Transform(Vector position, Vector eulerAngles)
		{
			Position = position;
			Angles = eulerAngles;
		}

		public Vector Position { get; set; }
		public Vector Angles { get; set; }
	}
}