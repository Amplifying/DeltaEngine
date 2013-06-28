using System.IO;

namespace DeltaEngine.Graphics.OpenTK
{
	public class OpenTKMesh : Mesh
	{
		public OpenTKMesh(string contentName)
			: base(contentName) {}

		protected override void DisposeData() {}

		protected override void LoadData(Stream fileData)
		{
			LoadMeshData(fileData);
		}

		private void LoadMeshData(Stream fileData)
		{
			var objMeshLoader = new OpenTKMeshLoader(fileData);
			numberOfVertices = objMeshLoader.Positions.Count;
		}

		private int numberOfVertices;

		public override int NumberOfVertices
		{
			get { return numberOfVertices; }
		}

		public override void Draw() {}
	}
}