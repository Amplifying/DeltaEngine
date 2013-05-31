using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using NativeDevice = SharpDX.Direct3D11.Device;
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
			context = device.Context;
			nativeDevice = device.NativeDevice;
			positionColorShader = new SharpDXPositionColorShader(device);
			positionColorTextureShader = new SharpDXPositionColorTextureShader(device);
			Reset(window.ViewportPixelSize);
			window.ViewportSizeChanged += Reset;
			device.Context.OutputMerger.BlendState = device.GetAlphaBlendStateLazy();
			InitializeVertexBuffers();
		}

		private readonly DeviceContext context;
		private readonly NativeDevice nativeDevice;
		private readonly SharpDXPositionColorShader positionColorShader;
		private readonly SharpDXPositionColorTextureShader positionColorTextureShader;

		private void InitializeVertexBuffers()
		{
			positionColorVertexBuffer = new SharpDXCircularBuffer(VertexBufferSize,
				(SharpDXDevice)device);
			positionColorUvVertexBuffer = new SharpDXCircularBuffer(VertexBufferSize,
				(SharpDXDevice)device);
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
			context.PixelShader.SetShaderResource(0, (image as SharpDXImage).NativeResourceView);
			var sampler = image.DisableLinearFiltering ? GetPointSamplerLazy() : GetLinearSamplerLazy();
			context.PixelShader.SetSampler(0, sampler);
		}

		public SamplerState GetLinearSamplerLazy()
		{
			return linearSampler ??
				(linearSampler = new SharpDXSampler(nativeDevice, Filter.MinMagMipLinear));
		}

		private SamplerState linearSampler;

		public SamplerState GetPointSamplerLazy()
		{
			return pointSampler ??
				(pointSampler = new SharpDXSampler(nativeDevice, Filter.MinMagMipPoint));
		}

		private SamplerState pointSampler;

		public override void DisableTexturing()
		{
			context.PixelShader.SetShaderResource(0, null);
		}

		public override void SetBlending(BlendMode blendMode)
		{
			if (currentBlendMode == blendMode)
				return;

			var description = new BlendStateDescription();
			description.RenderTarget[0] = GetRenderTargetBlendDescription(blendMode);
			var blendState = new BlendState(nativeDevice, description);
			context.OutputMerger.SetBlendState(blendState);
			currentBlendMode = blendMode;
		}

		private BlendMode currentBlendMode = BlendMode.Normal;

		private static RenderTargetBlendDescription GetRenderTargetBlendDescription(BlendMode blendMode)
		{
			var targetDescription = GetDefaultRenderTargetBlendDescription();
			SetupRenderTargetBlendDescriptionAccordingToTheMode(blendMode, ref targetDescription);
			return targetDescription;
		}

		private static RenderTargetBlendDescription GetDefaultRenderTargetBlendDescription()
		{
			var targetDescription = new RenderTargetBlendDescription();
			targetDescription.IsBlendEnabled = true;
			targetDescription.SourceAlphaBlend = BlendOption.One;
			targetDescription.DestinationAlphaBlend = BlendOption.Zero;
			targetDescription.AlphaBlendOperation = BlendOperation.Add;
			targetDescription.RenderTargetWriteMask = ColorWriteMaskFlags.All;
			targetDescription.SourceBlend = BlendOption.SourceAlpha;
			targetDescription.DestinationBlend = BlendOption.One;
			targetDescription.BlendOperation = BlendOperation.Add;
			return targetDescription;
		}

		private static void SetupRenderTargetBlendDescriptionAccordingToTheMode(BlendMode blendMode,
			ref RenderTargetBlendDescription targetDescription)
		{
			switch (blendMode)
			{
				case BlendMode.Normal:
					targetDescription.DestinationBlend = BlendOption.InverseSourceAlpha;
					break;
				case BlendMode.Additive:
					break;
				case BlendMode.Subtractive:
					targetDescription.BlendOperation = BlendOperation.ReverseSubtract;
					break;
				case BlendMode.LightEffect:
					targetDescription.SourceBlend = BlendOption.DestinationColor;
					targetDescription.BlendOperation = BlendOperation.Add;
					break;
				default:
					targetDescription.IsBlendEnabled = false;
					break;
			}			
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			CheckCreateVertexBuffer(positionColorVertexBuffer);
			positionColorVertexBuffer.SetVertexData(vertices);
			positionColorShader.Apply();
			BindVertexBuffer(positionColorVertexBuffer, VertexPositionColor.SizeInBytes);
			DecideKindOfDraw(mode, vertices.Length);
		}

		private static void CheckCreateVertexBuffer(SharpDXCircularBuffer buffer)
		{
			if (!buffer.IsCreated)
				buffer.Create();
		}

		private void BindVertexBuffer(SharpDXCircularBuffer vertexBuffer, int stride)
		{
			context.InputAssembler.SetVertexBuffers(0,
				new VertexBufferBinding(vertexBuffer.NativeBuffer, stride, vertexBuffer.Offset));
		}

		private void DecideKindOfDraw(VerticesMode mode, int verticesCount)
		{
			context.InputAssembler.PrimitiveTopology = Convert(mode);
			if (lastIndicesCount == -1)
				DoDraw(verticesCount);
			else
				DoDrawIndexed(lastIndicesCount);
		}

		private int lastIndicesCount = -1;

		public override void DisableIndices() {}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			CheckCreateVertexBuffer(positionColorUvVertexBuffer);
			positionColorUvVertexBuffer.SetVertexData(vertices);
			positionColorTextureShader.Apply();
			BindVertexBuffer(positionColorUvVertexBuffer, VertexPositionColorTextured.SizeInBytes);
			DecideKindOfDraw(mode, vertices.Length);
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			CheckCreateIndexBuffer();
			((SharpDXDevice)device).SetData(indexBuffer, indices, usedIndicesCount);
			lastIndicesCount = usedIndicesCount;
		}

		private void CheckCreateIndexBuffer(int indexCount = 600)
		{
			if (indexBuffer != null)
				return;

			indexBuffer = new SharpDXBuffer(nativeDevice, indexCount * sizeof(ushort),
				BindFlags.IndexBuffer);
		}

		private Buffer indexBuffer;

		private void DoDraw(int vertexCount)
		{
			context.Draw(vertexCount, 0);
		}

		private void DoDrawIndexed(int indexCount)
		{
			context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
			context.DrawIndexed(indexCount, 0, 0);
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