using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaCircularVertexBuffer<T> : CircularBuffer<T> where T : struct
	{
		public XnaCircularVertexBuffer(int bufferSize, Type xnaVertexType, XnaDevice device)
			: base(bufferSize)
		{
			this.xnaVertexType = xnaVertexType;
			this.device = device;
			VertexSize = Marshal.SizeOf(xnaVertexType);
		}

		private readonly Type xnaVertexType;
		private readonly XnaDevice device;

		public int VertexSize { get; private set; }

		public override void Create()
		{
			NativeBuffer = new DynamicVertexBuffer(device.NativeDevice, xnaVertexType,
				MaxNumberOfElements, BufferUsage.WriteOnly);
			IsCreated = true;
		}

		public DynamicVertexBuffer NativeBuffer { get; protected set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeData(T[] vertices, int dataSizeInBytes)
		{
			NativeBuffer.SetData(Offset, vertices, 0, vertices.Length, Marshal.SizeOf(typeof(T)),
				SetDataOptions.Discard);
		}
	}
}