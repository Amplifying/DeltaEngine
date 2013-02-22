using System;
using System.Runtime.InteropServices;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	[StructLayout(LayoutKind.Explicit)]
	public struct Matrix : IEquatable<Matrix>
	{
		public static readonly Matrix Zero = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0);

		public static readonly Matrix Identity = new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0,
			0, 1);

		[FieldOffset(0)]
		public float M11;
		[FieldOffset(4)]
		public float M12;
		[FieldOffset(8)]
		public float M13;
		[FieldOffset(12)]
		public float M14;
		[FieldOffset(16)]
		public float M21;
		[FieldOffset(20)]
		public float M22;
		[FieldOffset(24)]
		public float M23;
		[FieldOffset(28)]
		public float M24;
		[FieldOffset(32)]
		public float M31;
		[FieldOffset(36)]
		public float M32;
		[FieldOffset(40)]
		public float M33;
		[FieldOffset(44)]
		public float M34;
		[FieldOffset(48)]
		public float M41;
		[FieldOffset(52)]
		public float M42;
		[FieldOffset(56)]
		public float M43;
		[FieldOffset(60)]
		public float M44;

		[FieldOffset(48)]
		public Vector Translation;

		public Matrix(float m11, float m12, float m13, float m14, float m21, float m22, float m23,
			float m24, float m31, float m32, float m33, float m34, float m41, float m42, float m43,
			float m44)
			: this()
		{
			M11 = m11;
			M12 = m12;
			M13 = m13;
			M14 = m14;
			M21 = m21;
			M22 = m22;
			M23 = m23;
			M24 = m24;
			M31 = m31;
			M32 = m32;
			M33 = m33;
			M34 = m34;
			M41 = m41;
			M42 = m42;
			M43 = m43;
			M44 = m44;
		}

		public Matrix(float[] setMatrix)
			: this()
		{
			M11 = setMatrix[0];
			M12 = setMatrix[1];
			M13 = setMatrix[2];
			M14 = setMatrix[3];
			M21 = setMatrix[4];
			M22 = setMatrix[5];
			M23 = setMatrix[6];
			M24 = setMatrix[7];
			M31 = setMatrix[8];
			M32 = setMatrix[9];
			M33 = setMatrix[10];
			M34 = setMatrix[11];
			M41 = setMatrix[12];
			M42 = setMatrix[13];
			M43 = setMatrix[14];
			M44 = setMatrix[15];
		}

		public Matrix(Matrix setMatrix)
			: this()
		{
			M11 = setMatrix.M11;
			M12 = setMatrix.M12;
			M13 = setMatrix.M13;
			M14 = setMatrix.M14;
			M21 = setMatrix.M21;
			M22 = setMatrix.M22;
			M23 = setMatrix.M23;
			M24 = setMatrix.M24;
			M31 = setMatrix.M31;
			M32 = setMatrix.M32;
			M33 = setMatrix.M33;
			M34 = setMatrix.M34;
			M41 = setMatrix.M41;
			M42 = setMatrix.M42;
			M43 = setMatrix.M43;
			M44 = setMatrix.M44;
		}

		public float[] Values
		{
			get
			{
				return new[]
				{ M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
			}
		}

		public static Matrix CreateScale(float scale)
		{
			return new Matrix(scale, 0f, 0f, 0f, 0f, scale, 0f, 0f, 0f, 0f, scale, 0f, 0f, 0f, 0f, 1.0f);
		}

		public static Matrix CreateScale(Vector scale)
		{
			return CreateScale(scale.X, scale.Y, scale.Z);
		}

		public static Matrix CreateScale(float scaleX, float scaleY, float scaleZ)
		{
			return new Matrix(scaleX, 0f, 0f, 0f, 0f, scaleY, 0f, 0f, 0f, 0f, scaleZ, 0f, 0f, 0f, 0f, 1f);
		}

		public static Matrix CreateRotationX(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(1.0f, 0.0f, 0.0f, 0.0f,
												0.0f, cosValue, sinValue, 0.0f,
												0.0f, -sinValue, cosValue, 0.0f,
												0.0f, 0.0f, 0.0f, 1.0f);
		}

		public static Matrix CreateRotationY(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(cosValue, 0.0f, -sinValue, 0.0f,
												0.0f, 1.0f, 0.0f, 0.0f,
												sinValue, 0.0f, cosValue, 0.0f,
												0.0f, 0.0f, 0.0f, 1.0f);
		}

		public static Matrix CreateRotationZ(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(cosValue, sinValue, 0.0f, 0.0f,
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
				(sx * sy * cz) + (cx * -sz), (sx * sy * sz) + (cx * cz), sx * cy, 0.0f, 
				(cx * sy * cz) + (sx * sz), (cx * sy * sz) + (-sx * cz), cx * cy, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f);
		}
		
		public static Matrix CreateTranslation(Vector position)
		{
			return new Matrix(1f, 0f, 0f, 0f,
											  0f, 1f, 0f, 0f,
                        0f, 0f, 1f, 0f,
												position.X, position.Y, position.Z, 1.0f);
		}
		public static Matrix CreateTranslation(float x, float y, float z)
		{
			return new Matrix(1f, 0f, 0f, 0f,
												0f, 1f, 0f, 0f,
												0f, 0f, 1f, 0f,
												x, y, z, 1.0f);
		}

		public static Matrix Transpose(Matrix matrix)
		{
			return new Matrix(
				matrix.M11, matrix.M21, matrix.M31, matrix.M41,
				matrix.M12, matrix.M22, matrix.M32, matrix.M42,
				matrix.M13, matrix.M23, matrix.M33, matrix.M43,
				matrix.M14, matrix.M24, matrix.M34, matrix.M44);
		}

		public float Determinant
		{
			get
			{
				float m44X33M43X34 = (M44 * M33) - (M43 * M34);
				float m32X44M42X34 = (M32 * M44) - (M42 * M34);
				float m32X43M42X33 = (M32 * M43) - (M42 * M33);
				float m31X44M41X34 = (M31 * M44) - (M41 * M34);
				float m31X43M41X33 = (M31 * M43) - (M41 * M33);
				float m31X42M41X32 = (M31 * M42) - (M41 * M32);
				return (((((((M22 * m44X33M43X34) - (M23 * m32X44M42X34)) + (M24 * m32X43M42X33)) * M11) -
					((((M21 * m44X33M43X34) - (M23 * m31X44M41X34)) + (M24 * m31X43M41X33)) * M12)) +
					((((M21 * m32X44M42X34) - (M22 * m31X44M41X34)) + (M24 * m31X42M41X32)) * M13)) -
					((((M21 * m32X43M42X33) - (M22 * m31X43M41X33)) + (M23 * m31X42M41X32)) * M14));
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
			float[] values = Values;
			float[] valuesMatrix = matrix.Values;
			for (int i = 0; i < 16; i++)
				if (!values[i].IsNearlyEqual(valuesMatrix[i]))
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
		public Vector Right;

		[FieldOffset(16)]
		public Vector Up;

		[FieldOffset(32)]
		public Vector Front;

		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			return new Matrix(
				matrix1.M11 + matrix2.M11, matrix1.M12 + matrix2.M12,
				matrix1.M13 + matrix2.M13, matrix1.M14 + matrix2.M14,
				matrix1.M21 + matrix2.M21, matrix1.M22 + matrix2.M22,
				matrix1.M23 + matrix2.M23, matrix1.M24 + matrix2.M24,
				matrix1.M31 + matrix2.M31, matrix1.M32 + matrix2.M32,
				matrix1.M33 + matrix2.M33, matrix1.M34 + matrix2.M34,
				matrix1.M41 + matrix2.M41, matrix1.M42 + matrix2.M42,
				matrix1.M43 + matrix2.M43, matrix1.M44 + matrix2.M44);
		}

		public static Matrix operator -(Matrix matrix1, Matrix matrix2)
		{
			return new Matrix(
				matrix1.M11 - matrix2.M11, matrix1.M12 - matrix2.M12,
				matrix1.M13 - matrix2.M13, matrix1.M14 - matrix2.M14,
				matrix1.M21 - matrix2.M21, matrix1.M22 - matrix2.M22,
				matrix1.M23 - matrix2.M23, matrix1.M24 - matrix2.M24,
				matrix1.M31 - matrix2.M31, matrix1.M32 - matrix2.M32,
				matrix1.M33 - matrix2.M33, matrix1.M34 - matrix2.M34,
				matrix1.M41 - matrix2.M41, matrix1.M42 - matrix2.M42,
				matrix1.M43 - matrix2.M43, matrix1.M44 - matrix2.M44);
		}

		public static Vector operator *(Matrix matrix, Vector vector)
		{
			return
				new Vector(
					(vector.X * matrix.M11) + (vector.Y * matrix.M21) + (vector.Z * matrix.M31) + matrix.M41,
					(vector.X * matrix.M12) + (vector.Y * matrix.M22) + (vector.Z * matrix.M32) + matrix.M42,
					(vector.X * matrix.M13) + (vector.Y * matrix.M23) + (vector.Z * matrix.M33) + matrix.M43);
		}

		public static Matrix operator *(float scaleFactor, Matrix matrix)
		{
			return new Matrix(matrix * scaleFactor);
		}

		public static Matrix operator *(Matrix matrix, float scaleFactor)
		{
			return new Matrix(
				matrix.M11 * scaleFactor, matrix.M12 * scaleFactor,
				matrix.M13 * scaleFactor, matrix.M14 * scaleFactor,
				matrix.M21 * scaleFactor, matrix.M22 * scaleFactor,
				matrix.M23 * scaleFactor, matrix.M24 * scaleFactor,
				matrix.M31 * scaleFactor, matrix.M32 * scaleFactor,
				matrix.M33 * scaleFactor, matrix.M34 * scaleFactor,
				matrix.M41 * scaleFactor, matrix.M42 * scaleFactor,
				matrix.M43 * scaleFactor, matrix.M44 * scaleFactor);
		}

		public static Matrix operator /(Matrix matrix1, Matrix matrix2)
		{
			return new Matrix(
				matrix1.M11 / matrix2.M11, matrix1.M12 / matrix2.M12,
				matrix1.M13 / matrix2.M13, matrix1.M14 / matrix2.M14,
				matrix1.M21 / matrix2.M21, matrix1.M22 / matrix2.M22,
				matrix1.M23 / matrix2.M23, matrix1.M24 / matrix2.M24,
				matrix1.M31 / matrix2.M31, matrix1.M32 / matrix2.M32,
				matrix1.M33 / matrix2.M33, matrix1.M34 / matrix2.M34,
				matrix1.M41 / matrix2.M41, matrix1.M42 / matrix2.M42,
				matrix1.M43 / matrix2.M43, matrix1.M44 / matrix2.M44);
		}

		public static Matrix operator /(Matrix matrix1, float divider)
		{
			float num = 1f / divider;
			return new Matrix(
				matrix1.M11 * num, matrix1.M12 * num, matrix1.M13 * num, matrix1.M14 * num,
				matrix1.M21 * num, matrix1.M22 * num, matrix1.M23 * num, matrix1.M24 * num,
				matrix1.M31 * num, matrix1.M32 * num, matrix1.M33 * num, matrix1.M34 * num,
				matrix1.M41 * num, matrix1.M42 * num, matrix1.M43 * num, matrix1.M44 * num);
		}

		public static Matrix operator -(Matrix matrix1)
		{
			return new Matrix(
				-matrix1.M11, -matrix1.M12, -matrix1.M13, -matrix1.M14,
				-matrix1.M21, -matrix1.M22, -matrix1.M23, -matrix1.M24,
				-matrix1.M31, -matrix1.M32, -matrix1.M33, -matrix1.M34,
				-matrix1.M41, -matrix1.M42, -matrix1.M43, -matrix1.M44);
		}

		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			float[] values1 = matrix1.Values;
			float[] values2 = matrix2.Values;
			float[] result = new float[16];

			for (int row = 0; row < 4; row++)
			{
				for (int column = 0; column < 4; column++)
				{
					result[row * 4 + column] = values1[row * 4] * values2[column] + 
						values1[row * 4 + 1] * values2[column + 4] +
						values1[row * 4 + 2] * values2[column + 8] +
						values1[row * 4 + 3] * values2[column + 12];
				}
				
			}

			return new Matrix(result);
		}

		public Matrix Inverse
		{
			get
			{
				float m33X44M34X43 = (M33 * M44) - (M34 * M43);
				float m32X44M34X42 = (M32 * M44) - (M34 * M42);
				float m32X43M33X42 = (M32 * M43) - (M33 * M42);
				float m31X44M34X41 = (M31 * M44) - (M34 * M41);
				float m31X43M33X41 = (M31 * M43) - (M33 * M41);
				float m31X42M32X41 = (M31 * M42) - (M32 * M41);
				float m22M23M24 = ((M22 * m33X44M34X43) - (M23 * m32X44M34X42)) + (M24 * m32X43M33X42);
				float m21M23M24 = -(((M21 * m33X44M34X43) - (M23 * m31X44M34X41)) + (M24 * m31X43M33X41));
				float m21M22M24 = ((M21 * m32X44M34X42) - (M22 * m31X44M34X41)) + (M24 * m31X42M32X41);
				float m21M22M23 = -(((M21 * m32X43M33X42) - (M22 * m31X43M33X41)) + (M23 * m31X42M32X41));

				float inverseDet = 1f /
					((((M11 * m22M23M24) + (M12 * m21M23M24)) + (M13 * m21M22M24)) + (M14 * m21M22M23));
				float m23X44M24X43 = (M23 * M44) - (M24 * M43);
				float m22X44M24X42 = (M22 * M44) - (M24 * M42);
				float m22X43M23X42 = (M22 * M43) - (M23 * M42);
				float m21X44M24X41 = (M21 * M44) - (M24 * M41);
				float m21X43M23X41 = (M21 * M43) - (M23 * M41);
				float m21X42M22X41 = (M21 * M42) - (M22 * M41);
				float m23X34M24X33 = (M23 * M34) - (M24 * M33);
				float m22X34M24X32 = (M22 * M34) - (M24 * M32);
				float m22X33M23X32 = (M22 * M33) - (M23 * M32);
				float m21X34M23X31 = (M21 * M34) - (M24 * M31);
				float m21X33M23X31 = (M21 * M33) - (M23 * M31);
				float m21X32M22X31 = (M21 * M32) - (M22 * M31);
				return new Matrix(m22M23M24 * inverseDet,
					-(((M12 * m33X44M34X43) - (M13 * m32X44M34X42)) + (M14 * m32X43M33X42)) * inverseDet,
					(((M12 * m23X44M24X43) - (M13 * m22X44M24X42)) + (M14 * m22X43M23X42)) * inverseDet,
					-(((M12 * m23X34M24X33) - (M13 * m22X34M24X32)) + (M14 * m22X33M23X32)) * inverseDet,
					m21M23M24 * inverseDet,
					(((M11 * m33X44M34X43) - (M13 * m31X44M34X41)) + (M14 * m31X43M33X41)) * inverseDet,
					-(((M11 * m23X44M24X43) - (M13 * m21X44M24X41)) + (M14 * m21X43M23X41)) * inverseDet,
					(((M11 * m23X34M24X33) - (M13 * m21X34M23X31)) + (M14 * m21X33M23X31)) * inverseDet,
					m21M22M24 * inverseDet,
					-(((M11 * m32X44M34X42) - (M12 * m31X44M34X41)) + (M14 * m31X42M32X41)) * inverseDet,
					(((M11 * m22X44M24X42) - (M12 * m21X44M24X41)) + (M14 * m21X42M22X41)) * inverseDet,
					-(((M11 * m22X34M24X32) - (M12 * m21X34M23X31)) + (M14 * m21X32M22X31)) * inverseDet,
					m21M22M23 * inverseDet,
					(((M11 * m32X43M33X42) - (M12 * m31X43M33X41)) + (M13 * m31X42M32X41)) * inverseDet,
					-(((M11 * m22X43M23X42) - (M12 * m21X43M23X41)) + (M13 * m21X42M22X41)) * inverseDet,
					(((M11 * m22X33M23X32) - (M12 * m21X33M23X31)) + (M13 * m21X32M22X31)) * inverseDet);
			}
		}

		public override int GetHashCode()
		{
			return 
				M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() +
				M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() + M24.GetHashCode() +
				M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode() + M34.GetHashCode() +
				M41.GetHashCode() + M42.GetHashCode() + M43.GetHashCode() + M44.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is Matrix && Equals((Matrix)obj);
		}

		public bool Equals(Matrix other)
		{
			return (
				M11 == other.M11 && M22 == other.M22 && M33 == other.M33 && M44 == other.M44 &&
				M12 == other.M12 && M13 == other.M13 && M14 == other.M14 && M21 == other.M21 &&
				M23 == other.M23 && M24 == other.M24 && M31 == other.M31 && M32 == other.M32 &&
				M34 == other.M34 && M41 == other.M41 && M42 == other.M42 && M43 == other.M43);
		}

		public bool Equals(float[] other)
		{
			float[] values = Values;
			if (other.Length != 16)
				return false;
			for (int i = 0; i < 16; i++)
			{
				if (values[i] != other[i])
					return false;
			}
			return true;
		}

		public override string ToString()
		{
			return string.Join(", ", Values);
		}
	}
}