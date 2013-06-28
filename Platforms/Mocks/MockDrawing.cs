using DeltaEngine.Graphics;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockDrawing : Drawing
	{
		public MockDrawing(Device device)
			: base(device) {}

		public override void Dispose() {}
		public override void EnableTexturing(Image image) {}
		public override void DisableTexturing() {}
		public override void SetBlending(BlendMode blendMode) {}
		public override void SetIndices(short[] indices, int usedIndicesCount) {}
		public override void DisableIndices() {}

		public override void DrawVerticesForSprite(VerticesMode mode,
			VertexPositionColorTextured[] vertices)
		{
			NumberOfVerticesDrawn += vertices.Length;
			NumberOfTimesDrawn++;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			NumberOfVerticesDrawn += vertices.Length;
			NumberOfTimesDrawn++;
		}
	}
}