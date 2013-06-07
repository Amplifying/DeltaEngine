using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Snake.Tests
{
	public class SnakeTests : TestWithAllFrameworks
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

		[IntegrationTest]
		public void CreateSnakeAtOrigin(Type resolver)
		{
			Start(resolver, () =>
			{
				var snake = new Snake(gridSize);
				Assert.AreEqual(new Point(startPosition, startPosition),
					snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[IntegrationTest]
		public void SnakeHasTwoParts(Type resolver)
		{
			Start(resolver,
				(SnakeGame game, ScreenSpace screen) =>
					Assert.AreEqual(2, game.Snake.Get<Body>().BodyParts.Count));
		}

		[IntegrationTest]
		public void AddToSnake(Type resolver)
		{
			Start(resolver,
				(SnakeGame game) => Assert.AreEqual(2, game.Snake.Get<Body>().BodyParts.Count));
		}

		[Test]
		public void MoveSnake()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
					game.Snake.Get<Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void TouchTopBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
				Assert.AreEqual(0, game.Snake.NumberOfComponents);
			});
		}

		[Test]
		public void TouchLeftBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
				Assert.AreEqual(0, game.Snake.NumberOfComponents);
			});
		}

		[Test]
		public void TouchRightBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.MoveRight();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
				Assert.AreEqual(0, game.Snake.NumberOfComponents);
			});
		}

		[Test]
		public void TouchBotomBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveDown();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed * gridSize / 2);
				Assert.AreEqual(0, game.Snake.NumberOfComponents);
			});
		}

		[Test]
		public void CheckTrailingVector()
		{
			Start(typeof(MockResolver), () =>
			{
				var snake = new Snake(gridSize);
				Assert.AreEqual(new Point(0, blockSize), snake.Get<Body>().GetTrailingVector());
			});
		}

		[Test]
		public void SnakeCollidingWithItselfWillRestart()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.Snake.Get<Body>().AddSnakeBody();
				game.Snake.Get<Body>().AddSnakeBody();
				game.Snake.Get<Body>().AddSnakeBody();
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveDown();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				game.MoveRight();
				mockResolver.AdvanceTimeAndExecuteRunners(moveSpeed);
				Assert.AreEqual(0, game.Snake.NumberOfComponents);
			});
		}

		[Test]
		public void DisposeSnake()
		{
			Start(typeof(MockResolver), () =>
			{
				var snake = new Snake(gridSize) { IsActive = false };
				Assert.AreEqual(2, snake.Get<Body>().BodyParts.Count);
				snake.Dispose();
				EntitySystem.Current.Run();
				Assert.AreEqual(0, snake.NumberOfComponents);
				Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
				Assert.Throws<Entity.ComponentNotFound>(() => snake.Get<Body>());
			});
		}
	}
}