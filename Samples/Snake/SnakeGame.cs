using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;

namespace Snake
{
	/// <summary>
	/// This is the main game class holding the control of the entire snake game, user input and the 
	/// interaction of the snake with the borders and the chunks
	/// </summary>
	public class SnakeGame
	{
		public SnakeGame(ScreenSpace screen, InputCommands input, ContentLoader content)
		{
			gridSize = 25;
			blockSize = 1.0f / gridSize;
			this.screen = screen;
			this.content = content;
			this.input = input;
			SetupPlayArea();
			SetInput();
			InitializeSnake();
			SpawnFirstChunk();
		}

		private readonly int gridSize;
		private readonly float blockSize;
		private readonly ScreenSpace screen;
		private readonly ContentLoader content;
		private readonly InputCommands input;

		private void SetupPlayArea()
		{
			screen.Window.Title = "Snake - Let's go";
			screen.Window.TotalPixelSize = new Size(500, 500);
			screen.Window.BackgroundColor = Color.Red;
			new Rect(CalculateBackgroundDrawArea(), Color.Black);
		}

		private Rectangle CalculateBackgroundDrawArea()
		{
			return new Rectangle(blockSize, blockSize, blockSize * (gridSize - 2),
				blockSize * (gridSize - 2));
		}

		private void SetInput()
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
			snakeBody.Direction = new Point(-blockSize, 0);
		}

		private Point GetDirection()
		{
			var snakeHead = snakeBody.BodyParts[0];
			var partNextToSnakeHead = snakeBody.BodyParts[1];
			var direction = new Point(snakeHead.DrawArea.Left - partNextToSnakeHead.DrawArea.Left,
				snakeHead.DrawArea.Top - partNextToSnakeHead.DrawArea.Top);
			return direction;
		}

		public void MoveRight()
		{
			if (GetDirection().X < 0)
				return;
			snakeBody.Direction = new Point(blockSize, 0);
		}

		public void MoveUp()
		{
			if (GetDirection().Y > 0)
				return;
			snakeBody.Direction = new Point(0, -blockSize);
		}

		public void MoveDown()
		{
			if (GetDirection().Y < 0)
				return;
			snakeBody.Direction = new Point(0, blockSize);
		}

		private void InitializeSnake()
		{
			Snake = new Snake(gridSize);
			snakeBody = Snake.Get<Snake.Body>();
			AddEventListeners();
		}

		private Snake.Body snakeBody;
		public Snake Snake { get; private set; }

		private void AddEventListeners()
		{
			snakeBody.DetectSnakeCollisionWithChunk += SnakeCollisionWithChunk;
			snakeBody.SnakeCollidesWithBorderOrItself += Reset;
		}

		private void SnakeCollisionWithChunk(Point trailingVector)
		{
			if (Chunk.TopLeft == snakeBody.BodyParts[0].TopLeft)
			{
				Chunk.SpawnAtRandomLocation();
				GrowSnakeInSize(trailingVector);
			}
		}

		private void GrowSnakeInSize(Point trailingVector)
		{
			var snakeBodyParts = snakeBody.BodyParts;
			var tail = snakeBodyParts[snakeBodyParts.Count - 1].DrawArea.TopLeft;
			var newBodyPart = new Rect(CalculateTrailDrawArea(trailingVector, tail), Color.Teal);
			snakeBodyParts.Add(newBodyPart);
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
			Snake.Dispose();
			DisplayGameOverMessage();
		}

		private void DisplayGameOverMessage()
		{
			Chunk.IsActive = false;
			var fontGameOverText = new Font(content, "Tahoma30");
			var fontReplayText = new Font(content, "Verdana12");
			gameOverMsg = new FontText(fontGameOverText, "Game Over", Point.Half) { Color = Color.Red };
			restartMsg = new FontText(fontReplayText, "Do you want to continue (Y/N)",
				new Point(0.5f, 0.7f)) { Color = Color.Yellow };

			yesCommand = input.Add(Key.Y, State.Pressed, RestartGame);
			noCommand = input.Add(Key.N, State.Pressed, CloseGame);
		}

		private Command yesCommand;
		private Command noCommand;
		private FontText gameOverMsg;
		private FontText restartMsg;

		private void RestartGame()
		{
			input.Remove(yesCommand);
			input.Remove(noCommand);
			gameOverMsg.IsActive = false;
			restartMsg.IsActive = false;
			InitializeSnake();
			SpawnFirstChunk();
		}

		private void CloseGame()
		{
			screen.Window.Dispose();
		}

		private void SpawnFirstChunk()
		{
			Chunk = new Chunk(gridSize, blockSize);
			RespawnChunk();
		}

		public Chunk Chunk { get; private set; }

		public void RespawnChunk()
		{
			while (Chunk.IsCollidingWithSnake(snakeBody.BodyParts))
				Chunk.SpawnAtRandomLocation();
		}

		private void RemoveEventListeners()
		{
			snakeBody.DetectSnakeCollisionWithChunk -= SnakeCollisionWithChunk;
			snakeBody.SnakeCollidesWithBorderOrItself -= Reset;
		}
	}
}