using System;
using System.Runtime.InteropServices;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Matrix : IEquatable<Matrix>
	{
		public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23,
			float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43,
			float m44)
			: this()
		{
			this[0] = m11;
			this[1] = m12;
			this[2] = m13;
			this[3] = m14;
			this[4] = m21;
			this[5] = m22;
			this[6] = m23;
			this[7] = m24;
			this[8] = m31;
			this[9] = m32;
			this[10] = m33;
			this[11] = m34;
			this[12] = m41;
			this[13] = m42;
			this[14] = m43;
			this[15] = m44;
		}

		public Matrix(float[] setMatrix)
			: this()
		{
			for (int i = 0; i < 16; i++)
			{
				this[i] = setMatrix[i];
			} 
		}

		public Matrix(Matrix setMatrix)
			: this()
		{
			for (int i = 0; i < 16; i++)
			{
				this[i] = setMatrix[i];
			}
		}

		public static readonly Matrix Identity = 
			new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

		[FieldOffset(0)]
		private float M11;
		[FieldOffset(4)]
		private float M12;
		[FieldOffset(8)]
		private float M13;
		[FieldOffset(12)]
		private float M14;
		[FieldOffset(16)]
		private float M21;
		[FieldOffset(20)]
		private float M22;
		[FieldOffset(24)]
		private float M23;
		[FieldOffset(28)]
		private float M24;
		[FieldOffset(32)]
		private float M31;
		[FieldOffset(36)]
		private float M32;
		[FieldOffset(40)]
		private float M33;
		[FieldOffset(44)]
		private float M34;
		[FieldOffset(48)]
		private float M41;
		[FieldOffset(52)]
		private float M42;
		[FieldOffset(56)]
		private float M43;
		[FieldOffset(60)]
		private float M44;

		public float this[int index]
		{
			get
			{
				if (index > 0 && index < 16)
					return Values[index];
				return Values[0];
			}
			set {
				SetValue(index, value);
			}
		}

		private void SetValue(int index, float value)
		{
			if (index == 0) M11 = value;
			else if (index == 1) M12 = value;
			else if (index == 2) M13 = value;
			else if (index == 3) M14 = value;
			else if (index == 4) M21 = value;
			else if (index == 5) M22 = value;
			else if (index == 6) M23 = value;
			else if (index == 7) M24 = value;
			else if (index == 8) M31 = value;
			else if (index == 9) M32 = value;
			else if (index == 10)M33 = value;
			else if (index == 11) M34 = value;
			else if (index == 12) M41 = value;
			else if (index == 13) M42 = value;
			else if (index == 14) M43 = value;
			else if (index == 15) M44 = value;
		}

		public float[] Values
		{
			get
			{
				return new[]
				{ M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
			}
		}

		[FieldOffset(48)]
		private readonly Vector Translation;

		public static Matrix CreateScale(float scale)
		{
			return CreateScale(scale, scale, scale);
		}

		public static Matrix CreateScale(float scaleX, float scaleY, float scaleZ)
		{
			return new Matrix(
				scaleX, 0, 0, 0,
				0, scaleY, 0, 0,
				0, 0, scaleZ, 0,
				0, 0, 0, 1);
		}

		public static Matrix CreateScale(Vector scale)
		{
			return CreateScale(scale.X, scale.Y, scale.Z);
		}

		public static Matrix CreateRotationX(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(
				1.0f, 0.0f, 0.0f, 0.0f,
				0.0f, cosValue, sinValue, 0.0f,
				0.0f, -sinValue, cosValue, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f);
		}

		public static Matrix CreateRotationY(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(
				cosValue, 0.0f, -sinValue, 0.0f,
				0.0f, 1.0f, 0.0f, 0.0f,
				sinValue, 0.0f, cosValue, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f);
		}

		public static Matrix CreateRotationZ(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(
				cosValue, sinValue, 0.0f, 0.0f,
				-sinValue, cosValue, 0.0f, 0.0f,
				0.0f, 0.0f, 1f, 0f,
				0.0f, 0.0f, 0.0f, 1.0f);
		}

		public static Matrix CreateRotationZYX(float x, float y, float z)
		{
			var cx = MathExtensions.Cos(x);
			var sx = MathExtensions.Sin(x);
			var cy = MathExtensions.Cos(y);
			var sy = MathExtensions.Sin(y);
			var cz = MathExtensions.Cos(z);
			var sz = MathExtensions.Sin(z);
			return new Matrix(
				cy * cz, cy * sz, -sy, 0.0f, 
				sx * sy * cz + cx * -sz, sx * sy * sz + cx * cz, sx * cy, 0.0f, 
				cx * sy * cz + sx * sz, cx * sy * sz + -sx * cz, cx * cy, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f);
		}
		
		public static Matrix CreateTranslation(Vector position)
		{
			return new Matrix(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				position.X, position.Y, position.Z, 1);
		}
		public static Matrix CreateTranslation(float x, float y, float z)
		{
			return new Matrix(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				x, y, z, 1);
		}

		public static Matrix Transpose(Matrix matrix)
		{
			return new Matrix(
				matrix[0], matrix[4], matrix[8], matrix[12],
				matrix[1], matrix[5], matrix[9], matrix[13],
				matrix[2], matrix[6], matrix[10], matrix[14],
				matrix[3], matrix[7], matrix[11], matrix[15]);
		}

		public static Matrix GenerateOrthographicProjection(Size size)
		{
			return new Matrix(
				2.0f / size.Width, 0, 0, 0,
				0, 2.0f / -size.Height, 0, 0,
				0, 0, -1, 0,
				-1, 1, 0, 1);
		}

		public float Determinant
		{
			get
			{
				float m44X33M43X34 = (this[15] * this[10]) - (this[14] * this[11]);
				float m32X44M42X34 = (this[9] * this[15]) - (this[13] * this[11]);
				float m32X43M42X33 = (this[9] * this[14]) - (this[13] * this[10]);
				float m31X44M41X34 = (this[8] * this[15]) - (this[12] * this[11]);
				float m31X43M41X33 = (this[8] * this[14]) - (this[12] * this[10]);
				float m31X42M41X32 = (this[8] * this[13]) - (this[12] * this[9]);
				return (((((((this[5] * m44X33M43X34) - (this[6] * m32X44M42X34)) +
					(this[7] * m32X43M42X33)) * this[0]) -
					((((this[4] * m44X33M43X34) - (this[6] * m31X44M41X34)) +
					(this[7] * m31X43M41X33)) * this[1])) +
					((((this[4] * m32X44M42X34) - (this[5] * m31X44M41X34)) +
					(this[7] * m31X42M41X32)) * this[2])) -
					((((this[4] * m32X43M42X33) - (this[5] * m31X43M41X33)) +
					(this[6] * m31X42M41X32)) * this[3]));
			}
		}

		public static bool operator ==(Matrix matrix1, Matrix matrix2)
		{
			return matrix1.Equals(matrix2);
		}

		public static bool operator !=(Matrix matrix1, Matrix matrix2)
		{
			return !(matrix1 == matrix2);
		}

		public bool IsNearlyEqual(Matrix matrix)
		{
			for (int i = 0; i < 16; i++)
				if (!this[i].IsNearlyEqual(matrix[i]))
					return false;

			return true;
		}

		public Vector GetRow(int index)
		{
			switch (index)
			{
			case 0:
				return Right;
			case 1:
				return Up;
			case 2:
				return Front;
			case 3:
				return Translation;
			default:
				throw new IndexOutOfRangeException();
			}
		}

		[FieldOffset(0)]
		private readonly Vector Right;
		[FieldOffset(16)]
		private readonly Vector Up;
		[FieldOffset(32)]
		private readonly Vector Front;

		public static Vector operator *(Matrix matrix, Vector vector)
		{
			return
				new Vector(
					(vector.X * matrix[0]) + (vector.Y * matrix[4]) +
					(vector.Z * matrix[8]) + matrix[12],
					(vector.X * matrix[1]) + (vector.Y * matrix[5]) +
					(vector.Z * matrix[9]) + matrix[13],
					(vector.X * matrix[2]) + (vector.Y * matrix[6]) +
					(vector.Z * matrix[10]) + matrix[14]);
		}

		public static Matrix operator -(Matrix matrix1)
		{
			var result = new float[16];
			for (int i = 0; i < 16; i++)
				result[i] = -1.0f * matrix1[i];

			return new Matrix(result);
		}

		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			var result = new float[16];
			for (int row = 0; row < 4; row++)
				for (int column = 0; column < 4; column++)
					result[row * 4 + column] = matrix1[row * 4] * matrix2[column] +
						matrix1[row * 4 + 1] * matrix2[column + 4] +
						matrix1[row * 4 + 2] * matrix2[column + 8] +
						matrix1[row * 4 + 3] * matrix2[column + 12];

			return new Matrix(result);
		}

		public override int GetHashCode()
		{
			int hashCode = 0;
			for (int i = 0; i < 16; i++)
				hashCode += this[i].GetHashCode();

			return hashCode;
		}

		public override bool Equals(object obj)
		{
			return obj is Matrix && Equals((Matrix)obj);
		}

		public bool Equals(Matrix other)
		{
			for (int i = 0; i < 16; i++)
				if (!this[i].IsNearlyEqual(other[i]))
					return false;

			return true;
		}

		public bool Equals(float[] other)
		{
			if (other.Length != 16)
				throw new IndexOutOfRangeException();
			for (int i = 0; i < 16; i++)
				if (this[i] != other[i])
					return false;

			return true;
		}

		public override string ToString()
		{
			return string.Join(", ", Values);
		}
	}
}