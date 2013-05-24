using System.Runtime.InteropServices;

namespace DeltaEngine.Graphics
{
	public abstract class CircularBuffer
	{
		protected CircularBuffer(int bufferSize)
		{
			this.bufferSize = bufferSize;
			currentDataSize = 0;
			Offset = 0;
		}

		protected int bufferSize;
		protected int currentDataSize;

		public bool IsCreated { get; protected set; }
		public int Offset { get; protected set; }

		public abstract void Create();
		public abstract void Dispose();

		public void SetVertexData<T>(T[] vertices) where T : struct
		{
			var dataSizeInBytes = vertices.Length * Marshal.SizeOf(typeof(T));
			UpdateOffset(dataSizeInBytes);
			SetNativeVertexData(vertices, dataSizeInBytes);
		}

		protected void UpdateOffset(int newDataSize)
		{
			Offset += currentDataSize;
			if ((Offset + newDataSize) >= bufferSize)
				Offset = 0;

			currentDataSize = newDataSize;
		}

		protected abstract void SetNativeVertexData<T>(T[] vertices, int dataSizeInBytes)
			where T: struct;
	}
}