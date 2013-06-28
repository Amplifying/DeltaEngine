using SlimDX.Direct3D9;
using VertexBufferD3D9 = SlimDX.Direct3D9.VertexBuffer;
using VertexFormatD3D9 = SlimDX.Direct3D9.VertexFormat;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXCircularVertexBuffer<T> : CircularBuffer<T> where T : struct
	{
		public SlimDXCircularVertexBuffer(int maxNumberOfElements, SlimDXDevice device)
			: base(maxNumberOfElements)
		{
			this.device = device;
		}

		private readonly SlimDXDevice device;

		public override void Create()
		{
			NativeBuffer = new VertexBufferD3D9(device.NativeDevice, bufferSizeInBytes, Usage.Dynamic,
				VertexFormatD3D9.None, Pool.Default);
			IsCreated = true;
		}

		public VertexBufferD3D9 NativeBuffer { get; private set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeData(T[] elements, int dataSizeInBytes)
		{
			NativeBuffer.Lock(Offset, dataSizeInBytes, LockFlags.None).WriteRange(elements, 0, 0);
			NativeBuffer.Unlock();
		}
	}
}