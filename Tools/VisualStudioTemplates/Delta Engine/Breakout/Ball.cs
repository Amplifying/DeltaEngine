using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Input.Devices;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;

namespace $safeprojectname$
{
	/// <summary>
	/// The player interacts with the ball with his paddle and navigates it to destroy bricks.
	/// </summary>
	public class Ball : Sprite
	{
		public Ball(Paddle paddle, Content content, InputCommands inputCommands)
			: base(content.Load<Image>("Ball"), Rectangle.Zero)
		{
			this.paddle = paddle;
			UpdateOnPaddle();
			fireBallSound = content.Load<Sound>("PaddleBallStart");
			collisionSound = content.Load<Sound>("BallCollision");
			RegisterFireBallCommand(inputCommands);
		}

		protected readonly Paddle paddle;
		private readonly Sound fireBallSound;
		private readonly Sound collisionSound;

		public virtual void ResetBall()
		{
			UpdateOnPaddle();
			velocity = Point.Zero;
		}

		public override void Dispose()
		{
			base.Dispose();
			paddle.Dispose();
		}

		protected bool isOnPaddle;
		protected Point velocity;

		private void RegisterFireBallCommand(InputCommands inputCommands)
		{
			inputCommands.Add(Key.Space, State.Pressing, () => FireBallFromPaddle());
			inputCommands.Add(MouseButton.Left, State.Pressing, mouse => FireBallFromPaddle());
			inputCommands.Add((Touch touch) => FireBallFromPaddle());
			inputCommands.Add(GamePadButton.A, State.Pressing, () => FireBallFromPaddle());
		}

		private void FireBallFromPaddle()
		{
			if (!isOnPaddle || !IsVisible)
				return;

			isOnPaddle = false;
			float randomXSpeed = randomizer.Get(-0.15f, 0.15f);
			velocity = new Point(randomXSpeed == 0f ? 0.01f : randomXSpeed, StartBallSpeedY);
			fireBallSound.Play();
		}

		private readonly PseudoRandom randomizer = new PseudoRandom();
		private const float StartBallSpeedY = -1f;

		protected override void Render(Renderer renderer, Time time)
		{
			borders = renderer.Screen.Viewport;
			if (isOnPaddle)
				UpdateOnPaddle();
			else
				UpdateInFlight(time.CurrentDelta);

			DrawArea = Rectangle.FromCenter(Position, new Size(Height));
			base.Render(renderer, time);
		}

		private Rectangle borders;
		public Point Position { get; protected set; }
		private const float Height = Radius * 2.0f;
		protected const float Radius = 0.02f;

		private void UpdateOnPaddle()
		{
			isOnPaddle = true;
			Position = new Point(paddle.Position.X, paddle.Position.Y - Radius);
		}

		protected virtual void UpdateInFlight(float timeDelta)
		{
			Position += velocity * timeDelta;
			HandleBorderCollisions();
			HandlePaddleCollision();
		}

		private void HandleBorderCollisions()
		{
			if (Position.X < borders.Left + Radius)
				HandleBorderCollision(Direction.Left);
			else if (Position.X > borders.Right - Radius)
				HandleBorderCollision(Direction.Right);

			if (Position.Y < borders.Top + Radius)
				HandleBorderCollision(Direction.Top);
			else if (Position.Y > borders.Bottom - Radius)
				HandleBorderCollision(Direction.Bottom);
		}

		protected enum Direction
		{
			Left,
			Top,
			Right,
			Bottom,
		}

		protected void ReflectVelocity(Direction collisionSide)
		{
			switch (collisionSide)
			{
			case Direction.Left:
				velocity.X = Math.Abs(velocity.X);
				break;
			case Direction.Top:
				velocity.Y = Math.Abs(velocity.Y);
				break;
			case Direction.Right:
				velocity.X = -Math.Abs(velocity.X);
				break;
			case Direction.Bottom:
				velocity.Y = -Math.Abs(velocity.Y);
				break;
			}
		}

		private void HandleBorderCollision(Direction collisionAtBorder)
		{
			ReflectVelocity(collisionAtBorder);
			if (collisionAtBorder == Direction.Bottom)
				ResetBall();
			else
				collisionSound.Play(0.5f);
		}

		private void HandlePaddleCollision()
		{
			if (IsInAreaOfPaddle())
				SetNewVelocityAfterPaddleCollision();
		}

		private bool IsInAreaOfPaddle()
		{
			if (Position.Y + Radius > paddle.Position.Y && velocity.Y > 0)
				return Position.X + Radius > paddle.Position.X - Paddle.HalfWidth &&
					Position.X - Radius < paddle.Position.X + Paddle.HalfWidth;

			return false;
		}

		private void SetNewVelocityAfterPaddleCollision()
		{
			velocity.X += (Position.X - paddle.Position.X) * SpeedXIncrease;
			velocity.Y = -Math.Abs(velocity.Y) * SpeedYIncrease;
			velocity.X = velocity.X.Clamp(-5f, 5f);
			velocity.Y = velocity.Y.Clamp(-5f, 0f);
			collisionSound.Play(0.6f);
		}

		private const float SpeedYIncrease = 1.015f;
		private const float SpeedXIncrease = 2.5f;
	}
}