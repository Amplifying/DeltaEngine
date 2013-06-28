using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaCircularIndexBuffer : CircularBuffer<short>
	{
		public XnaCircularIndexBuffer(int maxNumberOfElements, XnaDevice device)
			: base(maxNumberOfElements)
		{
			this.device = device;
		}

		private readonly XnaDevice device;

		public override void Create()
		{
			NativeBuffer = new DynamicIndexBuffer(device.NativeDevice, IndexElementSize.SixteenBits, 
				MaxNumberOfElements, BufferUsage.WriteOnly);
			IsCreated = true;
		}

		public DynamicIndexBuffer NativeBuffer { get; protected set; }

		public override void Dispose()
		{
			NativeBuffer.Dispose();
			IsCreated = false;
		}

		protected override void SetNativeData(short[] indices, int dataSizeInBytes)
		{
			NativeBuffer.SetData(Offset, indices, 0, indices.Length);
		}
	}
}