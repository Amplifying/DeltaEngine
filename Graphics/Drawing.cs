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
		public abstract void DisableTexturing();
		public abstract void SetIndices(short[] indices, int usedIndicesCount);
		public abstract void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices);
		public abstract void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices);
	}
}