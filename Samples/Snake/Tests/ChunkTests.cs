using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering;
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
			chunk = new Chunk(gridSize, blockSize);
		}

		private int gridSize;
		private float blockSize;
		private Chunk chunk;

		[IntegrationTest]
		public void CreateFirstChunk(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				entitySystem.Add(chunk);
				Assert.AreEqual(1, entitySystem.NumberOfEntities);
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
		public void DrawChunk(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) => { entitySystem.Add(chunk); });
		}

		[VisualTest]
		public void DrawChunkAtRandomLocation(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ScreenSpace screenSpace) =>
			{
				screenSpace.Window.TotalPixelSize = new Size(200, 200);
				entitySystem.Add(chunk);
				chunk.SpawnAtRandomLocation();
			});
		}

		[IntegrationTest]
		public void CheckChunkSpawnWithinSnakeBody(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ScreenSpace screenSpace) =>
			{
				screenSpace.Window.TotalPixelSize = new Size(200, 200);
				var snake = new Snake(entitySystem, gridSize);
				entitySystem.Add(snake);
				entitySystem.Add(chunk);
				Assert.IsTrue(chunk.IsCollidingWithSnake(snake.Get<Snake.Body>().BodyParts));
			});
		}
	}
}