using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Graphics;

namespace DeltaEngine.Platforms.Mocks
{
	internal class MockMesh : Mesh
	{
		public MockMesh(string contentName, Drawing drawing)
			: base(contentName)
		{
			this.drawing = drawing;
			geometry = new MockGeometry();
		}

		private readonly Drawing drawing;
		private readonly MockGeometry geometry;

		public override int NumberOfVertices
		{
			get { return Name.Compare("Cube") ? 24 : 2203; }
		}

		protected override void LoadData(Stream fileData) {}

		public override void Draw()
		{
			drawing.NumberOfVerticesDrawn += NumberOfVertices;
			drawing.NumberOfTimesDrawn++;
		}

		protected override void DisposeData()
		{
			geometry.Dispose();
		}
	}
}