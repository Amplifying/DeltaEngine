using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Simplest vertex format with just 3D positions and vertex colors (12 + 4 bytes).
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionColor
	{
		public VertexPositionColor(Vector position, Color color)
		{
			Position = position;
			Color = color;
		}

		public readonly Vector Position;
		public readonly Color Color;

		public VertexPositionColor(Point position, Color color)
			: this(new Vector(position.X, position.Y, 0.0f), color) {}

		public static readonly int SizeInBytes = Vector.SizeInBytes + Color.SizeInBytes;
	}
}