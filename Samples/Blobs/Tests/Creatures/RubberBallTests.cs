using System;
using Blobs.Creatures;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;

namespace Blobs.Tests.Creatures
{
	public class RubberBallTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Bounce(Type resolver)
		{
			RubberBall ball = null;
			Start(resolver, (RubberBall b) =>
			{
				ball = b;
				ball.Center = new Point(0.1f, 0.2f);
				ball.Radius = 0.05f;
				ball.Color = Color.Orange;
				ball.Velocity = new Point(0.2f, 0.0f);
			}, () =>
			{
				if (ball.Center.Y + ball.RadiusY > 0.8f)
					ball.Collision = new Collision(new Point(ball.Center.X, 0.8f), -Point.UnitY, null);
			});
		}
	}
}