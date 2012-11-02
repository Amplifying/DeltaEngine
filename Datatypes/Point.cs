using System;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// This struct represents a 2D vector - useful for position of mouse on screen etc.
	/// </summary>
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
			float[] floats = pointAsString.SplitIntoFloats();
			if (floats.Length != 2)
				throw new InvalidNumberOfComponents();

			X = floats[0];
			Y = floats[1];
		}

		public float X { get; set; }
		public float Y { get; set; }

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Point Zero = new Point();
		public static readonly Point One = new Point(1, 1);
		public static readonly Point Half = new Point(0.5f, 0.5f);
		public static readonly Point UnitX = new Point(1, 0);
		public static readonly Point UnitY = new Point(0, 1);

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
	}
}