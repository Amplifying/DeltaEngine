using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Snake.Tests
{
	public class SnakeTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			gridSize = 25;
			blockSize = 1.0f / gridSize;
			startPosition = blockSize * (int)(gridSize / 2.0f);
			moveSpeed = 0.15f;
		}

		private int gridSize;
		private float blockSize;
		private float startPosition;
		private float moveSpeed;

		[Test]
		public void CreateSnakeAtOrigin()
		{
			var snake = new Snake(gridSize);
			Assert.AreEqual(new Point(startPosition, startPosition),
				snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void SnakeHasTwoParts()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			Assert.AreEqual(2, game.Snake.Get<Body>().BodyParts.Count);
		}

		[Test]
		public void AddToSnake()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			Assert.AreEqual(2, game.Snake.Get<Body>().BodyParts.Count);
		}

		[Test]
		public void MoveSnake()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
				game.Snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test]
		public void TouchTopBorder()
		{
			new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
		}

		[Test]
		public void TouchLeftBorder()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
		}

		[Test]
		public void TouchRightBorder()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveRight();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
		}

		[Test]
		public void TouchBottomBorder()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveDown();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
		}

		[Test]
		public void CheckTrailingVector()
		{
			var snake = new Snake(gridSize);
			Assert.AreEqual(new Point(0, blockSize), snake.Get<Body>().GetTrailingVector());
		}

		[Test]
		public void SnakeCollidingWithItselfWillRestart()
		{
			var game = new SnakeGame(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			game.Snake.Get<Body>().AddSnakeBody();
			game.Snake.Get<Body>().AddSnakeBody();
			game.Snake.Get<Body>().AddSnakeBody();
			game.MoveLeft();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveDown();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
			game.MoveRight();
			resolver.AdvanceTimeAndExecuteRunners(moveSpeed);
		}

		[Test]
		public void DisposeSnake()
		{
			var snake = new Snake(gridSize) { IsActive = false };
			Assert.AreEqual(2, snake.Get<Body>().BodyParts.Count);
			snake.Dispose();
			EntitySystem.Current.Run();
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			Assert.Throws<Entity.ComponentNotFound>(() => snake.Get<Body>());
		}
	}
}