using System;
using System.Diagnostics.Contracts;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Useful for processing 3D rotations. Riemer's XNA tutorials have a nice introductory 
	/// example of use: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Quaternions.php
	/// http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Flight_kinematics.php
	/// </summary>
	public struct Quaternion : IEquatable<Quaternion>, Lerp<Quaternion>
	{
		public Quaternion(float x, float y, float z, float w)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }

		public static Quaternion CreateFromAxisAngle(Vector axis, float angle)
		{
			var vectorPart = MathExtensions.Sin(angle * 0.5f) * axis;
			return new Quaternion(vectorPart.X, vectorPart.Y, vectorPart.Z,
				MathExtensions.Cos(angle * 0.5f));
		}

		[Pure]
		public Quaternion Lerp(Quaternion other, float interpolation)
		{
			return new Quaternion(
				MathExtensions.Lerp(X, other.X, interpolation),
				MathExtensions.Lerp(Y, other.Y, interpolation),
				MathExtensions.Lerp(Z, other.Z, interpolation),
				MathExtensions.Lerp(W, other.W, interpolation));
		}

		public bool Equals(Quaternion other)
		{
			return X.IsNearlyEqual(other.X) &&
				Y.IsNearlyEqual(other.Y) &&
				Z.IsNearlyEqual(other.Z) &&
				W.IsNearlyEqual(other.W);
		}

		public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);
	}
}