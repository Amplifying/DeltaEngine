using System;
using System.Diagnostics;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Represents a 2D vector, which is useful for screen positions (sprites, mouse, touch, etc.)
	/// </summary>
	[DebuggerDisplay("Point({X}, {Y})")]
	public struct Point : IEquatable<Point>
	{
		public Point(float x, float y)
			: this()
		{
			X = x;
			Y = y;
		}

		public Point(string pointAsString)
			: this()
		{
			float[] components = pointAsString.SplitIntoFloats();
			if (components.Length != 2)
				throw new InvalidNumberOfComponents();

			X = components[0];
			Y = components[1];
		}

		public float X { get; set; }
		public float Y { get; set; }

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Point Zero = new Point();
		public static readonly Point One = new Point(1, 1);
		public static readonly Point Half = new Point(0.5f, 0.5f);
		public static readonly Point UnitX = new Point(1, 0);
		public static readonly Point UnitY = new Point(0, 1);
		public const int SizeInBytes = 2 * 4;

		public static Point operator +(Point p1, Point p2)
		{
			return new Point(p1.X + p2.X, p1.Y + p2.Y);
		}

		public static Point operator -(Point p1, Point p2)
		{
			return new Point(p1.X - p2.X, p1.Y - p2.Y);
		}

		public static Point operator *(float f, Point p)
		{
			return new Point(p.X * f, p.Y * f);
		}

		public static Point operator *(Point p, float f)
		{
			return new Point(p.X * f, p.Y * f);
		}

		public static Point operator /(Point p, float f)
		{
			return new Point(p.X / f, p.Y / f);
		}

		public static bool operator !=(Point p1, Point p2)
		{
			return p1.Equals(p2) == false;
		}

		public static bool operator ==(Point p1, Point p2)
		{
			return p1.Equals(p2);
		}

		public static Point operator -(Point p)
		{
			return new Point(-p.X, -p.Y);
		}

		public bool Equals(Point other)
		{
			return X.IsNearlyEqual(other.X) && Y.IsNearlyEqual(other.Y);
		}

		public override bool Equals(object other)
		{
			return other is Point ? Equals((Point)other) : base.Equals(other);
		}

		public static implicit operator Point(Size s)
		{
			return new Point(s.Width, s.Height);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		public override string ToString()
		{
			return "(" + X.ToInvariantString() + ", " + Y.ToInvariantString() + ")";
		}

		public float DistanceTo(Point other)
		{
			float distanceX = X - other.X;
			float distanceY = Y - other.Y;
			return (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
		}

		public float DistanceToSquared(Point other)
		{
			float distanceX = X - other.X;
			float distanceY = Y - other.Y;
			return distanceX * distanceX + distanceY * distanceY;
		}

		public Point DirectionTo(Point other)
		{
			return other - this;
		}

		public void ReflectIfHittingBorder(Rectangle box, Rectangle borders)
		{
			if (box.Left <= borders.Left)
				X = X.Abs();
			if (box.Right >= borders.Right)
				X = -X.Abs();
			if (box.Top <= borders.Top)
				Y = Y.Abs();
			if (box.Bottom >= borders.Bottom)
				Y = -Y.Abs();
		}

		public static Point Lerp(Point point1, Point point2, float percentage)
		{
			float x = MathExtensions.Lerp(point1.X, point2.X, percentage);
			float y = MathExtensions.Lerp(point1.Y, point2.Y, percentage);
			return new Point(x, y);
		}

		public void RotateAround(Point center, float angleInDegrees, FastTrig fastTrig = null)
		{
			float rotationSin = fastTrig != null
				? fastTrig.Sin(angleInDegrees) : MathExtensions.Sin(angleInDegrees);

			float rotationCos = fastTrig != null
				? fastTrig.Cos(angleInDegrees) : MathExtensions.Cos(angleInDegrees);

			RotateAround(center, rotationSin, rotationCos);
		}

		public void RotateAround(Point center, float rotationSin, float rotationCos)
		{
			var translatedPoint = this - center;
			X = center.X + translatedPoint.X * rotationCos - translatedPoint.Y * rotationSin;
			Y = center.Y + translatedPoint.X * rotationSin + translatedPoint.Y * rotationCos;
		}

		public void Normalize()
		{
			var length = (float)Math.Sqrt(X * X + Y * Y);
			X /= length;
			Y /= length;
		}

		public float DotProduct(Point point)
		{
			return X * point.X + Y * point.Y;
		}
	}
}