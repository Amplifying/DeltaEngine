using SlimDX.Direct3D9;
using IndexBufferD3D9 = SlimDX.Direct3D9.IndexBuffer;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXCircularIndexBuffer : CircularBuffer<short>
	{
		public SlimDXCircularIndexBuffer(int maxNumberOfElements, SlimDXDevice device)
			: base(maxNumberOfElements)
		{
			this.device = device;
		}

		private readonly SlimDXDevice device;

		public override void Create()
		{
			NativeBuffer = new IndexBufferD3D9(device.NativeDevice, bufferSizeInBytes, Usage.Dynamic,
				Pool.Default, true);
			IsCreated = true;
		}

		public IndexBufferD3D9 NativeBuffer { get; private set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeData(short[] elements, int dataSizeInBytes)
		{
			NativeBuffer.Lock(Offset, dataSizeInBytes, LockFlags.None).WriteRange(elements, 0, 0);
			NativeBuffer.Unlock();
		}
	}
}