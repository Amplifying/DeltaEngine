using System;
using System.Collections.Generic;

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

		public abstract void DrawVerticesForSprite(VerticesMode mode,
			VertexPositionColorTextured[] vertices);

		public abstract void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices);

		public void DrawQuad(Image image, List<VertexPositionColorTextured> vertices,
			List<short> indices)
		{
			var vertexArray = new VertexPositionColorTextured[vertices.Count + 1];
			for (int i = 0; i < vertices.Count; ++i)
				vertexArray[i] = vertices[i];
			var indicesArray = new short[indices.Count + 1];
			for (int i = 0; i < indices.Count; ++i)
				indicesArray[i] = indices[i];
			EnableTexturing(image);
			SetIndices(indicesArray, indicesArray.Length);
			DrawVerticesForSprite(VerticesMode.Triangles, vertexArray);
		}

		public int NumberOfVerticesDrawn { get; set; }
		public int NumberOfTimesDrawn { get; set; }
	}
}