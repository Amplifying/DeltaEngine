using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SlimDX.Direct3D9;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXDrawing : Drawing
	{
		public SlimDXDrawing(SlimDXDevice device, Window window)
			: base(device)
		{
			this.device = device;
			CreateShaders();
			SetupWorldViewProjectionMatrix(window.ViewportPixelSize);
			window.ViewportSizeChanged += SetupWorldViewProjectionMatrix;
			CreateBuffers();
		}

		private new readonly SlimDXDevice device;

		private void CreateShaders()
		{
			positionColorShader = new SlimDXShader(device, SlimDXShadersSourceCode.PositionColor);
			positionColorTextureShader = new SlimDXShader(device,
				SlimDXShadersSourceCode.PositionColorTexture);
		}

		private SlimDXShader positionColorShader;
		private SlimDXShader positionColorTextureShader;

		private void SetupWorldViewProjectionMatrix(Size size)
		{
			var xScale = (size.Width > 0) ? 2.0f / size.Width : 0.0f;
			var yScale = (size.Height > 0) ? 2.0f / size.Height : 0.0f;
			var matrix = new []
			{
				xScale, 0.0f, 0.0f, -1.0f,
				0.0f, -yScale, 0.0f, 1.0f,
				0.0f, 0.0f, 1.0f, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f
			};

			positionColorShader.WorldViewProjectionMatrix = matrix;
			positionColorTextureShader.WorldViewProjectionMatrix = matrix;
		}

		public void CreateBuffers()
		{
			CreatePositionColorBuffer();
			CreatePositionColorUvBuffer();
			CreateIndexBuffer();			
		}

		private void CreatePositionColorBuffer()
		{
			if (positionColorBuffer != null)
				positionColorBuffer.Dispose();

			positionColorBuffer = new VertexBuffer(device.Device,
				VertexBufferSize * VertexPositionColorSize, Usage.Dynamic, VertexFormat.None, Pool.Default);
			SetVertexBufferPositionColorDeclaration();
		}

		private VertexBuffer positionColorBuffer;
		private const int VertexPositionColorSize = 16;
		private const int VertexBufferSize = 1024;

		private void SetVertexBufferPositionColorDeclaration()
		{
			var vertexElems = new []
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default,
					DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default,
					DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
			};

			positionColorVertexDeclaration = new VertexDeclaration(device.Device, vertexElems);
		}

		private VertexDeclaration positionColorVertexDeclaration;

		private void CreatePositionColorUvBuffer()
		{
			if (positionColorTextureBuffer != null)
				positionColorTextureBuffer.Dispose();

			positionColorTextureBuffer = new VertexBuffer(device.Device,
				VertexBufferSize * VertexPositionColorUvSize, Usage.Dynamic, VertexFormat.None,
				Pool.Default);
			SetVertexBufferPositionColorTextureDeclaration();
		}

		private VertexBuffer positionColorTextureBuffer;
		private const int VertexPositionColorUvSize = 24;

		private void SetVertexBufferPositionColorTextureDeclaration()
		{
			var vertexElems = new []
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default,
					DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default,
					DeclarationUsage.Color, 0),
				new VertexElement(0, 16, DeclarationType.Float2, DeclarationMethod.Default,
					DeclarationUsage.TextureCoordinate, 0), 
				VertexElement.VertexDeclarationEnd
			};

			positionColorTextureVertexDeclaration = new VertexDeclaration(device.Device, vertexElems);
		}

		private VertexDeclaration positionColorTextureVertexDeclaration;

		private void CreateIndexBuffer()
		{
			if (indexBuffer != null)
				indexBuffer.Dispose();

			indexBuffer = new IndexBuffer(device.Device, IndexBufferSize, Usage.Dynamic, Pool.Default,
				true);
		}

		private IndexBuffer indexBuffer;
		private int indicesCount;
		private const int IndexBufferSize = 1024;

		public override void DisableTexturing() {}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			indicesCount = indices.Length;
			indexBuffer.Lock(0, indices.Length * sizeof(short), LockFlags.None).WriteRange(indices, 0,
				indices.Length);
			indexBuffer.Unlock();
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			FillDataStreamPositionColor(vertices);
			device.Device.VertexDeclaration = positionColorVertexDeclaration;
			device.Device.SetStreamSource(0, positionColorBuffer, 0, VertexPositionColorSize);
			positionColorShader.Apply();
			DrawPrimitives(mode, vertices.Length);
		}

		private void DrawPrimitives(VerticesMode mode, int verticesCount)
		{
			device.Device.Indices = indexBuffer;
			var primitiveType = mode == VerticesMode.Triangles
				? PrimitiveType.TriangleList : PrimitiveType.LineList;
			var verticesPerPrimitive = mode == VerticesMode.Triangles
				? VerticesPerTriangle : VerticesPerLine;
			if (indicesCount > 0)
				device.Device.DrawIndexedPrimitives(primitiveType, 0, 0, verticesCount, 0,
					indicesCount / verticesPerPrimitive);
			else
				device.Device.DrawPrimitives(primitiveType, 0, verticesCount / verticesPerPrimitive);
		}

		private const int VerticesPerLine = 2;
		private const int VerticesPerTriangle = 3;

		private void FillDataStreamPositionColor(VertexPositionColor[] vertices)
		{
			positionColorBuffer.Lock(0, VertexPositionColorSize * vertices.Length,
				LockFlags.None).WriteRange(vertices, 0, 0);
			positionColorBuffer.Unlock();
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			FillDataStreamPositionColorUv(vertices);
			device.Device.VertexDeclaration = positionColorTextureVertexDeclaration;
			device.Device.SetStreamSource(0, positionColorTextureBuffer, 0, VertexPositionColorUvSize);
			positionColorTextureShader.Apply();
			DrawPrimitives(mode, vertices.Length);
		}

		private void FillDataStreamPositionColorUv(VertexPositionColorTextured[] vertices)
		{
			positionColorTextureBuffer.Lock(0, VertexPositionColorUvSize * vertices.Length,
				LockFlags.None).WriteRange(vertices, 0, 0);
			positionColorTextureBuffer.Unlock();
		}

		public override void Dispose()
		{
			indexBuffer.Dispose();
			positionColorBuffer.Dispose();
			positionColorShader.Dispose();
			positionColorVertexDeclaration.Dispose();
			positionColorTextureBuffer.Dispose();
			positionColorTextureShader.Dispose();
			positionColorTextureVertexDeclaration.Dispose();
		}
	}
}