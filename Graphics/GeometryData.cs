using System;
using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Graphics
{
	public class GeometryData : IDisposable
	{
		public GeometryData(VertexFormat vertexFormat, int maxNumberOfVertices, int maxNumberOfIndices)
		{
			Format = vertexFormat;
			this.maxNumberOfVertices = maxNumberOfVertices;
			this.maxNumberOfIndices = maxNumberOfIndices;
			InitializeGeometryData();
		}

		public VertexFormat Format { get; private set; }
		private readonly int maxNumberOfVertices;
		private readonly int maxNumberOfIndices;

		private void InitializeGeometryData()
		{
			Indices = new int[maxNumberOfIndices];
			VertexDataStream = new MemoryStream(maxNumberOfVertices * Format.Stride);
			VertexDataStream.SetLength(VertexDataStream.Capacity);
			vertexDataWriter = new BinaryWriter(VertexDataStream);
		}

		public int[] Indices { get; private set; }
		public MemoryStream VertexDataStream { get; private set; }
		private BinaryWriter vertexDataWriter;

		public void AddVertexData(Vector position, Point textureUV, Color color)
		{
			if (position.Length > maxNumberOfVertices)
				throw new IndexOutOfRangeException();

			VertexDataStream.Seek(NumberOfVertices * Format.Stride, SeekOrigin.Begin);
			foreach (var vertexElement in Format.Elements)
				switch (vertexElement.ElementType)
				{
					case VertexElementType.Position3D:
					case VertexElementType.Normal:
						vertexElement.SaveData(vertexDataWriter, position);
						break;

					case VertexElementType.Position2D:
					case VertexElementType.TextureUV:
						vertexElement.SaveData(vertexDataWriter, textureUV);
						break;

					case VertexElementType.Color:
						vertexElement.SaveData(vertexDataWriter, color);
						break;
				}

			NumberOfVertices++;
		}

		public int NumberOfVertices { get; private set; }

		public void AddIndices(int[] indices)
		{
			if (indices.Length > maxNumberOfIndices)
				throw new IndexOutOfRangeException();

			indices.CopyTo(Indices, 0);
			NumberOfIndices = indices.Length;
		}
		
		public int NumberOfIndices { get; private set; }

		public void Dispose()
		{
			vertexDataWriter.Dispose();
			VertexDataStream.Dispose();
		}
	}
}