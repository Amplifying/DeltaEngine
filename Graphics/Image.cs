using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Provides a way to load images and display them using graphics device.
	/// </summary>
	public abstract class Image : ContentData
	{
		protected Image(string filename, Drawing drawing)
			: base(filename)
		{
			this.drawing = drawing;
		}

		protected readonly Drawing drawing;
		public abstract Size PixelSize { get; }
		protected bool DisableLinearFiltering { get; set; }

		public virtual void Draw(VertexPositionColorTextured[] vertices)
		{
			drawing.SetIndices(QuadIndices, QuadIndices.Length);
			drawing.DrawVertices(VerticesMode.Triangles, vertices);
		}

		private static readonly short[] QuadIndices = { 0, 1, 2, 0, 2, 3 };

		protected readonly Color[] checkerMapColors =
		{
			Color.LightGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.DarkGray, Color.LightGray, Color.DarkGray, Color.LightGray,
			Color.LightGray, Color.DarkGray, Color.LightGray, Color.DarkGray,
			Color.DarkGray, Color.LightGray, Color.DarkGray, Color.LightGray
		};

		protected static readonly Size DefaultTextureSize = new Size(4, 4);
	}
}