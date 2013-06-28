using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using DxDevice = SharpDX.Direct3D11.Device;

namespace DeltaEngine.Graphics.SharpDX
{
	public class SharpDXCircularBuffer<T> : CircularBuffer<T> where T : struct
	{
		public SharpDXCircularBuffer(int maxNumberOfElements, SharpDXDevice device)
			: base(maxNumberOfElements)
		{
			this.device = device;
			bindFlags = IsIndexBuffer ? BindFlags.IndexBuffer : BindFlags.VertexBuffer;
		}

		private readonly SharpDXDevice device;
		private readonly BindFlags bindFlags;

		public override void Create()
		{
			NativeBuffer = new SharpDXBuffer(device.NativeDevice, bufferSizeInBytes, bindFlags);
			IsCreated = true;
		}

		public SharpDXBuffer NativeBuffer { get; private set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeData(T[] elements, int dataSizeInBytes)
		{
			DataStream dataStream;
			device.Context.MapSubresource(NativeBuffer, MapMode.WriteDiscard, MapFlags.None,
				out dataStream);
			dataStream.Seek(Offset, SeekOrigin.Begin);
			dataStream.WriteRange(elements, 0, elements.Length);
			device.Context.UnmapSubresource(NativeBuffer, 0);
		}
	}
}