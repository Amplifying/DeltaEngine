using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Sprites;

namespace Breakout
{
	/// <summary>
	/// The player interacts with the ball with his paddle and navigates it to destroy bricks.
	/// </summary>
	public class Ball : Sprite, Runner<ScreenSpace>
	{
		public Ball(Paddle paddle, ContentLoader content, InputCommands inputCommands)
			: base(content.Load<Image>("Ball"), Rectangle.Zero)
		{
			this.paddle = paddle;
			fireBallSound = content.Load<Sound>("PaddleBallStart");
			collisionSound = content.Load<Sound>("BallCollision");
			UpdateOnPaddle();
			RegisterFireBallCommand(inputCommands);
		}

		private readonly Paddle paddle;
		private readonly Sound fireBallSound;
		private readonly Sound collisionSound;

		private void UpdateOnPaddle()
		{
			isOnPaddle = true;
			Position = new Point(paddle.Position.X, paddle.Position.Y - Radius);
		}

		protected bool isOnPaddle;

		private void RegisterFireBallCommand(InputCommands inputCommands)
		{
			inputCommands.Add(Key.Space, State.Pressing, FireBallFromPaddle);
			inputCommands.Add(MouseButton.Left, State.Pressing, mouse => FireBallFromPaddle());
			inputCommands.Add(touch => FireBallFromPaddle());
			inputCommands.Add(GamePadButton.A, State.Pressing, FireBallFromPaddle);
		}

		private void FireBallFromPaddle()
		{
			if (!isOnPaddle || Visibility != Visibility.Show)
				return;

			isOnPaddle = false;
			float randomXSpeed = Randomizer.Current.Get(-0.15f, 0.15f);
			velocity = new Point(randomXSpeed.Abs() < 0.01f ? 0.01f : randomXSpeed, StartBallSpeedY);
			fireBallSound.Play();
		}

		protected Point velocity;
		private const float StartBallSpeedY = -1f;

		public virtual void ResetBall()
		{
			UpdateOnPaddle();
			velocity = Point.Zero;
		}

		public virtual void Run(ScreenSpace screen)
		{
			if (isOnPaddle)
				UpdateOnPaddle();
			else
				UpdateInFlight(Time.Current.Delta);

			float aspect = screen.ToPixelSpace(BallSize).AspectRatio;
			DrawArea = Rectangle.FromCenter(Position, new Size(Height / aspect, Height));
		}

		public Point Position { get; protected set; }
		public static readonly Size BallSize = new Size(Height);
		private const float Height = Radius * 2.0f;
		internal const float Radius = 0.02f;

		protected virtual void UpdateInFlight(float timeDelta)
		{
			Position += velocity * timeDelta;
			HandleBorderCollisions();
			HandlePaddleCollision();
		}

		private void HandleBorderCollisions()
		{
			if (Position.X < Radius)
				HandleBorderCollision(Direction.Left);
			else if (Position.X > 1.0f - Radius)
				HandleBorderCollision(Direction.Right);

			if (Position.Y < Radius)
				HandleBorderCollision(Direction.Top);
			else if (Position.Y > 1.0f - Radius)
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

		public void Dispose()
		{
			Visibility = Visibility.Hide;
			paddle.Visibility = Visibility.Hide;
		}
	}
}