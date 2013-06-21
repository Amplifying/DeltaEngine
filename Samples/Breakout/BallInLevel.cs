using DeltaEngine.Content;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;

namespace Breakout
{
	/// <summary>
	/// Extends the ball to live and communicate with the level
	/// </summary>
	public class BallInLevel : Ball
	{
		public BallInLevel(Paddle paddle, ContentLoader content, InputCommands inputCommands,
			Level level)
			: base(paddle, content, inputCommands)
		{
			Level = level;
		}

		public Level Level { get; private set; }

		protected override void UpdateInFlight(float timeDelta)
		{
			base.UpdateInFlight(timeDelta);
			HandleBrickCollisions();
		}

		private void HandleBrickCollisions()
		{
			HandleBrickCollision(Level.GetBrickAt(Position.X - Radius, Position.Y), Direction.Left);
			HandleBrickCollision(Level.GetBrickAt(Position.X, Position.Y - Radius), Direction.Top);
			HandleBrickCollision(Level.GetBrickAt(Position.X + Radius, Position.Y), Direction.Right);
			HandleBrickCollision(Level.GetBrickAt(Position.X, Position.Y + Radius), Direction.Bottom);
		}

		private void HandleBrickCollision(Sprite brick, Direction collisionSide)
		{
			if (brick == null)
				return;

			Level.Explode(brick, Position);
			ReflectVelocity(collisionSide);
		}

		public override void ResetBall()
		{
			Level.LifeLost(Position);
			base.ResetBall();
		}
	}
}