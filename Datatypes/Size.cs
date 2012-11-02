using System;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Struct used to hold the width and height of an object (eg. a rectangle)
	/// </summary>
	public struct Size
	{
		public Size(float width, float height)
			: this()
		{
			Width = width;
			Height = height;
		}

		public Size(string sizeAsString)
			: this()
		{
			float[] floats = sizeAsString.SplitIntoFloats();
			if (floats.Length != 2)
				throw new InvalidNumberOfComponents();

			Width = floats[0];
			Height = floats[1];
		}

		public float Width { get; set; }
		public float Height { get; set; }

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Size Zero = new Size();
		public static readonly Size One = new Size(1, 1);
		public static readonly Size Half = new Size(0.5f, 0.5f);

		public static bool operator ==(Size s1, Size s2)
		{
			return s1.Equals(s2);
		}

		public static bool operator !=(Size s1, Size s2)
		{
			return s1.Equals(s2) == false;
		}

		public static Size operator *(Size s1, Size s2)
		{
			return new Size(s1.Width * s2.Width, s1.Height * s2.Height);
		}

		public static Size operator *(Size s, float f)
		{
			return new Size(s.Width * f, s.Height * f);
		}

		public static Size operator *(float f, Size s)
		{
			return new Size(f * s.Width, f * s.Height);
		}

		public static Size operator /(Size s, float f)
		{
			return new Size(s.Width / f, s.Height / f);
		}

		public static Size operator /(float f, Size s)
		{
			return new Size(f / s.Width, f / s.Height);
		}

		public static Size operator +(Size s1, Size s2)
		{
			return new Size(s1.Width + s2.Width, s1.Height + s2.Height);
		}

		public static Size operator -(Size s1, Size s2)
		{
			return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
		}

		private bool Equals(Size other)
		{
			return Width.IsNearlyEqual(other.Width) && Height.IsNearlyEqual(other.Height);
		}

		public override bool Equals(object other)
		{
			return other is Size ? Equals((Size)other) : base.Equals(other);
		}

		public static explicit operator Size(Point p)
		{
			return new Size(p.X, p.Y);
		}

		public override int GetHashCode()
		{
			return Width.GetHashCode() ^ Height.GetHashCode();
		}

		public override string ToString()
		{
			return "(" + Width.ToInvariantString() + ", " + Height.ToInvariantString() + ")";
		}

		public static Size Lerp(Size size1, Size size2, float percentage)
		{
			float width = MathExtensions.Lerp(size1.Width, size2.Width, percentage);
			float height = MathExtensions.Lerp(size1.Height, size2.Height, percentage);

			return new Size(width, height);
		}

		public float Aspect
		{
			get { return Width / Height; }
		}
	}
}