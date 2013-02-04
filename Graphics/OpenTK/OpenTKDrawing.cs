using System;
using System.Runtime.InteropServices;
using DeltaEngine.Platforms;
using OpenTK.Graphics.OpenGL;

namespace DeltaEngine.Graphics.OpenTK
{
	/// <summary>
	/// Facilitates drawing shapes on screen. Needs a graphic device and uses ScreenSpace.
	/// Please note that OpenTKDrawing is not yet optimized and will become much faster soon.
	/// </summary>
	public class OpenTKDrawing : Drawing
	{
		public OpenTKDrawing(Device device, Window window)
			: base(device)
		{
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, window.ViewportPixelSize.Width, window.ViewportPixelSize.Height, 0, -1, 1);
			SetBlending();
		}

		public override void Dispose() {}

		public override void DisableTexturing()
		{
			GL.Disable(EnableCap.Texture2D);
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			CheckCreateBuffer(ref positionColorBufferID, 8192, VertexPositionColor.SizeInBytes, false);
			SetVertexData(vertices, positionColorBufferID);
			SetBaseClientStates();
			SetBaseVertexDeclaration(VertexPositionColor.SizeInBytes);
			if (lastIndicesCount == -1)
				GL.DrawArrays(Convert(mode), 0, vertices.Length);
			else
				GL.DrawElements(Convert(mode), lastIndicesCount, DrawElementsType.UnsignedShort,
					IntPtr.Zero);
			lastIndicesCount = -1;
		}

		private int positionColorBufferID = -1;

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			CheckCreateBuffer(ref positionColorUvBufferID, 8192, VertexPositionColorTextured.SizeInBytes,
				false);
			SetVertexData(vertices, positionColorUvBufferID);
			SetBaseClientStates();
			GL.EnableClientState(ArrayCap.TextureCoordArray);
			SetBaseVertexDeclaration(VertexPositionColorTextured.SizeInBytes);
			GL.TexCoordPointer(2, TexCoordPointerType.Float, VertexPositionColorTextured.SizeInBytes,
				(IntPtr)16);
			if (lastIndicesCount == -1)
				GL.DrawArrays(Convert(mode), 0, vertices.Length);
			else
				GL.DrawElements(Convert(mode), lastIndicesCount, DrawElementsType.UnsignedShort,
					IntPtr.Zero);
			lastIndicesCount = -1;
		}

		private int positionColorUvBufferID = -1;

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			CheckCreateBuffer(ref indexBufferID, 12288, 2, true);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferID);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * 2), indices,
				BufferUsageHint.StreamDraw);
			lastIndicesCount = usedIndicesCount;
		}

		private int indexBufferID = -1;
		private int lastIndicesCount = -1;

		private static void SetBlending()
		{
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		private static void CheckCreateBuffer(ref int bufferID, int vertexOrIndexCount, int stride,
			bool isIndex)
		{
			if (bufferID != -1)
				return;

			GL.GenBuffers(1, out bufferID);
			var bufferTarget = isIndex ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;
			GL.BindBuffer(bufferTarget, bufferID);
			GL.BufferData(bufferTarget, (IntPtr)(vertexOrIndexCount * stride), IntPtr.Zero,
				BufferUsageHint.StaticDraw);
		}

		private static void SetVertexData<T>(T[] vertices, int bufferID) where T : struct
		{
			var sizeInBytes = (IntPtr)(vertices.Length * Marshal.SizeOf(typeof(T)));
			GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeInBytes, IntPtr.Zero, BufferUsageHint.StreamDraw);
			GL.BufferData(BufferTarget.ArrayBuffer, sizeInBytes, vertices, BufferUsageHint.DynamicDraw);
		}

		private static void SetBaseClientStates()
		{
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.VertexArray);
		}

		private static void SetBaseVertexDeclaration(int stride)
		{
			GL.VertexPointer(3, VertexPointerType.Float, stride, IntPtr.Zero);
			GL.ColorPointer(4, ColorPointerType.UnsignedByte, stride, (IntPtr)12);
		}

		private static BeginMode Convert(VerticesMode mode)
		{
			switch (mode)
			{
			case VerticesMode.Lines:
				return BeginMode.Lines;
			default:
				return BeginMode.Triangles;
			}
		}
	}
}