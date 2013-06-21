using System;
using OpenTK.Graphics.OpenGL;

namespace DeltaEngine.Graphics.OpenTK
{
	public class OpenTKCircularBuffer : CircularBuffer
	{
		public OpenTKCircularBuffer(int bufferSize) : base(bufferSize)
		{
		}

		public int NativeBuffer
		{
			get;
			protected set;
		}

		public override void Create()
		{
			int bufferID;
			GL.GenBuffers(1, out bufferID);
			NativeBuffer = bufferID;
			GL.BindBuffer(BufferTarget.ArrayBuffer, NativeBuffer);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)bufferSize, IntPtr.Zero, 
				BufferUsageHint.StaticDraw);
			IsCreated = true;
		}

		public override void Dispose()
		{
			GL.DeleteBuffer(NativeBuffer);
			IsCreated = false;
		}

		protected override void SetNativeVertexData<T>(T[] vertices, int dataSizeInBytes)
		{
			GL.BindBufferRange(BufferTarget.ArrayBuffer, 0, NativeBuffer, (IntPtr)Offset, 
				(IntPtr)dataSizeInBytes);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)dataSizeInBytes, vertices, 
				BufferUsageHint.DynamicDraw);
		}
	}
}