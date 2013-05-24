using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Matrix = SharpDX.Matrix;

namespace DeltaEngine.Graphics.SharpDX
{
	/// <summary>
	/// Simple drawing support for DirectX 11, currently just allows to draw colored 2D rectangles.
	/// Please note that SharpDXDrawing is not yet optimized and will become much faster soon.
	/// </summary>
	public class SharpDXDrawing : Drawing
	{
		public SharpDXDrawing(SharpDXDevice device, Window window)
			: base(device)
		{
			this.device = device;
			positionColorShader = new SharpDXPositionColorShader(device);
			positionColorTextureShader = new SharpDXPositionColorTextureShader(device);
			Reset(window.ViewportPixelSize);
			window.ViewportSizeChanged += Reset;
			device.Context.OutputMerger.BlendState = device.GetAlphaBlendStateLazy();
			InitializeVertexBuffers();
		}

		private new readonly SharpDXDevice device;
		private readonly SharpDXPositionColorShader positionColorShader;
		private readonly SharpDXPositionColorTextureShader positionColorTextureShader;

		private void InitializeVertexBuffers()
		{
			positionColorVertexBuffer = new SharpDXCircularBuffer(VertexBufferSize, device);
			positionColorUvVertexBuffer = new SharpDXCircularBuffer(VertexBufferSize, device);
		}

		private SharpDXCircularBuffer positionColorVertexBuffer;
		private SharpDXCircularBuffer positionColorUvVertexBuffer;
		private const int VertexBufferSize = 1024;

		private void Reset(Size size)
		{
			var xScale = (size.Width > 0) ? 2.0f / size.Width : 0.0f;
			var yScale = (size.Height > 0) ? 2.0f / size.Height : 0.0f;
			var viewportTransform = new Matrix
			{
				M11 = xScale,
				M22 = -yScale,
				M33 = 1.0f,
				M44 = 1.0f,
				M41 = -1.0f,
				M42 = 1.0f
			};
			positionColorTextureShader.WorldViewProjection = viewportTransform;
			positionColorShader.WorldViewProjection = viewportTransform;
		}

		public override void Dispose() {}

		public override void EnableTexturing(Image image)
		{
			device.Context.PixelShader.SetShaderResource(0, (image as SharpDXImage).NativeResourceView);
			var sampler = image.DisableLinearFiltering ? GetPointSamplerLazy() : GetLinearSamplerLazy();
			device.Context.PixelShader.SetSampler(0, sampler);
		}

		public SamplerState GetLinearSamplerLazy()
		{
			return linearSampler ??
				(linearSampler = new SharpDXSampler(device.NativeDevice, Filter.MinMagMipLinear));
		}

		private SamplerState linearSampler;

		public SamplerState GetPointSamplerLazy()
		{
			return pointSampler ??
				(pointSampler = new SharpDXSampler(device.NativeDevice, Filter.MinMagMipPoint));
		}

		private SamplerState pointSampler;

		public override void DisableTexturing()
		{
			device.Context.PixelShader.SetShaderResource(0, null);
		}

		public override void SetBlending(BlendMode blendMode)
		{
			var blendState = new BlendState(device.NativeDevice, GetBlendingDescription());
			device.Context.OutputMerger.SetBlendState(blendState);
		}

		private static BlendStateDescription GetBlendingDescription()
		{
			var targetDescription = new RenderTargetBlendDescription();
			targetDescription.SourceAlphaBlend = BlendOption.One;
			targetDescription.DestinationAlphaBlend = BlendOption.Zero;
			targetDescription.SourceBlend = BlendOption.One;
			targetDescription.DestinationBlend = BlendOption.Zero;
			var description = new BlendStateDescription();
			description.AlphaToCoverageEnable = false;
			description.IndependentBlendEnable = false;
			description.RenderTarget[0] = targetDescription;
			return description;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			CheckCreatePositionColorBuffer();
			positionColorVertexBuffer.SetVertexData(vertices);
			positionColorShader.Apply();
			BindVertexBuffer(positionColorVertexBuffer, VertexPositionColor.SizeInBytes);
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length);
			else
				DoDrawIndexed(mode, lastIndicesCount);

			AfterDraw();
		}

		private void CheckCreatePositionColorBuffer()
		{
			if (!positionColorVertexBuffer.IsCreated)
				positionColorVertexBuffer.Create();
		}

		public override void DisableIndices() {}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			CheckCreatePositionColorTextureBuffer();
			positionColorUvVertexBuffer.SetVertexData(vertices);
			positionColorTextureShader.Apply();
			BindVertexBuffer(positionColorUvVertexBuffer, VertexPositionColorTextured.SizeInBytes);
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length);
			else
				DoDrawIndexed(mode, lastIndicesCount);

			AfterDraw();
		}

		private void CheckCreatePositionColorTextureBuffer()
		{
			if (!positionColorUvVertexBuffer.IsCreated)
				positionColorUvVertexBuffer.Create();
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			CheckCreateIndexBuffer();
			device.SetData(indexBuffer, indices, usedIndicesCount);
			lastIndicesCount = usedIndicesCount;
		}

		private int lastIndicesCount = -1;

		private void CheckCreateIndexBuffer(int indexCount = 600)
		{
			if (indexBuffer != null)
				return;

			indexBuffer = new SharpDXBuffer(device.NativeDevice, indexCount * sizeof(ushort),
				BindFlags.IndexBuffer);
		}

		private Buffer indexBuffer;

		private void BindVertexBuffer(SharpDXCircularBuffer vertexBuffer, int stride)
		{
			device.Context.InputAssembler.SetVertexBuffers(0,
				new VertexBufferBinding(vertexBuffer.NativeBuffer, stride, vertexBuffer.Offset));
		}

		private void DoDraw(VerticesMode mode, int vertexCount)
		{
			var primitiveMode = Convert(mode);
			var context = device.Context;
			context.InputAssembler.PrimitiveTopology = primitiveMode;
			context.Draw(vertexCount, 0);
		}

		private void DoDrawIndexed(VerticesMode mode, int indexCount)
		{
			var primitiveMode = Convert(mode);
			var context = device.Context;
			context.InputAssembler.PrimitiveTopology = primitiveMode;
			device.Context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
			context.DrawIndexed(indexCount, 0, 0);
		}

		private void AfterDraw()
		{
			lastIndicesCount = -1;
		}

		private static PrimitiveTopology Convert(VerticesMode mode)
		{
			switch (mode)
			{
				case VerticesMode.Lines:
					return PrimitiveTopology.LineList;
				default:
					return PrimitiveTopology.TriangleList;
			}
		}
	}
}