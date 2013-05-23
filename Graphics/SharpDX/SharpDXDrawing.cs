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
			InitializeBuffers();
		}

		private new readonly SharpDXDevice device;
		private readonly SharpDXPositionColorShader positionColorShader;
		private readonly SharpDXPositionColorTextureShader positionColorTextureShader;

		private void InitializeBuffers()
		{
			positionColorUvVertexBuffer = new CircularBuffer(NumberOfVertexBufferChunks);
		}

		private const int NumberOfVertexBufferChunks = 4;
		private CircularBuffer positionColorUvVertexBuffer;
		private Buffer positionColorVertexBuffer;

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

		public override void DisableTexturing()
		{
			device.Context.PixelShader.SetShaderResource(0, null);
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			CheckCreatePositionColorBuffer();
			device.SetData(positionColorVertexBuffer, vertices, vertices.Length);
			positionColorShader.Apply();
			BindVertexBuffer(positionColorVertexBuffer, VertexPositionColor.SizeInBytes);
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length);
			else
				DoDrawIndexed(mode, vertices.Length, lastIndicesCount);

			AfterDraw();
		}

		private void CheckCreatePositionColorBuffer(int vertexCount = 400)
		{
			if (positionColorVertexBuffer != null)
				return;

			positionColorVertexBuffer = new SharpDXBuffer(device.NativeDevice,
				vertexCount * VertexPositionColor.SizeInBytes, BindFlags.VertexBuffer);
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			CheckCreatePositionColorTextureBuffer();
			device.SetData(positionColorUvVertexBuffer, vertices, vertices.Length);
			positionColorTextureShader.Apply();
			BindVertexBuffer(positionColorUvVertexBuffer.Handle, VertexPositionColorTextured.SizeInBytes);
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length, positionColorUvVertexBuffer.CurrentChunk);
			else
				DoDrawIndexed(mode, vertices.Length, lastIndicesCount,
					positionColorUvVertexBuffer.CurrentChunk);

			AfterDraw();
			positionColorUvVertexBuffer.SelectNextChunk();
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			CheckCreateIndexBuffer();
			device.SetData(indexBuffer, indices, usedIndicesCount);
			lastIndicesCount = usedIndicesCount;
		}

		private int lastIndicesCount = -1;

		private void CheckCreatePositionColorTextureBuffer(int vertexCount = 400)
		{
			if (positionColorUvVertexBuffer.Handle != null)
				return;

			int bufferSize = vertexCount * VertexPositionColorTextured.SizeInBytes *
				positionColorUvVertexBuffer.NumberOfChunks;
			positionColorUvVertexBuffer.Handle = new SharpDXBuffer(device.NativeDevice, bufferSize,
				BindFlags.VertexBuffer);
		}

		private void CheckCreateIndexBuffer(int indexCount = 600)
		{
			if (indexBuffer != null)
				return;

			indexBuffer = new SharpDXBuffer(device.NativeDevice, indexCount * sizeof(ushort),
				BindFlags.IndexBuffer);
		}

		private Buffer indexBuffer;

		private void BindVertexBuffer(Buffer vertexBuffer, int stride)
		{
			device.Context.InputAssembler.SetVertexBuffers(0,
				new VertexBufferBinding(vertexBuffer, stride, 0));
		}

		private void DoDraw(VerticesMode mode, int vertexCount, int currentChunk = 0)
		{
			var primitiveMode = Convert(mode);
			var context = device.Context;
			context.InputAssembler.PrimitiveTopology = primitiveMode;
			context.Draw(vertexCount, vertexCount * currentChunk);
		}

		private void DoDrawIndexed(VerticesMode mode, int vertexCount, int indexCount,
			int currentChunk = 0)
		{
			var primitiveMode = Convert(mode);
			var context = device.Context;
			context.InputAssembler.PrimitiveTopology = primitiveMode;
			device.Context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R16_UInt, 0);
			context.DrawIndexed(indexCount, 0, vertexCount * currentChunk);
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