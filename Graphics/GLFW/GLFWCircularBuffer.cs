using System;
using Pencil.Gaming.Graphics;

namespace DeltaEngine.Graphics.GLFW
{
	public class GLFWCircularBuffer<T> : CircularBuffer<T> where T : struct
	{
		public GLFWCircularBuffer(int maxNumberOfElements) : base(maxNumberOfElements)
		{
			if (IsIndexBuffer)
			{
				bufferTarget = BufferTarget.ElementArrayBuffer;
				bufferUsageHint = BufferUsageHint.StreamDraw;
			} else
			{
				bufferTarget = BufferTarget.ArrayBuffer;
				bufferUsageHint = BufferUsageHint.DynamicDraw;
			}
		}

		private readonly BufferTarget bufferTarget;
		private readonly BufferUsageHint bufferUsageHint;

		protected int NativeBuffer
		{
			get;
			set;
		}

		public override void Create()
		{
			int bufferID;
			GL.GenBuffers(1, out bufferID);
			if (bufferID == 0)
				throw new UnableToCreateBuffer();

			NativeBuffer = bufferID;
			GL.BindBuffer(bufferTarget, NativeBuffer);
			GL.BufferData(bufferTarget, (IntPtr)bufferSizeInBytes, IntPtr.Zero, 
				BufferUsageHint.StaticDraw);
			IsCreated = true;
		}

		public override void Dispose()
		{
			GL.DeleteBuffer(NativeBuffer);
			IsCreated = false;
		}

		protected override void SetNativeData(T[] elements, int dataSizeInBytes)
		{
			GL.BindBufferRange(bufferTarget, 0, NativeBuffer, (IntPtr)Offset, (IntPtr)dataSizeInBytes);
			GL.BufferData(bufferTarget, (IntPtr)dataSizeInBytes, elements, bufferUsageHint);
		}
		private class UnableToCreateBuffer : Exception
		{
		}
	}
}