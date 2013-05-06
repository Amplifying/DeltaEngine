using System;
using Blobs.Creatures;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Shapes;

namespace Blobs
{
	/// <summary>
	/// A rotateable rectangle with flat top and bottom, and curved sides
	/// </summary>
	public class Platform : IDisposable
	{
		public Platform(EntitySystem entitySystem, Rectangle rectangle, float rotation = 0.0f)
		{
			this.entitySystem = entitySystem;
			this.rectangle = rectangle;
			this.rotation = rotation;
			UpdateRendering();
		}

		private readonly EntitySystem entitySystem;
		public Color Color;
		private readonly float rotation;
		private Rectangle rectangle;

		private void UpdateRendering()
		{
			leftCircle.Remove<RenderPolygonOutline>();
			entitySystem.Add(leftCircle);
			rightCircle.Remove<RenderPolygonOutline>();
			entitySystem.Add(rightCircle);
			middle.Remove<RenderPolygonOutline>();
			entitySystem.Add(middle);
		}

		private readonly Ellipse leftCircle = new Ellipse(Rectangle.Zero, Color.White)
		{
			RenderLayer = PlatformRenderLayer
		};

		private const int PlatformRenderLayer = 8;

		private readonly Ellipse rightCircle = new Ellipse(Rectangle.Zero, Color.White)
		{
			RenderLayer = PlatformRenderLayer
		};

		private readonly Polygon middle = new Polygon(Color.White)
		{
			RenderLayer = PlatformRenderLayer
		};

		public void Run()
		{
			if (HasColorChanged())
				ChangeCircleColors();

			if (HasRenderingChanged())
				PositionTriangleAndCircles();
		}

		private static bool HasColorChanged()
		{
			//TODO: Platform.HasColorChanged
			return true;
		}

		private void ChangeCircleColors()
		{
			leftCircle.Color = Color;
			rightCircle.Color = Color;
			middle.Color = Color;
		}

		private static bool HasRenderingChanged()
		{
			//TODO: Platform.HasRenderingChanged
			return true;
		}

		private void PositionTriangleAndCircles()
		{
			StoreRotatedCorners();
			PositionLeftCircle();
			PositionRightCircle();
		}

		private void StoreRotatedCorners()
		{
			topLeft = rectangle.TopLeft;
			topLeft.RotateAround(rectangle.Center, rotation);
			topRight = rectangle.TopRight;
			topRight.RotateAround(rectangle.Center, rotation);
			bottomLeft = rectangle.BottomLeft;
			bottomLeft.RotateAround(rectangle.Center, rotation);
			bottomRight = rectangle.BottomRight;
			bottomRight.RotateAround(rectangle.Center, rotation);
			UpdateMiddle();
		}

		private Point topLeft;
		private Point topRight;
		private Point bottomLeft;
		private Point bottomRight;

		private void UpdateMiddle()
		{
			middle.Points.Clear();
			middle.Points.Add(topLeft);
			middle.Points.Add(topRight);
			middle.Points.Add(bottomRight);
			middle.Points.Add(bottomLeft);
		}

		private void PositionLeftCircle()
		{
			leftCircle.Center = (topLeft + bottomLeft) / 2;
			leftCircle.Radius = leftCircle.Center.DistanceTo(topLeft);
		}

		private void PositionRightCircle()
		{
			rightCircle.Center = (topRight + bottomRight) / 2;
			rightCircle.Radius = rightCircle.Center.DistanceTo(topRight);
		}

		public void CheckForCollision(object solid)
		{
			ball = solid as RubberBall;
			if (ball != null)
				CheckForCollisionWithBall();
		}

		private RubberBall ball;

		private void CheckForCollisionWithBall()
		{
			if (!CheckForCollisionOfBallWithRectangle())
				if (!CheckForCollisionOfBallWithCircle(leftCircle))
					CheckForCollisionOfBallWithCircle(rightCircle);
		}

		private bool CheckForCollisionOfBallWithRectangle()
		{
			return CheckForIntersectionOfBallWithEdge(topLeft, topRight) ||
				CheckForIntersectionOfBallWithEdge(topRight, bottomRight) ||
				CheckForIntersectionOfBallWithEdge(bottomLeft, bottomRight) ||
				CheckForIntersectionOfBallWithEdge(topLeft, bottomLeft);
		}

		private bool CheckForIntersectionOfBallWithEdge(Point start, Point finish)
		{
			float dx = finish.X - start.X;
			float dy = finish.Y - start.Y;
			float a = dx * dx + dy * dy;
			float b = 2 * (dx * (start.X - ball.Center.X) + dy * (start.Y - ball.Center.Y));
			if (DoesBallFailToIntersectInfiniteEdge(a, b, start))
				return false;

			float t = -b / (2.0f * a);
			if (t < 0.0f || t > 1.0f)
				return false;

			BallIntersectsEdge(new Point(start.X + t * dx, start.Y + t * dy), new Point(-dy, dx));
			return true;
		}

		private bool DoesBallFailToIntersectInfiniteEdge(float a, float b, Point start)
		{
			float c = (start.X - ball.Center.X) * (start.X - ball.Center.X) +
				(start.Y - ball.Center.Y) * (start.Y - ball.Center.Y) - ball.Radius * ball.Radius;
			float det = b * b - 4 * a * c;
			return (a <= 0.0000001f) || (det < 0.0f);
		}

		private void BallIntersectsEdge(Point collisionPoint, Point collisionNormal)
		{
			var collision = new Collision(collisionPoint, collisionNormal, this);
			if (collision.Normal.DotProduct(collision.Point.DirectionTo(ball.PreviousFrameCenter)) <
				0.0f)
				collision.Normal = -collision.Normal;

			ball.Collision = collision;
		}

		private bool CheckForCollisionOfBallWithCircle(Ellipse circle)
		{
			var direction = circle.Center.DirectionTo(ball.Center);
			direction.Normalize();
			if (circle.Center.DistanceTo(ball.Center) > circle.Radius + ball.Radius)
				return false;

			BallCollidesWithCircle(circle, direction);
			return true;
		}

		private void BallCollidesWithCircle(Ellipse circle, Point direction)
		{
			Point collisionPoint = circle.Center + direction * circle.Radius;
			ball.Collision = new Collision(collisionPoint, direction, this);
		}

		public void Dispose()
		{
			leftCircle.IsActive = false;
			rightCircle.IsActive = false;
			middle.IsActive = false;
		}
	}
}