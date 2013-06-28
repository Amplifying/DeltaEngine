using System;
using Blobs.Creatures;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Blobs.Tests.Creatures
{
	public class RubberBallTests : TestWithMocksOrVisually
	{
		[Test]
		public void Bounce()
		{
			var ball = new RubberBall();
			ball.Center = new Point(0.1f, 0.2f);
			ball.Radius = 0.05f;
			ball.Color = Color.Orange;
			ball.Velocity = new Point(0.2f, 0.0f);
			if (ball.Center.Y + ball.RadiusY > 0.8f)
				ball.Collision = new Collision(new Point(ball.Center.X, 0.8f), -Point.UnitY, null);
		}
	}
}