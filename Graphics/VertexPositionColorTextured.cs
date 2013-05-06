using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Vertex struct that describes 3D position, vertex color and texture coordinate.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionColorTextured
	{
		public readonly Vector Position;
		public readonly Color Color;
		public readonly Point TextureCoordinate;

		public VertexPositionColorTextured(Vector position, Color color, Point uv)
		{
			Position = position;
			Color = color;
			TextureCoordinate = uv;
		}

		public VertexPositionColorTextured(Point position, Color color, Point uv)
		{
			Position = new Vector(position.X, position.Y, 0.0f);
			Color = color;
			TextureCoordinate = uv;
		}

		public static readonly int SizeInBytes = Vector.SizeInBytes + Color.SizeInBytes + 
			Point.SizeInBytes;
	}
}
