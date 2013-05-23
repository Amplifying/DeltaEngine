using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace DeltaEngine.Graphics.OpenTK
{
	public class OpenTKDrawing : Drawing
	{
		public OpenTKDrawing(Device device) : base(device)
		{
			InitializeBuffers();
			SetBlending();
		}

		private CircularBuffer positionColorBuffer;
		private const int NumberOfVertexBufferChunks = 4;
		private CircularBuffer positionColorUvBuffer;
		private int indexBufferID = InvalidHandle;
		private const int InvalidHandle = -1;
		private const int InitialIndexBufferSize = 12288;
		private int lastIndicesCount = InvalidHandle;
		private const int InitialVertexBufferSize = 8192;

		private void InitializeBuffers()
		{
			positionColorBuffer = new CircularBuffer(InvalidHandle, NumberOfVertexBufferChunks);
			positionColorUvBuffer = new CircularBuffer(InvalidHandle, NumberOfVertexBufferChunks);
		}

		public override void Dispose()
		{
		}

		public override void SetIndices(short[] indices, int usedIndicesCount)
		{
			if (indexBufferID == InvalidHandle)
				indexBufferID = CreateIndexBuffer(InitialIndexBufferSize * sizeof(short));

			BindBufferAndAddData(indices);
			lastIndicesCount = usedIndicesCount;
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices)
		{
			if (positionColorBuffer.Handle == InvalidHandle)
				positionColorBuffer.Handle = CreateVertexBuffer(positionColorBuffer, 
					InitialVertexBufferSize * VertexPositionColor.SizeInBytes);

			SetVertexData(vertices, positionColorBuffer);
			SetBaseClientStates();
			SetBaseVertexDeclaration(VertexPositionColor.SizeInBytes);
			DecideKindOfDraw(mode, vertices.Length);
			lastIndicesCount = InvalidHandle;
			positionColorBuffer.SelectNextChunk();
		}

		public override void DrawVertices(VerticesMode mode, VertexPositionColorTextured[] vertices)
		{
			if (positionColorUvBuffer.Handle == InvalidHandle)
				positionColorUvBuffer.Handle = CreateVertexBuffer(positionColorUvBuffer, 
					InitialVertexBufferSize * VertexPositionColorTextured.SizeInBytes);

			SetVertexData(vertices, positionColorUvBuffer);
			SetBaseClientStates();
			EnableTextureCoordArray();
			SetBaseVertexDeclaration(VertexPositionColorTextured.SizeInBytes);
			SetTextureCoordDeclaration();
			DecideKindOfDraw(mode, vertices.Length);
			lastIndicesCount = InvalidHandle;
			positionColorUvBuffer.SelectNextChunk();
		}

		private static void SetBlending()
		{
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
		}

		public override void DisableTexturing()
		{
			GL.Disable(EnableCap.Texture2D);
		}

		private int CreateIndexBuffer(int totalSize)
		{
			int bufferID;
			GL.GenBuffers(1, out bufferID);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, bufferID);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(totalSize), IntPtr.Zero, 
				BufferUsageHint.StaticDraw);
			return bufferID;
		}

		private int CreateVertexBuffer(CircularBuffer buffer, int totalSize)
		{
			int bufferID;
			GL.GenBuffers(1, out bufferID);
			GL.BindBuffer(BufferTarget.ArrayBuffer, bufferID);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(totalSize * buffer.NumberOfChunks), 
				IntPtr.Zero, BufferUsageHint.StaticDraw);
			return bufferID;
		}

		private void BindBufferAndAddData(short[] indices)
		{
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferID);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * 2), indices, 
				BufferUsageHint.StreamDraw);
		}

		private void SetVertexData<T>(T[] vertices, CircularBuffer buffer) where T : struct
		{
			int sizeInBytes = vertices.Length * Marshal.SizeOf(typeof(T));
			int offset = sizeInBytes * buffer.CurrentChunk;
			GL.BindBufferRange(BufferTarget.ArrayBuffer, 0, buffer.Handle, (IntPtr)offset, 
				(IntPtr)sizeInBytes);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)sizeInBytes, vertices, 
				BufferUsageHint.DynamicDraw);
		}

		private void SetBaseClientStates()
		{
			GL.EnableClientState(ArrayCap.ColorArray);
			GL.EnableClientState(ArrayCap.VertexArray);
		}

		private void EnableTextureCoordArray()
		{
			GL.EnableClientState(ArrayCap.TextureCoordArray);
		}

		private void SetBaseVertexDeclaration(int stride)
		{
			GL.VertexPointer(3, VertexPointerType.Float, stride, IntPtr.Zero);
			GL.ColorPointer(4, ColorPointerType.UnsignedByte, stride, (IntPtr)12);
		}

		private void SetTextureCoordDeclaration()
		{
			GL.TexCoordPointer(2, TexCoordPointerType.Float, VertexPositionColorTextured.SizeInBytes, 
				(IntPtr)16);
		}

		private void DecideKindOfDraw(VerticesMode mode, int numVertices)
		{
			if (lastIndicesCount == -1)
				GL.DrawArrays(Convert(mode), 0, numVertices);
			else
				GL.DrawElements(Convert(mode), lastIndicesCount, DrawElementsType.UnsignedShort, 
					IntPtr.Zero);
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