using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
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
			startPosition = blockSize * (gridSize / 2);
		}

		private int gridSize;
		private float blockSize;
		private float startPosition;

		[IntegrationTest]
		public void CreateSnakeAtOrigin(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem) =>
			{
				var snake = new Snake(entitySystem, gridSize);
				Assert.AreEqual(new Point(startPosition, startPosition),
					snake.Get<Snake.Body>().BodyParts[0].TopLeft);
			});
		}

		[IntegrationTest]
		public void SnakeHasTwoParts(Type resolver)
		{
			Start(resolver,
				(SnakeGame game, ScreenSpace screen) =>
					Assert.AreEqual(2, game.Snake.Get<Snake.Body>().BodyParts.Count));
		}

		[VisualTest]
		public void PartsOfSnakeBodyAreNotOnSamePlace(Type resolver)
		{
			Start(resolver, (SnakeGame game) => { });
		}

		[IntegrationTest]
		public void AddToSnake(Type resolver)
		{
			Start(resolver,
				(SnakeGame game) => Assert.AreEqual(2, game.Snake.Get<Snake.Body>().BodyParts.Count));
		}

		[Test]
		public void MoveSnake()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed);
				Assert.AreEqual(new Point(startPosition, startPosition - blockSize),
					game.Snake.Get<Snake.Body>().BodyParts[0].TopLeft);
			});
		}

		private const float MoveSpeed = 0.15f;

		[Test]
		public void TouchTopBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed * gridSize / 2);
				Assert.AreEqual(new Point(startPosition, startPosition),
					game.Snake.Get<Snake.Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void TouchLeftBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed * gridSize / 2);
				Assert.AreEqual(new Point(startPosition, startPosition),
					game.Snake.Get<Snake.Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void TouchRightBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.MoveRight();
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed * gridSize / 2);
				Assert.AreEqual(new Point(startPosition, startPosition),
					game.Snake.Get<Snake.Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void TouchBotomBorder()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed);
				game.MoveDown();
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed * gridSize / 2);
				Assert.AreEqual(new Point(startPosition, startPosition),
					game.Snake.Get<Snake.Body>().BodyParts[0].TopLeft);
			});
		}

		[Test]
		public void CheckTrailingVector()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem) =>
			{
				var snake = new Snake(entitySystem, gridSize);
				Assert.AreEqual(new Point(0, blockSize), snake.Get<Snake.Body>().GetTrailingVector());
			});
		}

		[Test]
		public void SnakeCollidingWithItselfWillRestart()
		{
			Start(typeof(MockResolver), (SnakeGame game) =>
			{
				game.Snake.Get<Snake.Body>().AddSnakeBody();
				game.Snake.Get<Snake.Body>().AddSnakeBody();
				game.Snake.Get<Snake.Body>().AddSnakeBody();
				game.MoveLeft();
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed);
				game.MoveDown();
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed);
				game.MoveRight();
				mockResolver.AdvanceTimeAndExecuteRunners(MoveSpeed);
				Assert.AreEqual(2, game.Snake.Get<Snake.Body>().BodyParts.Count);
			});
		}

		[Test]
		public void DisposeSnake()
		{
			Start(typeof(MockResolver), (EntitySystem entitySystem) =>
			{
				var snake = new Snake(entitySystem, gridSize);
				Assert.AreEqual(2, snake.Get<Snake.Body>().BodyParts.Count);
				snake.Dispose(entitySystem);
				Assert.AreEqual(0, snake.NumberOfComponents);
				Assert.AreEqual(0, entitySystem.NumberOfEntities);
				Assert.Throws<Entity.ComponentNotFound>(() => snake.Get<Snake.Body>());
			});
		}
	}
}