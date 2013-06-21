using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class CircularBufferTests
	{
		[Test]
		public void CreateBuffer()
		{
			var buffer = new CircularBufferTest(BufferSize);
			CreateAndCheck(buffer);
		}

		private const int BufferSize = 1024;

		private class CircularBufferTest : CircularBuffer
		{
			public CircularBufferTest(int bufferSize)
				: base(bufferSize) {}

			public override void Create()
			{
				IsCreated = true;
			}

			public override void Dispose()
			{
				IsCreated = false;
			}

			protected override void SetNativeVertexData<T>(T[] vertices, int dataSizeInBytes) {}
		}

		private static void CreateAndCheck(CircularBuffer buffer)
		{
			Assert.IsFalse(buffer.IsCreated);
			buffer.Create();
			Assert.IsTrue(buffer.IsCreated);
		}

		[Test]
		public void CreateAndDisposeBuffer()
		{
			var buffer = new CircularBufferTest(BufferSize);
			CreateAndCheck(buffer);
			buffer.Dispose();
			Assert.IsFalse(buffer.IsCreated);
		}

		[Test]
		public void OffsetInitialization()
		{
			var buffer = new CircularBufferTest(BufferSize);
			Assert.AreEqual(0, buffer.Offset);
		}

		[Test]
		public void OffsetIncrement()
		{
			var buffer = new CircularBufferTest(BufferSize);
			var data = new byte[DataSize];
			buffer.SetVertexData(data);
			Assert.AreEqual(0, buffer.Offset);
			buffer.SetVertexData(data);
			Assert.AreEqual(DataSize, buffer.Offset);
		}

		private const int DataSize = 32;

		[Test]
		public void OffsetSeveralIncrements()
		{
			var buffer = new CircularBufferTest(BufferSize);
			var data = new byte[DataSize];
			for (int i = 0; i < IncrementCount; i++)
			{
				buffer.SetVertexData(data);
				Assert.AreEqual(i * DataSize, buffer.Offset);
			}
		}

		private const int IncrementCount = 4;

		[Test]
		public void DataBiggerThanHalfOfTheBufferSize()
		{
			var buffer = new CircularBufferTest(BufferSize);
			var data = new byte[BigDataSize];
			for (int i = 0; i < IncrementCount; i++)
			{
				buffer.SetVertexData(data);
				Assert.AreEqual(0, buffer.Offset);
			}			
		}

		private const int BigDataSize = 768;

		[Test]
		public void LoadDataWithDifferentSize()
		{
			var buffer = new CircularBufferTest(BufferSize);
			var smallData = new byte[DataSize];
			var bigData = new byte[BigDataSize];
			buffer.SetVertexData(bigData);
			Assert.AreEqual(0, buffer.Offset);
			buffer.SetVertexData(smallData);
			Assert.AreEqual(BigDataSize, buffer.Offset);
			buffer.SetVertexData(smallData);
			Assert.AreEqual(BigDataSize + DataSize, buffer.Offset);
			buffer.SetVertexData(bigData);
			Assert.AreEqual(0, buffer.Offset);
		}
	}
}