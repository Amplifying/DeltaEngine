using DeltaEngine.Datatypes;

namespace Blobs
{
	/// <summary>
	/// Stores a collision with an object
	/// </summary>
	public struct Collision
	{
		public Collision(Point point, Point normal, object @object)
			: this()
		{
			Point = point;
			Normal = normal;
			Object = @object;
		}

		public Point Point;
		public Point Normal;
		public readonly object Object;
	}
}