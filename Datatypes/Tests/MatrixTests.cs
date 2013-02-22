using System;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class MatrixTests
	{
		[Test]
		public void MatrixZero()
		{
			var matrix = Matrix.Zero;
			Console.WriteLine(matrix);
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(0, matrix.Values[i]);
		}

		[Test]
		public void CreateWith16Floats()
		{
			var matrix = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			AssertValues0To15(matrix);
		}

		[Test]
		public void CreateWithFloatArray()
		{
			float[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			var matrix = new Matrix(values);
			AssertValues0To15(matrix);
		}

		[Test]
		public void CreateFromAnotherMatrix()
		{
			var matrix = new Matrix(Matrix.Identity);
			matrix.M11 = 3;
			Assert.AreEqual(3, matrix.Values[0]);
		}

		private static void AssertValues0To15(Matrix matrix)
		{
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(i, matrix.Values[i]);
		}

		[Test]
		public void MatrixIdentityShouldNotBeModified()
		{
			Matrix matrix = Matrix.Identity;
			matrix.M11 = 10;
			Assert.AreEqual(1, Matrix.Identity.M11);
		}

		[Test]
		public void CreateScaleFromOneScalar()
		{
			var matrix = Matrix.CreateScale(5);
			Assert.AreEqual(5, matrix.M11);
			Assert.AreEqual(5, matrix.M22);
			Assert.AreEqual(5, matrix.M33);
			Assert.AreEqual(1, matrix.M44);
		}

		[Test]
		public void CreateScaleFromVector()
		{
			var matrix = Matrix.CreateScale(Vector.One);
			Assert.AreEqual(1, matrix.M11);
			Assert.AreEqual(1, matrix.M22);
			Assert.AreEqual(1, matrix.M33);
			Assert.AreEqual(1, matrix.M44);
		}

		[Test]
		public void CreateScaleFromThreeScalar()
		{
			var matrix = Matrix.CreateScale(3, 4, 7);
			Assert.AreEqual(3, matrix.M11);
			Assert.AreEqual(4, matrix.M22);
			Assert.AreEqual(7, matrix.M33);
			Assert.AreEqual(1, matrix.M44);
		}

		[Test]
		public void RotateMatrix()
		{
			var matrix1 = new Matrix(1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1);
			var matrix2 = new Matrix(0, 0, -1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1);
			var matrix3 = new Matrix(0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
			var matrix4 = matrix1 * (matrix2 * matrix3);
			Assert.IsTrue(matrix1.IsNearlyEqual(Matrix.CreateRotationX(90)));
			Assert.IsTrue(matrix2.IsNearlyEqual(Matrix.CreateRotationY(90)));
			Assert.IsTrue(matrix3.IsNearlyEqual(Matrix.CreateRotationZ(90)));
			Assert.IsTrue(matrix4.IsNearlyEqual(Matrix.CreateRotationZYX(90, 90, 90)));
		}

		[Test]
		public void TranslateMatrix()
		{
			var matrix1 = Matrix.CreateTranslation(1, 2, 3);
			var matrix2 = Matrix.CreateTranslation(Vector.UnitX);
			Assert.AreEqual(matrix1.Translation, new Vector(1, 2, 3));
			Assert.AreEqual(matrix2.Translation, Vector.UnitX);
		}

		[Test]
		public void Transpose()
		{
			float[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			var matrix1 = new Matrix(values);
			Matrix matrix2 = Matrix.Transpose(matrix1);
			Assert.AreEqual(matrix1, Matrix.Transpose(matrix2));
		}

		[Test]
		public void GetDeterminant()
		{
			var matrix = Matrix.Identity;
			Assert.AreEqual(1, matrix.Determinant);
			matrix.M23 = 2;
			matrix.M24 = 1;
			matrix.M32 = 2;
			matrix.M34 = 3;
			matrix.M42 = 2;
			matrix.M43 = 1;
			Assert.AreEqual(6, matrix.Determinant);
			matrix = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			Assert.AreEqual(0, matrix.Determinant);
		}

		[Test]
		public void AreEqual()
		{
			float[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			var matrix1 = new Matrix(values);
			var matrix2 = new Matrix(values);
			Assert.IsTrue(matrix1 == matrix2);
			Assert.IsTrue(matrix1.Equals(matrix2));
			matrix2.M22 = 20;
			Assert.IsTrue(matrix1 != matrix2);
			Assert.IsTrue(matrix1.Equals(values));
			Assert.IsFalse(matrix1.Equals(Point.One));
		}

		[Test]
		public void GetRows()
		{
			var matrix1 = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			Assert.AreEqual(new Vector(0, 1, 2), matrix1.GetRow(0));
			Assert.AreEqual(new Vector(4, 5, 6), matrix1.GetRow(1));
			Assert.AreEqual(new Vector(8, 9, 10), matrix1.GetRow(2));
			Assert.AreEqual(new Vector(12, 13, 14), matrix1.GetRow(3));
		}

		[Test]
		public void AddMatrix()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			var matrix2 = new Matrix(2, 0, 1, 0, 0, 2, 0, 0, 0, 0, 2, 0, 2, 0, 0, 2);
			matrix2 = matrix1 + matrix2;
			Assert.AreEqual(3, matrix2.M11);
			Assert.AreEqual(0, matrix2.M12);
			Assert.AreEqual(4, matrix2.M13);
			Assert.AreEqual(0, matrix2.M14);
			Assert.AreEqual(0, matrix2.M21);
			Assert.AreEqual(4, matrix2.M22);
			Assert.AreEqual(0, matrix2.M23);
			Assert.AreEqual(4, matrix2.M24);
			Assert.AreEqual(0, matrix2.M31);
			Assert.AreEqual(3, matrix2.M32);
			Assert.AreEqual(4, matrix2.M33);
			Assert.AreEqual(0, matrix2.M34);
			Assert.AreEqual(3, matrix2.M41);
			Assert.AreEqual(0, matrix2.M42);
			Assert.AreEqual(0, matrix2.M43);
			Assert.AreEqual(4, matrix2.M44);
		}

		[Test]
		public void SubMatrix()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			var matrix2 = new Matrix(2, 0, 1, 0, 0, 2, 0, 0, 0, 0, 2, 0, 2, 0, 0, 2);
			matrix2 = matrix1 - matrix2;
			Assert.AreEqual(-1, matrix2.M11);
			Assert.AreEqual(0, matrix2.M12);
			Assert.AreEqual(2, matrix2.M13);
			Assert.AreEqual(0, matrix2.M14);
			Assert.AreEqual(0, matrix2.M21);
			Assert.AreEqual(0, matrix2.M22);
			Assert.AreEqual(0, matrix2.M23);
			Assert.AreEqual(4, matrix2.M24);
			Assert.AreEqual(0, matrix2.M31);
			Assert.AreEqual(3, matrix2.M32);
			Assert.AreEqual(0, matrix2.M33);
			Assert.AreEqual(0, matrix2.M34);
			Assert.AreEqual(-1, matrix2.M41);
			Assert.AreEqual(0, matrix2.M42);
			Assert.AreEqual(0, matrix2.M43);
			Assert.AreEqual(0, matrix2.M44);
		}

		[Test]
		public void MultiplyVector()
		{
			var matrix2 = Matrix.CreateScale(5);
			var vector = new Vector(3, 2, 2);
			vector = matrix2 * vector;
			Assert.AreEqual(new Vector(15, 10, 10), vector);
		}

		[TestCase]
		public void MultiplyScalar()
		{
			Matrix matrix1 = Matrix.Identity;
			var matrix2 = Matrix.CreateScale(5);
			matrix2.M44 = 5;
			Assert.AreEqual(matrix2, matrix1 * 5);
			Assert.AreEqual(matrix2, 5 * matrix1);
		}

		[Test]
		public void DivideByMatrix()
		{
			var matrix1 = Matrix.CreateScale(4);
			var matrix2 = new Matrix(2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2);
			matrix2 = matrix1 / matrix2;
			Assert.AreEqual(2, matrix2.M11);
			Assert.AreEqual(2, matrix2.M22);
			Assert.AreEqual(2, matrix2.M33);
			Assert.AreEqual(0.5f, matrix2.M44);
		}

		[Test]
		public void DivideByScalar()
		{
			var matrix1 = Matrix.CreateScale(6);
			const float Scalar = 4;
			Matrix matrix2 = matrix1 / Scalar;
			Assert.AreEqual(1.5f, matrix2.M11);
			Assert.AreEqual(1.5f, matrix2.M22);
			Assert.AreEqual(1.5f, matrix2.M33);
			Assert.AreEqual(0.25f, matrix2.M44);
		}

		[Test]
		public void NegateMatrix()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			Assert.AreEqual(-1 * matrix1, -matrix1);
		}

		[Test]
		public void MultiplyMatrix()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			var matrix2 = new Matrix(2, 0, 1, 0, 0, 2, 0, 0, 0, 0, 2, 0, 2, 0, 0, 2);
			Assert.AreEqual(matrix1, matrix1 * Matrix.Identity);
			matrix2 = matrix1 * matrix2;
			Assert.AreEqual(2, matrix2.M11);
			Assert.AreEqual(0, matrix2.M12);
			Assert.AreEqual(7, matrix2.M13);
			Assert.AreEqual(0, matrix2.M14);
			Assert.AreEqual(8, matrix2.M21);
			Assert.AreEqual(4, matrix2.M22);
			Assert.AreEqual(0, matrix2.M23);
			Assert.AreEqual(8, matrix2.M24);
			Assert.AreEqual(0, matrix2.M31);
			Assert.AreEqual(6, matrix2.M32);
			Assert.AreEqual(4, matrix2.M33);
			Assert.AreEqual(0, matrix2.M34);
			Assert.AreEqual(6, matrix2.M41);
			Assert.AreEqual(0, matrix2.M42);
			Assert.AreEqual(1, matrix2.M43);
			Assert.AreEqual(4, matrix2.M44);
		}

		[Test]
		public void GetInverse()
		{
			Matrix matrix1 = Matrix.Identity;
			matrix1.M23 = 2.0f;
			matrix1.M24 = 1.0f;
			matrix1.M32 = 2.0f;
			matrix1.M34 = 3.0f;
			matrix1.M42 = 2.0f;
			matrix1.M43 = 1.0f;
			Matrix matrix2 = matrix1.Inverse;
			Assert.IsTrue(matrix1.IsNearlyEqual(matrix2.Inverse));
			CheckPrecalculatedValues(matrix2);
		}

		private static void CheckPrecalculatedValues(Matrix matrix2)
		{
			Assert.AreEqual(1, matrix2.M11);
			Assert.IsTrue(matrix2.M22.IsNearlyEqual(-0.3333333f));
			Assert.IsTrue(matrix2.M23.IsNearlyEqual(-0.1666666f));
			Assert.IsTrue(matrix2.M24.IsNearlyEqual(0.8333333f));
			Assert.IsTrue(matrix2.M32.IsNearlyEqual(0.6666666f));
			Assert.IsTrue(matrix2.M33.IsNearlyEqual(-0.1666666f));
			Assert.IsTrue(matrix2.M34.IsNearlyEqual(-0.1666666f));
			Assert.IsTrue(matrix2.M42.IsNearlyEqual(0.0f));
			Assert.IsTrue(matrix2.M43.IsNearlyEqual(0.5f));
			Assert.IsTrue(matrix2.M44.IsNearlyEqual(-0.5f));
		}

		[Test]
		public void InverseExamples()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			var matrix2 = matrix1.Inverse;
			Assert.IsTrue(Matrix.Identity.IsNearlyEqual(matrix1 * matrix2));
			Assert.IsTrue(Matrix.Identity.IsNearlyEqual(matrix2 * matrix1));
		}

		[Test]
		public void WriteMatrix()
		{
			var matrix1 = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			const string MatrixString = "0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15";
			Assert.AreEqual(MatrixString, matrix1.ToString());
		}

		[Test]
		public void CalculateHashCode()
		{
			var matrix1 = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			int hashCode = 0.0f.GetHashCode() + 1.0f.GetHashCode() + 2.0f.GetHashCode() + 3.0f.GetHashCode() +
				4.0f.GetHashCode() + 5.0f.GetHashCode() + 6.0f.GetHashCode() + 7.0f.GetHashCode() +
				8.0f.GetHashCode() + 9.0f.GetHashCode() + 10.0f.GetHashCode() + 11.0f.GetHashCode() +
				12.0f.GetHashCode() + 13.0f.GetHashCode() + 14.0f.GetHashCode() + 15.0f.GetHashCode();
			Assert.AreEqual(hashCode, matrix1.GetHashCode());
		}

		[Test]
		public void TransformPosition()
		{
			var position = new Vector(3, 5, 2);
			var translation = Matrix.CreateTranslation(2, 0, 5);
			var rotation = Matrix.CreateRotationY(90);
			var scale = Matrix.CreateScale(3);
			var transformation = scale * rotation * translation;

			var result = translation * (rotation * (scale * position));
			Assert.AreEqual(result, transformation * position);
		}
	}
}