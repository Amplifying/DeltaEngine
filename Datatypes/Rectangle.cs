using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Holds data for a rectangle by specifying its top left corner and the width and height.
	/// </summary>
	[DebuggerDisplay("Rectangle(Left={Left}, Top={Top}, Width={Width}, Height={Height})")]
	public struct Rectangle : IEquatable<Rectangle>
	{
		public Rectangle(float left, float top, float width, float height)
			: this()
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		public float Left { get; set; }
		public float Top { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }

		public Rectangle(Point position, Size size)
			: this(position.X, position.Y, size.Width, size.Height) {}

		public Rectangle(string rectangleAsString)
			: this()
		{
			string[] componentStrings = rectangleAsString.Split(' ');
			if (componentStrings.Length != 4)
				throw new InvalidNumberOfComponents();

			Left = componentStrings[0].FromInvariantString(0.0f);
			Top = componentStrings[1].FromInvariantString(0.0f);
			Width = componentStrings[2].FromInvariantString(0.0f);
			Height = componentStrings[3].FromInvariantString(0.0f);
		}

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Rectangle Zero = new Rectangle();
		public static readonly Rectangle One = new Rectangle(Point.Zero, Size.One);
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Rectangle));

		public float Right
		{
			get { return Left + Width; }
			set { Left = value - Width; }
		}

		public float Bottom
		{
			get { return Top + Height; }
			set { Top = value - Height; }
		}

		public Size Size
		{
			get { return new Size(Width, Height); }
		}

		public Point TopLeft
		{
			get { return new Point(Left, Top); }
		}

		public Point TopRight
		{
			get { return new Point(Left + Width, Top); }
		}

		public Point BottomLeft
		{
			get { return new Point(Left, Top + Height); }
		}

		public Point BottomRight
		{
			get { return new Point(Left + Width, Top + Height); }
		}

		public Point Center
		{
			get { return new Point(Left + Width / 2, Top + Height / 2); }
			set
			{
				Left = value.X - Width / 2;
				Top = value.Y - Height / 2;
			}
		}

		public static Rectangle Lerp(Rectangle rectangle1, Rectangle rectangle2, float percentage)
		{
			return new Rectangle(Point.Lerp(rectangle1.TopLeft, rectangle2.TopLeft, percentage),
				Size.Lerp(rectangle1.Size, rectangle2.Size, percentage));
		}

		public static Rectangle FromCenter(float x, float y, float width, float height)
		{
			return FromCenter(new Point(x, y), new Size(width, height));
		}

		public static Rectangle FromCenter(Point center, Size size)
		{
			return new Rectangle(new Point(center.X - size.Width / 2, center.Y - size.Height / 2), size);
		}

		public bool Contains(Point position)
		{
			return position.X >= Left && position.X < Right && position.Y >= Top && position.Y < Bottom;
		}

		public float Aspect
		{
			get { return Width / Height; }
		}

		public Rectangle Reduce(Size size)
		{
			return new Rectangle(Left + size.Width / 2, Top + size.Height / 2, Width - size.Width,
				Height - size.Height);
		}

		public Rectangle GetInnerRectangle(Rectangle relativeRectangle)
		{
			return new Rectangle(Left + Width * relativeRectangle.Left,
				Top + Height * relativeRectangle.Top, Width * relativeRectangle.Width,
				Height * relativeRectangle.Height);
		}

		public Rectangle Move(Point translation)
		{
			return new Rectangle(Left + translation.X, Top + translation.Y, Width, Height);
		}

		public static bool operator ==(Rectangle rect1, Rectangle rect2)
		{
			return rect1.Equals(rect2);
		}

		public static bool operator !=(Rectangle rect1, Rectangle rect2)
		{
			return !rect1.Equals(rect2);
		}

		public override bool Equals(object obj)
		{
			return obj is Rectangle ? Equals((Rectangle)obj) : base.Equals(obj);
		}

		public bool Equals(Rectangle other)
		{
			return TopLeft == other.TopLeft && Size == other.Size;
		}

		public override int GetHashCode()
		{
			//// ReSharper disable NonReadonlyFieldInGetHashCode
			return Left.GetHashCode() ^ Top.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
		}

		public override string ToString()
		{
			return Left.ToInvariantString() + " " + Top.ToInvariantString() + " " +
				Width.ToInvariantString() + " " + Height.ToInvariantString();
		}

		public Point[] GetRotatedRectangleCorners(Point center, float rotation)
		{
			return new[]
			{
				TopLeft.RotateAround(center, rotation), TopRight.RotateAround(center, rotation),
				BottomRight.RotateAround(center, rotation), BottomLeft.RotateAround(center, rotation) 
			};
		}

		public bool IsColliding(float rotation, Rectangle otherRect, float otherRotation)
		{
			var rotatedRect = GetRotatedRectangleCorners(Center, rotation);
			var rotatedOtherRect = otherRect.GetRotatedRectangleCorners(otherRect.Center, otherRotation);
			foreach (var axis in GetAxes(otherRect))
				if (IsProjectedAxisOutsideRectangles(axis, rotatedRect, rotatedOtherRect))
					return false;
			return true;
		}

		private IEnumerable<Point> GetAxes(Rectangle rectangle)
		{
			return new[]
			{
				new Point(TopRight.X - TopLeft.X, TopRight.Y - TopLeft.Y),
				new Point(TopRight.X - BottomRight.X, TopRight.Y - BottomRight.Y),
				new Point(rectangle.TopLeft.X - rectangle.BottomLeft.X,
					rectangle.TopLeft.Y - rectangle.BottomLeft.Y),
				new Point(rectangle.TopLeft.X - rectangle.TopRight.X,
					rectangle.TopLeft.Y - rectangle.TopRight.Y)
			};
		}

		private static bool IsProjectedAxisOutsideRectangles(Point axis, Point[] rotatedRect,
			Point[] rotatedOtherRect)
		{
			var rectMin = float.MaxValue;
			var rectMax = float.MinValue;
			var otherMin = float.MaxValue;
			var otherMax = float.MinValue;
			GetRectangleProjectionResult(axis, rotatedRect, ref rectMin, ref rectMax);
			GetRectangleProjectionResult(axis, rotatedOtherRect, ref otherMin, ref otherMax);
			return rectMin > otherMax || rectMax < otherMin;
		}

		private static void GetRectangleProjectionResult(Point axis, IEnumerable<Point> cornerList,
			ref float min, ref float max)
		{
			foreach (var corner in cornerList)
			{
				float projectedValue = corner.DistanceFromProjectAxisPoint(axis) * axis.X + axis.Y;
				if (projectedValue < min)
					min = projectedValue;
				if (projectedValue > max)
					max = projectedValue;
			}
		}
	}
}