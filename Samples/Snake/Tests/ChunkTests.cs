using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Snake.Tests
{
	public class ChunkTests : TestWithAllFrameworks
	{
		[SetUp]
		public void Init()
		{
			gridSize = 10;
			blockSize = 1.0f / gridSize;
		}

		private int gridSize;
		private float blockSize;

		[IntegrationTest]
		public void CreateFirstChunk(Type resolver)
		{
			Start(resolver, () =>
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
			});
		}

		[VisualTest]
		public void DrawChunkAtRandomLocation(Type resolver)
		{
			Start(resolver, (ScreenSpace screenSpace) =>
			{
				screenSpace.Window.TotalPixelSize = new Size(800, 600);
				var smallChunk = new Chunk(gridSize, blockSize);
				smallChunk.SpawnAtRandomLocation();
			});
		}

		[IntegrationTest]
		public void CheckChunkSpawnWithinSnakeBody(Type resolver)
		{
			Start(resolver, (ScreenSpace screenSpace) =>
			{
				screenSpace.Window.TotalPixelSize = new Size(200, 200);
				var snake = new Snake(gridSize);
				var chunk = new Chunk(gridSize, blockSize);
				Assert.IsTrue(chunk.IsCollidingWithSnake(snake.Get<Body>().BodyParts));
			});
		}
	}
}