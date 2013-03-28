using System.Drawing;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Direct3D11;
using Color = DeltaEngine.Datatypes.Color;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXDrawing : Drawing
	{
		public SlimDXDrawing(SlimDXDevice device)
			: base(device)
		{
			this.device = device;
			context = device.Device.ImmediateContext;
			drawShader = new SlimDXDrawShader(device);
			brush = new SolidColorBrush(device.RenderTarget, new Color4(0.0f, 0.0f, 0.0f));
		}

		private new readonly SlimDXDevice device;
		private readonly DeviceContext context;
		private readonly SlimDXDrawShader drawShader;
		private readonly SolidColorBrush brush;

		private void SetColor(Color color)
		{
			if (color == lastColor)
				return;

			lastColor = color;
			brush.Color = new Color4(color.R, color.G, color.B);
		}

		private Color lastColor;

		private void CheckCreatePositionColorUvBuffer(VertexPositionColorTextured[] vertices, 
			DataStream data)
		{
			if(positionColorTextureBuffer != null)
				return;

			positionColorTextureBuffer = new Buffer(device.Device, data,
				VertexPositionColorUvSize * vertices.Length, ResourceUsage.Default, BindFlags.VertexBuffer,
				CpuAccessFlags.None, ResourceOptionFlags.None, 0);
		}

		private Buffer positionColorTextureBuffer;
		private const int VertexPositionColorUvSize = sizeof(float) * (3 + 4 + 2);

		public override void DisableTexturing() {}

		public override void SetIndices(short[] indices, int usedIndicesCount) {}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			DataStream dataStream = FillDataStreamPositionColorUv(vertices);
			CheckCreatePositionColorUvBuffer(vertices, dataStream);
			context.InputAssembler.SetVertexBuffers(0,
				new VertexBufferBinding(positionColorTextureBuffer, VertexPositionColorUvSize, 0));
			drawShader.Apply();
			context.InputAssembler.PrimitiveTopology = mode == VerticesMode.Triangles ?
				PrimitiveTopology.TriangleList : PrimitiveTopology.LineList;
			context.Draw(vertices.Length, 0);
		}

		private static DataStream FillDataStreamPositionColorUv(VertexPositionColorTextured[] vertices)
		{
			var dataStream = new DataStream(VertexPositionColorUvSize * vertices.Length, true, true);
			for(int i = 0; i < vertices.Length; i++)
			{
				dataStream.Write(new Vector3(vertices[i].Position.X, vertices[i].Position.Y,
					vertices[i].Position.Z));
				dataStream.Write(new Vector4(vertices[i].Color.R / 255.0f, vertices[i].Color.G / 255.0f, 
					vertices[i].Color.B / 255.0f, vertices[i].Color.A / 255.0f));
				dataStream.Write(new Vector2(vertices[i].TextureCoordinate.X, 
					vertices[i].TextureCoordinate.Y));
			}

			dataStream.Position = 0;

			return dataStream;			
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
			device.RenderTarget.FillRectangle(brush, sharpRect);
		}

		private void DrawLines(VertexPositionColor start, VertexPositionColor end)
		{
			var sharpStart = new PointF(start.Position.X, start.Position.Y);
			var sharpEnd = new PointF(end.Position.X, end.Position.Y);
			device.RenderTarget.DrawLine(brush, sharpStart, sharpEnd);
		}

		public override void Dispose()
		{
			brush.Dispose();
			drawShader.Dispose();

			if(positionColorTextureBuffer != null)
				positionColorTextureBuffer.Dispose();
		}
	}
}
