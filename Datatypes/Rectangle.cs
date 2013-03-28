using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Holds data for a rectangle by specifying its top left corner and the width and height.
	/// </summary>
	[StructLayout(LayoutKind.Explicit),
	 DebuggerDisplay("Rectangle(Left={Left}, Top={Top}, Width={Width}, Height={Height})")]
	public struct Rectangle
	{
		public Rectangle(float left, float top, float width, float height)
			: this()
		{
			Left = left;
			Top = top;
			Width = width;
			Height = height;
		}

		public Rectangle(Point position, Size size)
			: this(position.X, position.Y, size.Width, size.Height) { }

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

		[FieldOffset(0)]
		public float Left;
		[FieldOffset(4)]
		public float Top;
		[FieldOffset(8)]
		public float Width;
		[FieldOffset(12)]
		public float Height;
		[FieldOffset(0)]
		public Point TopLeft;
		[FieldOffset(8)]
		public Size Size;

		public class InvalidNumberOfComponents : Exception { }

		public static readonly Rectangle Zero;
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

		public Point TopRight
		{
			get { return new Point(Left + Width, Top); }
			set
			{
				Left = value.X - Width;
				Top = value.Y;
			}
		}

		public Point BottomLeft
		{
			get { return new Point(Left, Top + Height); }
			set
			{
				Left = value.X;
				Top = value.Y - Height;
			}
		}

		public Point BottomRight
		{
			get { return new Point(Left + Width, Top + Height); }
			set
			{
				Left = value.X - Width;
				Top = value.Y - Height;
			}
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

		public static Rectangle FromCenter(float x, float y, float width, float height)
		{
			return FromCenter(new Point(x, y), new Size(width, height));
		}

		public static Rectangle FromCenter(Point center, Size size)
		{
			return new Rectangle(new Point(center.X - size.Width / 2, center.Y - size.Height / 2), size);
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
			// ReSharper disable NonReadonlyFieldInGetHashCode
			return Left.GetHashCode() ^ Top.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
		}

		public bool Contains(Point position)
		{
			return position.X >= Left && position.X < Right && position.Y >= Top && position.Y < Bottom;
		}

		public override string ToString()
		{
			return Left.ToInvariantString() + " " + Top.ToInvariantString() + " " +
				Width.ToInvariantString() + " " + Height.ToInvariantString();
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
	}
}