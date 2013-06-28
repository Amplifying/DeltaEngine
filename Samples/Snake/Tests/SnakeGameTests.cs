using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Snake.Tests
{
	public class SnakeGameTests : TestWithMocksOrVisually
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

		[Test]
		public void StartGame()
		{
			new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
		}

		[Test]
		public void MoveSnakeLeft()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition - blockSize, startPosition),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void MoveSnakeRight()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveRight();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition + blockSize, startPosition),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void MoveSnakeLeftAndThenBotom()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveDown();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition - blockSize, startPosition + blockSize),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void MoveSnakeTop()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveUp();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void SnakeCantMoveDownWhenGoingUp()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveDown();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void SnakeCantMoveUpWhenWhenGoingDown()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveLeft();
			game.MoveDown();
			game.MoveUp();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void SnakeCantMoveLeftWhenWhenGoingRight()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveRight();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition + blockSize * 2, startPosition),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void SnakeCantMoveRightWhenWhenGoingLeft()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveRight();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition - blockSize * 2, startPosition),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void SnakeCantMoveUptWhenWhenGoingDown()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveDown();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveUp();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition - blockSize, startPosition + blockSize * 2),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void RespawnChunkIfCollidingWithSnake()
		{
			var snakeGame = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			snakeGame.Chunk.DrawArea = snakeGame.Snake.Get<Body>().BodyParts[0].DrawArea;
			Assert.IsTrue(snakeGame.Chunk.IsCollidingWithSnake(snakeGame.Snake.Get<Body>().BodyParts));
			snakeGame.RespawnChunk();
			Assert.IsFalse(snakeGame.Chunk.IsCollidingWithSnake(snakeGame.Snake.Get<Body>().BodyParts));
		}

		[Test]
		public void SnakeEatsChunk()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			var snakeHead = game.Snake.Get<Body>().BodyParts[0].DrawArea;
			var direction = game.Snake.Get<Body>().Direction;
			var snakeBodyParts = game.Snake.Get<Body>().BodyParts;
			var oldTailTopLeftCorner = snakeBodyParts[snakeBodyParts.Count - 1].DrawArea.TopLeft;

			game.Chunk.DrawArea =
				new Rectangle(new Point(snakeHead.Left + direction.X, snakeHead.Top + direction.Y),
					new Size(blockSize));
			game.MoveUp();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(3, game.Snake.Get<Body>().BodyParts.Count);
			var newTailTopLeftCorner = snakeBodyParts[snakeBodyParts.Count - 1].DrawArea.TopLeft;
			Assert.AreEqual(oldTailTopLeftCorner, newTailTopLeftCorner);
		}

		[Test]
		public void DisplayGameOver()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.Reset();
		}
	}
}