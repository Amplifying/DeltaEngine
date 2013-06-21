using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using DxDevice = SharpDX.Direct3D11.Device;

namespace DeltaEngine.Graphics.SharpDX
{
	public class SharpDXCircularBuffer : CircularBuffer
	{
		public SharpDXCircularBuffer(int bufferSize, SharpDXDevice device)
			: base(bufferSize)
		{
			this.device = device;
		}

		private readonly SharpDXDevice device;

		public override void Create()
		{
			NativeBuffer = new SharpDXBuffer(device.NativeDevice, bufferSize, BindFlags.VertexBuffer);
			IsCreated = true;
		}

		public SharpDXBuffer NativeBuffer { get; private set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeVertexData<T>(T[] vertices, int dataSizeInBytes)
		{
			DataStream dataStream;
			device.Context.MapSubresource(NativeBuffer, MapMode.WriteDiscard, MapFlags.None,
				out dataStream);
			dataStream.Seek(Offset, SeekOrigin.Begin);
			dataStream.WriteRange(vertices, 0, vertices.Length);
			device.Context.UnmapSubresource(NativeBuffer, 0);
		}
	}
}