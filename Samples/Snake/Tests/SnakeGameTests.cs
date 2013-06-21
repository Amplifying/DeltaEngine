using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Snake.Tests
{
	public class SnakeGameTests : TestWithAllFrameworks
	{
		[SetUp]
		public void Init()
		{
			gridSize = 25;
			blockSize = 1.0f / gridSize;
			startPosition = blockSize * (float)Math.Floor(gridSize / 2.0f);
			moveSpeed = 0.15f;
		}

		private float blockSize;
		private int gridSize;
		private float startPosition;
		private float moveSpeed;

		[VisualTest]
		public void StartGame(Type resolver)
		{
			Start(resolver, (SnakeGame game) => { });
		}

		[Test]
		public void MoveSnakeLeft()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition - blockSize, startPosition),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void MoveSnakeRight()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveRight();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition + blockSize, startPosition),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void MoveSnakeLeftAndThenBotom()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveDown();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition - blockSize, startPosition + blockSize),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void MoveSnakeTop()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveUp();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void SnakeCantMoveDownWhenGoingUp()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveDown();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void SnakeCantMoveUpWhenWhenGoingDown()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveLeft();
				game.MoveDown();
				game.MoveUp();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void SnakeCantMoveLeftWhenWhenGoingRight()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveRight();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition + blockSize * 2, startPosition),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void SnakeCantMoveRightWhenWhenGoingLeft()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveRight();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition - blockSize * 2, startPosition),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void SnakeCantMoveUptWhenWhenGoingDown()
		{
			Start(typeof(MockResolver), (SnakeGame game, ScreenSpace screen) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveDown();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveUp();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition - blockSize, startPosition + blockSize * 2),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[IntegrationTest]
		public void RespawnChunkIfCollidingWithSnake(Type resolver)
		{
			Start(resolver, (SnakeGame snakeGame) =>
			{
				snakeGame.Chunk.DrawArea = snakeGame.Snake.Get<Body>().BodyParts[0].DrawArea;
				Assert.IsTrue(
					snakeGame.Chunk.IsCollidingWithSnake(snakeGame.Snake.Get<Body>().BodyParts));
				snakeGame.RespawnChunk();
				Assert.IsFalse(
					snakeGame.Chunk.IsCollidingWithSnake(snakeGame.Snake.Get<Body>().BodyParts));
			});
		}

		[Test]
		public void SnakeEatsChunk()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				var snakeHead = game.Snake.Get<Body>().BodyParts[0].DrawArea;
				var direction = game.Snake.Get<Body>().Direction;
				var snakeBodyParts = game.Snake.Get<Body>().BodyParts;
				var oldTailTopLeftCorner = snakeBodyParts[snakeBodyParts.Count - 1].DrawArea.TopLeft;

				game.Chunk.DrawArea =
					new Rectangle(new Point(snakeHead.Left + direction.X, snakeHead.Top + direction.Y),
						new Size(blockSize));
				game.MoveUp();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(3, game.Snake.Get<Body>().BodyParts.Count);
				var newTailTopLeftCorner = snakeBodyParts[snakeBodyParts.Count - 1].DrawArea.TopLeft;
				Assert.AreEqual(oldTailTopLeftCorner, newTailTopLeftCorner);
			});
		}

		[VisualTest]
		public void DisplayGameOver(Type resolver)
		{
			Start(resolver, (SnakeGame game) => { game.Reset(); });
		}
	}
}