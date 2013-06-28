using System;
using System.IO;
using DeltaEngine.Datatypes;
using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;

namespace DeltaEngine.Graphics.GLFW
{
	public class GLFWMesh : Mesh
	{
		public GLFWMesh(string contentName)
			: base(contentName)
		{
			vertexFormat = new VertexFormat(new[]
				{
					new VertexElement(VertexElementType.Position3D),
					new VertexElement(VertexElementType.TextureUV),
					new VertexElement(VertexElementType.Color)
				});
		}

		private readonly VertexFormat vertexFormat;

		protected override void LoadData(Stream fileData)
		{
			LoadMeshData(fileData);
		}

		private void LoadMeshData(Stream fileData)
		{
			Vector4[] positions;
			Vector3[] normals;
			Vector2[] texCoords;
			int[] indices;
			GL.Utils.LoadModel(fileData, out positions, out normals, out texCoords, out indices, true);
			if (positions.Length != texCoords.Length)
				throw new NotSupportedException();

			CreateGeometry(positions, indices, texCoords);
		}

		private void CreateGeometry(Vector4[] positions, int[] indices, Vector2[] texCoords)
		{
			var geometryData = new GeometryData(vertexFormat, positions.Length, indices.Length);
			geometryData.AddIndices(indices);
			for (int num = 0; num < positions.Length; ++num)
				geometryData.AddVertexData(new Vector(positions[num].X, positions[num].Y, positions[num].Z),
					new Point(texCoords[num].X, 1.0f - texCoords[num].Y), Color.White);

			geometry = new GLFWGeometry();
			geometry.CreateFrom(geometryData);
		}

		private GLFWGeometry geometry;

		public override void Draw()
		{
			GL.Enable(EnableCap.DepthTest);
			geometry.Draw();
		}

		protected override void DisposeData()
		{
			geometry.Dispose();
		}

		public override int NumberOfVertices
		{
			get { return geometry.GeometryData.NumberOfVertices; }
		}
	}
}