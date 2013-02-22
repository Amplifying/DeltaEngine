using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Color = DeltaEngine.Datatypes.Color;

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
			brush = new SolidColorBrush(device.RenderTarget, new Color4(lastColor.PackedRgba));
			device.RenderTarget.Transform = Matrix3x2.Identity;
			drawShader = new SharpDXDrawShader(device);
			Reset(window.ViewportPixelSize);
			window.ViewportSizeChanged += Reset;
			device.Context.OutputMerger.BlendState = device.AlphaBlendState;
		}

		private new readonly SharpDXDevice device;
		private readonly SharpDXDrawShader drawShader;
		private readonly SolidColorBrush brush;

		private void Reset(Size size)
		{
			lastColor = Color.Black;
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
			drawShader.WorldViewProjection = viewportTransform;
		}

		private void SetColor(Color color)
		{
			if (color == lastColor)
				return;

			lastColor = color;
			brush.Color = new Color4(color.PackedRgba);
			device.RenderTarget.AntialiasMode = AntialiasMode.Aliased;
		}

		private Color lastColor;

		public override void Dispose() { }

		public override void DisableTexturing()
		{
			device.Context.PixelShader.SetShaderResource(0, null);
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			SetColor(vertices[0].Color);
			if (mode == VerticesMode.Lines)
				for (int num = 0; num + 1 < vertices.Length; num += 2)
					DrawLines(vertices[num], vertices[num + 1]);
			else
				for (int num = 0; num + 3 < vertices.Length; num += 4)
					DrawRectangles(vertices[num], vertices[num + 2]);
		}

		private void DrawRectangles(VertexPositionColor topLeft, VertexPositionColor bottomRight)
		{
			var sharpRect = new RectangleF(topLeft.Position.X, topLeft.Position.Y,
				bottomRight.Position.X, bottomRight.Position.Y);
			device.RenderTarget.FillRectangle(sharpRect, brush);
		}

		private void DrawLines(VertexPositionColor start, VertexPositionColor end)
		{
			var sharpStart = new DrawingPointF(start.Position.X, start.Position.Y);
			var sharpEnd = new DrawingPointF(end.Position.X, end.Position.Y);
			device.RenderTarget.DrawLine(sharpStart, sharpEnd, brush);
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			CheckCreatePositionColorTextureBuffer();
			device.SetData(positionColorUvVertexBuffer, vertices, vertices.Length);
			drawShader.Apply();
			BindVertexBuffer(positionColorUvVertexBuffer, VertexPositionColorTextured.SizeInBytes);
			if (lastIndicesCount == -1)
				DoDraw(mode, vertices.Length);
			else
				DoDrawIndexed(mode, lastIndicesCount);
			AfterDraw();
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
			if (positionColorUvVertexBuffer != null)
				return;

			positionColorUvVertexBuffer = new SharpDXBuffer(device.NativeDevice,
				vertexCount * VertexPositionColorTextured.SizeInBytes, BindFlags.VertexBuffer);
		}

		private Buffer positionColorUvVertexBuffer;

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