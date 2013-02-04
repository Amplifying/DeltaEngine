using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering;

namespace Breakout
{
	/// <summary>
	/// Extends the ball to live and communicate with the level
	/// </summary>
	public class BallInLevel : Ball
	{
		public BallInLevel(Paddle paddle, Content content, Input input, Level level)
			: base(paddle, content, input)
		{
			this.level = level;
		}

		private readonly Level level;
		public Level CurrentLevel
		{
			get { return level; }
		}

		protected override void UpdateInFlight(float timeDelta)
		{
			base.UpdateInFlight(timeDelta);
			HandleBrickCollisions();
		}

		private void HandleBrickCollisions()
		{
			HandleBrickCollision(level.GetBrickAt(Position.X - Radius, Position.Y), Direction.Left);
			HandleBrickCollision(level.GetBrickAt(Position.X, Position.Y - Radius), Direction.Top);
			HandleBrickCollision(level.GetBrickAt(Position.X + Radius, Position.Y), Direction.Right);
			HandleBrickCollision(level.GetBrickAt(Position.X, Position.Y + Radius), Direction.Bottom);
		}

		private void HandleBrickCollision(Sprite brick, Direction collisionSide)
		{
			if (brick == null)
				return;

			level.Explode(brick);
			ReflectVelocity(collisionSide);
		}

		public override void ResetBall()
		{
			level.LostLive(Position);
			base.ResetBall();
		}
	}
}