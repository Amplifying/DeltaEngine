using System;
using System.Diagnostics;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Color with a byte or float per component (red, green, blue, alpha)
	/// </summary>
	[DebuggerDisplay("Color(R={R}, G={G}, B={B}, A={A})")]
	public struct Color : IEquatable<Color>
	{
		public Color(byte r, byte g, byte b, byte a = 255)
			: this()
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public byte R { get; set; }
		public byte G { get; set; }
		public byte B { get; set; }
		public byte A { get; set; }

		public Color(float r, float g, float b, float a = 1.0f)
			: this()
		{
			R = (byte)(r * 255);
			G = (byte)(g * 255);
			B = (byte)(b * 255);
			A = (byte)(a * 255);
		}

		public static readonly Color Black = new Color(0, 0, 0);
		public static readonly Color Blue = new Color(0, 0, 255);
		public static readonly Color CornflowerBlue = new Color(100, 149, 237);
		public static readonly Color Cyan = new Color(0, 255, 255);
		public static readonly Color Gray = new Color(128, 128, 128);
		public static readonly Color Green = new Color(0, 255, 0);
		public static readonly Color PaleGreen = new Color(152, 251, 152);
		public static readonly Color Pink = new Color(255, 192, 203);
		public static readonly Color Purple = new Color(255, 0, 255);
		public static readonly Color Red = new Color(255, 0, 0);
		public static readonly Color White = new Color(255, 255, 255);
		public static readonly Color Yellow = new Color(255, 255, 0);

		public int PackedArgb
		{
			get
			{
				return (A << 24) + (R << 16) + (G << 8) + B;
			}
		}

		public static bool operator !=(Color c1, Color c2)
		{
			return c1.Equals(c2) == false;
		}

		public static bool operator ==(Color c1, Color c2)
		{
			return c1.Equals(c2);
		}


		public bool Equals(Color other)
		{
			return PackedArgb == other.PackedArgb;
		}

		public override bool Equals(object other)
		{
			return other is Color ? Equals((Color)other) : base.Equals(other);
		}

		public override int GetHashCode()
		{
			return PackedArgb;
		}

		public static Color Lerp(Color color1, Color color2, float percentage)
		{
			var r = (byte)(MathExtensions.Lerp(color1.R, color2.R, percentage));
			var g = (byte)(MathExtensions.Lerp(color1.G, color2.G, percentage));
			var b = (byte)(MathExtensions.Lerp(color1.B, color2.B, percentage));
			var a = (byte)(MathExtensions.Lerp(color1.A, color2.A, percentage));

			return new Color(r, g, b, a);
		}

		public static Color GetRandomBrightColor()
		{
			var r = (byte)PseudoRandom.Get(128, 256);
			var g = (byte)PseudoRandom.Get(128, 256);
			var b = (byte)PseudoRandom.Get(128, 256);
			return new Color(r, g, b);
		}
	}
}