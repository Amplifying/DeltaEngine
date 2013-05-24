using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaCircularBuffer : CircularBuffer
	{
		public XnaCircularBuffer(int bufferSize, Type vertexType, XnaDevice device)
			: base(bufferSize)
		{
			this.vertexType = vertexType;
			this.device = device;
			VertexSize = Marshal.SizeOf(vertexType);
		}

		private readonly Type vertexType;
		private readonly XnaDevice device;

		public int VertexSize { get; private set; }

		public override void Create()
		{
			NativeBuffer = new DynamicVertexBuffer(device.NativeDevice,
				vertexType, bufferSize / VertexSize, BufferUsage.WriteOnly);
			IsCreated = true;
		}

		public DynamicVertexBuffer NativeBuffer { get; protected set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeVertexData<T>(T[] vertices, int dataSizeInBytes)
		{
			NativeBuffer.SetData(Offset, vertices, 0, vertices.Length, Marshal.SizeOf(typeof(T)),
				SetDataOptions.Discard);
		}
	}
}