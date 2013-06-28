using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Snake.Tests
{
	public class ChunkTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			gridSize = 10;
			blockSize = 1.0f / gridSize;
		}

		private int gridSize;
		private float blockSize;

		[Test]
		public void CreateFirstChunk()
		{
			var chunk = new Chunk(gridSize, blockSize);
			Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
			Assert.AreEqual(blockSize, chunk.Size.Width);
			Assert.AreEqual(blockSize, chunk.Size.Height);
			Assert.AreEqual(Color.Purple, chunk.Color);
			Assert.LessOrEqual(0.0f, chunk.DrawArea.Left);
			Assert.LessOrEqual(0.0f, chunk.DrawArea.Top);
			Assert.GreaterOrEqual(1.0f, chunk.DrawArea.Left);
			Assert.GreaterOrEqual(1.0f, chunk.DrawArea.Top);
		}

		[Test]
		public void DrawChunkAtRandomLocation()
		{
			var screenSpace = Resolve<ScreenSpace>();
			screenSpace.Window.ViewportPixelSize = new Size(800, 600);
			var smallChunk = new Chunk(gridSize, blockSize);
			smallChunk.SpawnAtRandomLocation();
		}

		[Test]
		public void CheckChunkSpawnWithinSnakeBody()
		{
			var screenSpace = Resolve<ScreenSpace>();
			screenSpace.Window.ViewportPixelSize = new Size(200, 200);
			new Snake(gridSize);
			new Chunk(gridSize, blockSize);
		}
	}
}