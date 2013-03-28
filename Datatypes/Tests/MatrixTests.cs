using System;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class MatrixTests
	{
		[Test]
		public void MatrixZero()
		{
			var matrix = new Matrix();
			Console.WriteLine(matrix);
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(0, matrix[i]);
		}

		[Test]
		public void CreateWith16Floats()
		{
			var matrix = new Matrix(new float[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15});
			AssertValues0To15(matrix);
		}

		[Test]
		public void CreateWithFloatArray()
		{
			float[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			var matrix = new Matrix(values);
			AssertValues0To15(matrix);
		}

		private static void AssertValues0To15(Matrix matrix)
		{
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(i, matrix[i]);
		}

		[Test]
		public void CreateScaleFromOneScalar()
		{
			var matrix = Matrix.CreateScale(5, 5, 5);
			Assert.AreEqual(5, matrix[0]);
			Assert.AreEqual(5, matrix[5]);
			Assert.AreEqual(5, matrix[10]);
			Assert.AreEqual(1, matrix[15]);
		}

		[Test]
		public void CreateScaleFromVector()
		{
			var matrix = Matrix.CreateScale(1, 1, 1);
			Assert.AreEqual(1, matrix[0]);
			Assert.AreEqual(1, matrix[5]);
			Assert.AreEqual(1, matrix[10]);
			Assert.AreEqual(1, matrix[15]);
		}

		[Test]
		public void CreateScaleFromThreeScalar()
		{
			var matrix = Matrix.CreateScale(3, 4, 7);
			Assert.AreEqual(3, matrix[0]);
			Assert.AreEqual(4, matrix[5]);
			Assert.AreEqual(7, matrix[10]);
			Assert.AreEqual(1, matrix[15]);
		}

		[Test]
		public void SizeOfMatrix()
		{
			Assert.AreEqual(64, Matrix.SizeInBytes);
		}

		[Test]
		public void RotateMatrix()
		{
			var matrix1 = new Matrix(new float[]{1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1});
			var matrix2 = new Matrix(new float[]{0, 0, -1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1});
			var matrix3 = new Matrix(new float[]{0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1});
			var matrix4 = matrix1 * (matrix2 * matrix3);
			Assert.IsTrue(matrix1.IsNearlyEqual(Matrix.CreateRotationXyz(90, 0, 0)));
			Assert.IsTrue(matrix2.IsNearlyEqual(Matrix.CreateRotationXyz(0, 90, 0)));
			Assert.IsTrue(matrix3.IsNearlyEqual(Matrix.CreateRotationXyz(0, 0, 90)));
			Assert.IsTrue(matrix4.IsNearlyEqual(Matrix.CreateRotationXyz(90, 90, 90)));
		}

		[Test]
		public void TranslateMatrix()
		{
			var matrix1 = Matrix.CreateTranslation(1, 2, 3);
			var matrix2 = Matrix.CreateTranslation(1, 0, 0);
			Assert.AreEqual(new Vector(matrix1[12], matrix1[13], matrix1[14]), new Vector(1, 2, 3));
			Assert.AreEqual(new Vector(matrix2[12], matrix2[13], matrix2[14]), Vector.UnitX);
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
		public void CreateOrtographicProjectionMatrix()
		{
			var size = new Size(10, 5);
			var matrix = Matrix.GenerateOrthographicProjection(size);
			var values = new[] {0.2f, 0.0f, 0.0f, 0.0f, 0.0f, -0.4f, 0.0f, 0.0f,
				0.0f, 0.0f, -1.0f, 0.0f, -1.0f, 1.0f, 0.0f, 1.0f};
			Assert.AreEqual(matrix, new Matrix(values));
		}

		[Test]
		public void AccesViolation()
		{
			var matrix1 = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			float num = matrix1[15];
			Assert.AreEqual(15, num);
			Assert.Throws<IndexOutOfRangeException>(delegate { num = matrix1[17]; });
		}

		[Test]
		public void GetDeterminant()
		{
			var matrix = Matrix.Identity;
			Assert.AreEqual(1, matrix.GetDeterminant());
			matrix[6] = 2;
			matrix[7] = 1;
			matrix[9] = 2;
			matrix[11] = 3;
			matrix[13] = 2;
			matrix[14] = 1;
			Assert.AreEqual(6, matrix.GetDeterminant());
		}

		[Test]
		public void AreEqual()
		{
			float[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
			var matrix1 = new Matrix(values);
			var matrix2 = new Matrix(values);
			Assert.IsTrue(matrix1 == matrix2);
			Assert.IsTrue(matrix1.Equals(matrix2));
			matrix2[5] = 20;
			Assert.IsTrue(matrix1 != matrix2);
			object pointAsObject = Point.One;
			Assert.IsFalse(matrix1.Equals(pointAsObject));
		}

		[Test]
		public void MultiplyVector()
		{
			var matrix2 = Matrix.CreateScale(5, 5, 5);
			var vector = new Vector(3, 2, 2);
			vector = matrix2 * vector;
			Assert.AreEqual(new Vector(15, 10, 10), vector);
		}

		[Test]
		public void MultiplyMatrix()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			var matrix3 = new Matrix(2, 0, 1, 0, 0, 2, 0, 0, 0, 0, 2, 0, 2, 0, 0, 2);
			var result = new Matrix(2, 0, 7, 0, 8, 4, 0, 8, 0, 6, 4, 0, 6, 0, 1, 4);
			Assert.AreEqual(matrix1, matrix1 * Matrix.Identity);
			Matrix matrix2 = matrix1 * matrix3;
			Assert.AreEqual(result, matrix2);
		}

		[Test]
		public void IsNotNearlyEqual()
		{
			var matrix1 = new Matrix(new float[]{1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2});
			Assert.IsFalse(matrix1.IsNearlyEqual(Matrix.Identity));
		}

		[Test]
		public void WriteMatrix()
		{
			var matrix1 = new Matrix(new float[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15});
			const string MatrixString = "0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15";
			Assert.AreEqual(MatrixString, matrix1.ToString());
		}

		[Test]
		public void CalculateHashCode()
		{
			var matrix1 = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			Assert.AreEqual(204607600, matrix1.GetHashCode());
			var matrix2 = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
			Assert.AreEqual(0, matrix2.GetHashCode());
		}

		[Test]
		public void TransformPosition()
		{
			var position = new Vector(3, 5, 2);
			var translation = Matrix.CreateTranslation(2, 0, 5);
			var rotation = Matrix.CreateRotationXyz(0, 90, 0);
			var scale = Matrix.CreateScale(3, 3, 3);
			var transformation = scale * rotation * translation;
			var result = translation * (rotation * (scale * position));
			Assert.AreEqual(result, transformation * position);
		}
	}
}