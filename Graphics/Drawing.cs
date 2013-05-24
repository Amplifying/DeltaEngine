using System;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Allows to draw shapes or images on screen. Needs a graphic device.
	/// </summary>
	public abstract class Drawing : IDisposable
	{
		protected Drawing(Device device)
		{
			this.device = device;
		}

		protected readonly Device device;
		public abstract void Dispose();
		public abstract void EnableTexturing(Image image);
		public abstract void DisableTexturing();
		public abstract void SetBlending(BlendMode blendMode);
		public abstract void SetIndices(short[] indices, int usedIndicesCount);
		public abstract void DisableIndices();
		public abstract void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices);
		public abstract void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices);

		public void DrawQuad(Image image, VertexPositionColorTextured[] vertices)
		{
			EnableTexturing(image);
			SetIndices(QuadIndices, QuadIndices.Length);
			DrawVertices(VerticesMode.Triangles, vertices);
		}

		private static readonly short[] QuadIndices = { 0, 1, 2, 0, 2, 3 };
	}
}