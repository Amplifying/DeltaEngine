using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SlimDX.Direct3D9;
using NativeDevice = SlimDX.Direct3D9.Device;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXDrawing : Drawing
	{
		public SlimDXDrawing(SlimDXDevice device, Window window)
			: base(device)
		{
			nativeDevice = device.NativeDevice;
			CreateShaders();
			SetupWorldViewProjectionMatrix(window.ViewportPixelSize);
			window.ViewportSizeChanged += SetupWorldViewProjectionMatrix;
			CreateBuffers();
			SetDefaultBlendMode();
		}

		private readonly NativeDevice nativeDevice;

		private void CreateShaders()
		{
			positionColorShader = new SlimDXShader((SlimDXDevice)device,
				SlimDXShadersSourceCode.PositionColor);
			positionColorTextureShader = new SlimDXShader((SlimDXDevice)device,
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
			positionColorBuffer = new SlimDXCircularBuffer(VertexBufferSize, (SlimDXDevice)device);
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

			positionColorVertexDeclaration = new VertexDeclaration(nativeDevice, vertexElems);
		}

		private VertexDeclaration positionColorVertexDeclaration;

		private void CreatePositionColorUvBuffer()
		{
			positionColorTextureBuffer = new SlimDXCircularBuffer(VertexBufferSize, (SlimDXDevice)device);
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
				new VertexDeclaration(nativeDevice, vertexElems);
		}

		private VertexDeclaration positionColorTextureVertexDeclaration;

		private void SetDefaultBlendMode()
		{
			SetBlending(BlendMode.Normal);
		}

		private void CreateIndexBuffer()
		{
			if (indexBuffer != null)
				indexBuffer.Dispose();

			indexBuffer = new IndexBuffer(nativeDevice, IndexBufferSize, Usage.Dynamic,
				Pool.Default, true);
		}

		private IndexBuffer indexBuffer;
		private int indicesCount;
		private const int IndexBufferSize = 1024;

		public override void EnableTexturing(Image image)
		{
			nativeDevice.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.None);
			nativeDevice.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Anisotropic);
			nativeDevice.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Anisotropic);
			nativeDevice.SetTexture(0, (image as SlimDXImage).NativeTexture);
		}

		public override void DisableTexturing()
		{
			nativeDevice.SetTexture(0, null);
		}

		public override void SetBlending(BlendMode blendMode)
		{
			if (currentBlendMode == blendMode)
				return;

			SetupBlendRenderState(blendMode);
			currentBlendMode = blendMode;
		}

		private BlendMode currentBlendMode = BlendMode.Opaque;

		private void SetupBlendRenderState(BlendMode blendMode)
		{
			nativeDevice.SetRenderState(RenderState.AlphaRef, 1);
			switch (blendMode)
			{
				case BlendMode.Normal:
					nativeDevice.SetRenderState(RenderState.AlphaBlendEnable, true);
					nativeDevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
					nativeDevice.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
					nativeDevice.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
					break;
				case BlendMode.Opaque:
					nativeDevice.SetRenderState(RenderState.AlphaBlendEnable, false);
					break;
				case BlendMode.AlphaTest:
					nativeDevice.SetRenderState(RenderState.AlphaBlendEnable, false);
					nativeDevice.SetRenderState(RenderState.AlphaTestEnable, true);
					nativeDevice.SetRenderState(RenderState.AlphaFunc, Compare.GreaterEqual);
					break;
				case BlendMode.Additive:
					nativeDevice.SetRenderState(RenderState.AlphaBlendEnable, true);
					nativeDevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
					nativeDevice.SetRenderState(RenderState.DestinationBlend, Blend.One);
					nativeDevice.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
					break;
				case BlendMode.Subtractive:
					nativeDevice.SetRenderState(RenderState.AlphaBlendEnable, true);
					nativeDevice.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
					nativeDevice.SetRenderState(RenderState.DestinationBlend, Blend.One);
					nativeDevice.SetRenderState(RenderState.BlendOperation,
						BlendOperation.ReverseSubtract);
					break;
				case BlendMode.LightEffect:
					nativeDevice.SetRenderState(RenderState.AlphaBlendEnable, true);
					nativeDevice.SetRenderState(RenderState.SourceBlend, Blend.DestinationColor);
					nativeDevice.SetRenderState(RenderState.DestinationBlend, Blend.One);
					nativeDevice.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
					break;
			}			
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
			nativeDevice.VertexDeclaration = positionColorVertexDeclaration;
			nativeDevice.SetStreamSource(0, positionColorBuffer.NativeBuffer,
				positionColorBuffer.Offset, VertexPositionColorSize);
			positionColorShader.Apply();
			DrawPrimitives(mode, vertices.Length);
		}

		private void DrawPrimitives(VerticesMode mode, int verticesCount)
		{
			nativeDevice.Indices = indexBuffer;
			var primitiveType = mode == VerticesMode.Triangles
				? PrimitiveType.TriangleList : PrimitiveType.LineList;
			var verticesPerPrimitive = mode == VerticesMode.Triangles
				? VerticesPerTriangle : VerticesPerLine;
			if (indicesCount > 0)
				nativeDevice.DrawIndexedPrimitives(primitiveType, 0, 0, verticesCount, 0,
					indicesCount / verticesPerPrimitive);
			else
				nativeDevice.DrawPrimitives(primitiveType, 0, verticesCount / verticesPerPrimitive);
		}

		private const int VerticesPerLine = 2;
		private const int VerticesPerTriangle = 3;

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			positionColorTextureBuffer.SetVertexData(vertices);
			nativeDevice.VertexDeclaration = positionColorTextureVertexDeclaration;
			nativeDevice.SetStreamSource(0, positionColorTextureBuffer.NativeBuffer,
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