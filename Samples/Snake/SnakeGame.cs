using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace Snake
{
	public class SnakeGame
	{
		public SnakeGame(ScreenSpace screen, EntitySystem entitySystem, InputCommands input)
		{
			gridSize = 25;
			blockSize = 1.0f / gridSize;
			this.screen = screen;
			this.entitySystem = entitySystem;
			SetupPlayArea();
			SetInput(input);
			InitializeSnake();
			SpawnFirstChunk();
		}

		private readonly int gridSize;
		private readonly float blockSize;
		private readonly ScreenSpace screen;
		private readonly EntitySystem entitySystem;

		private void SetupPlayArea()
		{
			screen.Window.Title = "Snake - Let's go";
			screen.Window.TotalPixelSize = new Size(500, 500);
			screen.Window.BackgroundColor = Color.DarkRed;
			var background = new Rect(CalculateBackgroundDrawArea(), Color.Black);
			entitySystem.Add(background);
		}

		private Rectangle CalculateBackgroundDrawArea()
		{
			return new Rectangle(blockSize, blockSize, blockSize * (gridSize - 2),
				blockSize * (gridSize - 2));
		}

		private void SetInput(InputCommands input)
		{
			input.Add(Key.CursorLeft, State.Pressing, MoveLeft);
			input.Add(Key.CursorRight, State.Pressing, MoveRight);
			input.Add(Key.CursorUp, State.Pressing, MoveUp);
			input.Add(Key.CursorDown, State.Pressing, MoveDown);
		}

		public void MoveLeft()
		{
			if (GetDirection().X > 0)
				return;
			Snake.Get<Snake.Body>().Direction = new Point(-blockSize, 0);
		}

		private Point GetDirection()
		{
			var parts = Snake.Get<Snake.Body>().BodyParts;
			return new Point(parts[0].DrawArea.Left - parts[1].DrawArea.Left,
				parts[0].DrawArea.Top - parts[1].DrawArea.Top);
		}

		public void MoveRight()
		{
			if (GetDirection().X < 0)
				return;
			Snake.Get<Snake.Body>().Direction = new Point(blockSize, 0);
		}

		public void MoveUp()
		{
			if (GetDirection().Y > 0)
				return;
			Snake.Get<Snake.Body>().Direction = new Point(0, -blockSize);
		}

		public void MoveDown()
		{
			if (GetDirection().Y < 0)
				return;
			Snake.Get<Snake.Body>().Direction = new Point(0, blockSize);
		}

		private void InitializeSnake()
		{
			Snake = new Snake(entitySystem, gridSize);
			entitySystem.Add(Snake);
			AddEventListeners();
		}

		public Snake Snake { get; private set; }

		private void AddEventListeners()
		{
			Snake.Get<Snake.Body>().DetectSnakeCollisionWithChunk += SnakeCollisionWithChunk;
			Snake.Get<Snake.Body>().SnakeCollidesWithBorderOrItself += Reset;
		}

		private void SnakeCollisionWithChunk(Point trailingVector)
		{
			if (Chunk.TopLeft == Snake.Get<Snake.Body>().BodyParts[0].TopLeft)
			{
				Chunk.SpawnAtRandomLocation();
				GrowSnakeInSize(trailingVector);
			}
		}

		private void GrowSnakeInSize(Point trailingVector)
		{
			var snakeBodyParts = Snake.Get<Snake.Body>().BodyParts;
			var tail = snakeBodyParts[snakeBodyParts.Count - 1].DrawArea.TopLeft;
			var newBodyPart = new Rect(CalculateTrailDrawArea(trailingVector, tail), Color.Teal);
			snakeBodyParts.Add(newBodyPart);
			entitySystem.Add(newBodyPart);
			screen.Window.Title = "Snake - Length: " + snakeBodyParts.Count;
		}

		private Rectangle CalculateTrailDrawArea(Point trailingVector, Point tail)
		{
			return new Rectangle(new Point(tail.X + trailingVector.X, tail.Y + trailingVector.Y),
				new Size(blockSize));
		}

		public void Reset()
		{
			RemoveEventListeners();
			Snake.Dispose(entitySystem);
			InitializeSnake();
			screen.Window.Title = "Snake - Game Over";
		}

		private void SpawnFirstChunk()
		{
			Chunk = new Chunk(gridSize, blockSize);
			RespawnChunk();
			entitySystem.Add(Chunk);
		}

		public Chunk Chunk { get; private set; }

		public void RespawnChunk()
		{
			while (Chunk.IsCollidingWithSnake(Snake.Get<Snake.Body>().BodyParts))
				Chunk.SpawnAtRandomLocation();
		}

		private void RemoveEventListeners()
		{
			Snake.Get<Snake.Body>().DetectSnakeCollisionWithChunk -= SnakeCollisionWithChunk;
			Snake.Get<Snake.Body>().SnakeCollidesWithBorderOrItself -= Reset;
		}
	}
}