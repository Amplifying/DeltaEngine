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
			positionColorBuffer = new SlimDXCircularBuffer(VertexBufferSize, device);
			positionColorBuffer.Create();
			SetVertexBufferPositionColorDeclaration();
		}

		private SlimDXCircularBuffer positionColorBuffer;
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

			positionColorVertexDeclaration = new VertexDeclaration(device.NativeDevice, vertexElems);
		}

		private VertexDeclaration positionColorVertexDeclaration;

		private void CreatePositionColorUvBuffer()
		{
			positionColorTextureBuffer = new SlimDXCircularBuffer(VertexBufferSize, device);
			positionColorTextureBuffer.Create();
			SetVertexBufferPositionColorTextureDeclaration();
		}

		private SlimDXCircularBuffer positionColorTextureBuffer;
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

			positionColorTextureVertexDeclaration =
				new VertexDeclaration(device.NativeDevice, vertexElems);
		}

		private VertexDeclaration positionColorTextureVertexDeclaration;

		private void CreateIndexBuffer()
		{
			if (indexBuffer != null)
				indexBuffer.Dispose();

			indexBuffer = new IndexBuffer(device.NativeDevice, IndexBufferSize, Usage.Dynamic,
				Pool.Default, true);
		}

		private IndexBuffer indexBuffer;
		private int indicesCount;
		private const int IndexBufferSize = 1024;

		public override void EnableTexturing(Image image)
		{
			device.NativeDevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
			device.NativeDevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
			device.NativeDevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
			device.NativeDevice.SetTexture(0, (image as SlimDXImage).NativeTexture);
		}

		public override void DisableTexturing()
		{
			device.NativeDevice.SetTexture(0, null);
		}

		public override void SetBlending(BlendMode blendMode)
		{
			device.NativeDevice.SetRenderState(RenderState.AlphaBlendEnable, true);
			device.NativeDevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			device.NativeDevice.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
			device.NativeDevice.SetRenderState(RenderState.AlphaRef, 1);
			device.NativeDevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			device.NativeDevice.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			indicesCount = indices.Length;
			indexBuffer.Lock(0, indices.Length * sizeof(short), LockFlags.None).WriteRange(indices, 0,
				indices.Length);
			indexBuffer.Unlock();
		}

		public override void DisableIndices()
		{
			indicesCount = 0;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			positionColorBuffer.SetVertexData(vertices);
			device.NativeDevice.VertexDeclaration = positionColorVertexDeclaration;
			device.NativeDevice.SetStreamSource(0, positionColorBuffer.NativeBuffer,
				positionColorBuffer.Offset, VertexPositionColorSize);
			positionColorShader.Apply();
			DrawPrimitives(mode, vertices.Length);
		}

		private void DrawPrimitives(VerticesMode mode, int verticesCount)
		{
			device.NativeDevice.Indices = indexBuffer;
			var primitiveType = mode == VerticesMode.Triangles
				? PrimitiveType.TriangleList : PrimitiveType.LineList;
			var verticesPerPrimitive = mode == VerticesMode.Triangles
				? VerticesPerTriangle : VerticesPerLine;
			if (indicesCount > 0)
				device.NativeDevice.DrawIndexedPrimitives(primitiveType, 0, 0, verticesCount, 0,
					indicesCount / verticesPerPrimitive);
			else
				device.NativeDevice.DrawPrimitives(primitiveType, 0, verticesCount / verticesPerPrimitive);
		}

		private const int VerticesPerLine = 2;
		private const int VerticesPerTriangle = 3;

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			positionColorTextureBuffer.SetVertexData(vertices);
			device.NativeDevice.VertexDeclaration = positionColorTextureVertexDeclaration;
			device.NativeDevice.SetStreamSource(0, positionColorTextureBuffer.NativeBuffer,
				positionColorTextureBuffer.Offset, VertexPositionColorUvSize);
			positionColorTextureShader.Apply();
			DrawPrimitives(mode, vertices.Length);
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