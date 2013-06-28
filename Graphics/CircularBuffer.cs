using System;
using System.Runtime.InteropServices;

namespace DeltaEngine.Graphics
{
	/// <summary>
	/// Provides a way to render lots of small batches inside a much larger vertex buffer.
	/// </summary>
	public abstract class CircularBuffer<T> : IDisposable where T : struct
	{
		protected CircularBuffer(int maxNumberOfElements)
		{
			MaxNumberOfElements = maxNumberOfElements;
			elementSize = Marshal.SizeOf(typeof(T));
			Initialize();
		}

		public int MaxNumberOfElements { get; private set; }
		protected readonly int elementSize;

		private void Initialize()
		{
			bufferSizeInBytes = MaxNumberOfElements * elementSize;
			lastDataSize = 0;
			Offset = 0;
			if (IsCreated)
			{
				Dispose();
				Create();
			}
		}

		protected int bufferSizeInBytes;
		protected int lastDataSize;

		public int Offset { get; private set; }
		public bool IsCreated { get; protected set; }

		public abstract void Create();
		public abstract void Dispose();

		public bool IsIndexBuffer
		{
			get
			{
				return typeof(T) == typeof(short) || typeof(T) == typeof(ushort) ||
					typeof(T) == typeof(int) || typeof(T) == typeof(uint);
			}
		}

		public void SetData(T[] elements)
		{
			var dataSizeInBytes = elements.Length * elementSize;
			CheckDataSize(elements.Length);
			UpdateOffset(dataSizeInBytes);
			SetNativeData(elements, dataSizeInBytes);
		}

		private void CheckDataSize(int numberOfElements)
		{
			bool sizeChanged = false;
			while (numberOfElements > MaxNumberOfElements)
			{
				MaxNumberOfElements *= 2;
				sizeChanged = true;
			}

			if (sizeChanged)
				Initialize();
		}

		private void UpdateOffset(int newDataSize)
		{
			Offset += lastDataSize;
			if (Offset + newDataSize >= MaxNumberOfElements)
				Offset = 0;

			lastDataSize = newDataSize;
		}

		protected abstract void SetNativeData(T[] elements, int dataSizeInBytes);
	}
}