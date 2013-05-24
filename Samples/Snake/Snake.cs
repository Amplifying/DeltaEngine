using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Shapes;

namespace Snake
{
	/// <summary>
	/// This class holds data about the snake body and checks for snake collisions with either 
	/// itself or with the borders and whether the snake must grow in size.
	/// </summary>
	public class Snake : Entity
	{
		public Snake(int gridSize)
		{
			Add(new Body(gridSize));
			Add<SnakeHandler>();
		}

		public void Dispose()
		{
			foreach (var bodyPart in Get<Body>().BodyParts)
				bodyPart.IsActive = false;

			Get<Body>().BodyParts.Clear();
			Remove<Body>();
			Remove<SnakeHandler>();
		}

		public class Body
		{
			public Body(int gridSize)
			{
				this.gridSize = gridSize;
				blockSize = 1.0f / gridSize;
				SpawnSnake();
			}

			private readonly int gridSize;
			private readonly float blockSize;

			private void SpawnSnake()
			{
				BodyParts = new List<Rect>();
				Direction = new Point(0, -blockSize);
				PlaceSnakeHead();
				AddSnakeBody();
			}

			public List<Rect> BodyParts { get; private set; }
			public Point Direction { get; set; }

			private void PlaceSnakeHead()
			{
				var startPosition = blockSize * (float)Math.Floor(gridSize / 2.0f);
				var firstPart = new Rect(CalculateHeadDrawArea(startPosition), Color.Teal);
				BodyParts.Add(firstPart);
			}

			private Rectangle CalculateHeadDrawArea(float startPosition)
			{
				return new Rectangle(new Point(startPosition, startPosition), new Size(1.0f / gridSize));
			}

			public void AddSnakeBody()
			{
				var snakeHead = BodyParts[BodyParts.Count - 1].DrawArea;
				var newTail = new Rect(CalculateBodyDrawArea(snakeHead), Color.Teal);
				BodyParts.Add(newTail);
			}

			private Rectangle CalculateBodyDrawArea(Rectangle snakeHead)
			{
				return new Rectangle(snakeHead.Left, snakeHead.Top + blockSize, blockSize, blockSize);
			}

			internal void MoveBody()
			{
				trailingVector = GetTrailingVector();
				MoveBodyTowardsHead();
				MoveHeadInDesiredDirection();
			}

			private Point trailingVector;

			public Point GetTrailingVector()
			{
				var tail = BodyParts[BodyParts.Count - 1].DrawArea.TopLeft;
				var partBeforeTail = BodyParts[BodyParts.Count - 2].DrawArea.TopLeft;
				return new Point(tail.X - partBeforeTail.X, tail.Y - partBeforeTail.Y);
			}

			private void MoveBodyTowardsHead()
			{
				for (int count = BodyParts.Count - 1; count >= 1; count--)
					BodyParts[count].DrawArea = BodyParts[count - 1].DrawArea;
			}

			private void MoveHeadInDesiredDirection()
			{
				var newHeadPos = new Point(BodyParts[0].DrawArea.Left + Direction.X,
					BodyParts[0].DrawArea.Top + Direction.Y);
				BodyParts[0].DrawArea = new Rectangle(newHeadPos, new Size(blockSize));
			}

			internal void CheckSnakeCollidesWithChunk()
			{
				if (DetectSnakeCollisionWithChunk != null)
					DetectSnakeCollisionWithChunk(trailingVector);
			}

			public event Action<Point> DetectSnakeCollisionWithChunk;

			internal void CheckSnakeCollisionWithBorderOrItself()
			{
				if (SnakeCollidesWithBorderOrItself == null)
					return; //ncrunch: no coverage

				var snakeHead = BodyParts[0];
				if (SnakeCollidesWithItself(snakeHead) || SnakeCollidesWithBorders(snakeHead))
					SnakeCollidesWithBorderOrItself();
			}

			private bool SnakeCollidesWithItself(Rect snakeHead)
			{
				for (int count = 3; count < BodyParts.Count; count++)
					if (snakeHead.DrawArea.TopLeft == BodyParts[count].DrawArea.TopLeft)
						return true;

				return false;
			}

			private bool SnakeCollidesWithBorders(Rect snakeHead)
			{
				if ((snakeHead.DrawArea.Left < blockSize - 0.01f ||
					snakeHead.DrawArea.Top < blockSize - 0.01f ||
					snakeHead.DrawArea.Left > 1 - blockSize - 0.01f ||
					snakeHead.DrawArea.Top > 1 - blockSize - 0.01f))
					return true;

				return false;
			}

			public event Action SnakeCollidesWithBorderOrItself;
		}

		internal class SnakeHandler : EntityHandler
		{
			public override void Handle(List<Entity> entities)
			{
				foreach (var entity in entities)
					if (Time.Current.CheckEvery(0.15f))
					{
						entity.Get<Body>().MoveBody();
						entity.Get<Body>().CheckSnakeCollidesWithChunk();
						entity.Get<Body>().CheckSnakeCollisionWithBorderOrItself();
					}
			}

			public override EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Normal; }
			}
		}
	}
}