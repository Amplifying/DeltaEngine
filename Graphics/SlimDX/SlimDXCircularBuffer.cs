using SlimDX.Direct3D9;

namespace DeltaEngine.Graphics.SlimDX
{
	public class SlimDXCircularBuffer : CircularBuffer
	{
		public SlimDXCircularBuffer(int bufferSize, SlimDXDevice device)
			: base(bufferSize)
		{
			this.device = device;
		}

		private readonly SlimDXDevice device;

		public override void Create()
		{
			NativeBuffer = new VertexBuffer(device.NativeDevice, bufferSize, Usage.Dynamic,
				VertexFormat.None, Pool.Default);
			IsCreated = true;
		}

		public VertexBuffer NativeBuffer { get; private set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeVertexData<T>(T[] vertices, int dataSizeInBytes)
		{
			NativeBuffer.Lock(Offset, dataSizeInBytes, LockFlags.None).WriteRange(vertices, 0, 0);
			NativeBuffer.Unlock();
		}
	}
}