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
			matrix[0] = 3;
			Assert.AreEqual(3, matrix[0]);
		}

		private static void AssertValues0To15(Matrix matrix)
		{
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(i, matrix[i]);
		}

		[Test]
		public void CreateScaleFromOneScalar()
		{
			var matrix = Matrix.CreateScale(5);
			Assert.AreEqual(5, matrix[0]);
			Assert.AreEqual(5, matrix[5]);
			Assert.AreEqual(5, matrix[10]);
			Assert.AreEqual(1, matrix[15]);
		}

		[Test]
		public void CreateScaleFromVector()
		{
			var matrix = Matrix.CreateScale(Vector.One);
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
			Assert.AreEqual(matrix1.GetRow(3), new Vector(1, 2, 3));
			Assert.AreEqual(matrix2.GetRow(3), Vector.UnitX);
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
			Size size = new Size(10, 5);
			var matrix = Matrix.GenerateOrthographicProjection(size);
			var values = new[] {0.2f, 0.0f, 0.0f, 0.0f, 0.0f, -0.4f, 0.0f, 0.0f,
				0.0f, 0.0f, -1.0f, 0.0f, -1.0f, 1.0f, 0.0f, 1.0f};
			Assert.AreEqual(matrix, new Matrix(values));
		}

		[Test]
		public void GetDeterminant()
		{
			var matrix = Matrix.Identity;
			Assert.AreEqual(1, matrix.Determinant);
			matrix[6] = 2;
			matrix[7] = 1;
			matrix[9] = 2;
			matrix[11] = 3;
			matrix[13] = 2;
			matrix[14] = 1;
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
			matrix2[5] = 20;
			Assert.IsTrue(matrix1 != matrix2);
			Assert.IsTrue(matrix1.Equals(values));
			values[0] = 17;
			Assert.IsFalse(matrix1.Equals(values));
			Assert.IsFalse(matrix1.Equals(Point.One));
			Assert.Throws<IndexOutOfRangeException>(() => matrix1.Equals(new float[17]));
		}

		[Test]
		public void GetRows()
		{
			var matrix1 = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			Assert.AreEqual(new Vector(0, 1, 2), matrix1.GetRow(0));
			Assert.AreEqual(new Vector(4, 5, 6), matrix1.GetRow(1));
			Assert.AreEqual(new Vector(8, 9, 10), matrix1.GetRow(2));
			Assert.AreEqual(new Vector(12, 13, 14), matrix1.GetRow(3));
			Assert.Throws<IndexOutOfRangeException>(() => matrix1.GetRow(-1));
		}

		[Test]
		public void MultiplyVector()
		{
			var matrix2 = Matrix.CreateScale(5);
			var vector = new Vector(3, 2, 2);
			vector = matrix2 * vector;
			Assert.AreEqual(new Vector(15, 10, 10), vector);
		}

		[Test]
		public void NegateMatrix()
		{
			var values = new float[] { 1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2 };
			var matrix1 = new Matrix(values);
			for (int i = 0; i < 16; i++)
				values[i] *= -1;
				Assert.AreEqual(-matrix1, new Matrix(values));
		}

		[Test]
		public void MultiplyMatrix()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			var matrix3 = new Matrix(2, 0, 1, 0, 0, 2, 0, 0, 0, 0, 2, 0, 2, 0, 0, 2);

			Assert.AreEqual(matrix1, matrix1 * Matrix.Identity);
			Matrix matrix2 = new Matrix();
			long start = DateTime.Now.Ticks;
			for(int i = 0; i < 1000; i++)
				matrix2 = matrix1 * matrix3;
			long stop = DateTime.Now.Ticks;
			double elapsed = (stop - start) / 1000.0 / 10000.0;
			Console.WriteLine("Multiplication time:  " + elapsed + " seconds");
			Assert.AreEqual(2, matrix2[0]);
			Assert.AreEqual(0, matrix2[1]);
			Assert.AreEqual(7, matrix2[2]);
			Assert.AreEqual(0, matrix2[3]);
			Assert.AreEqual(8, matrix2[4]);
			Assert.AreEqual(4, matrix2[5]);
			Assert.AreEqual(0, matrix2[6]);
			Assert.AreEqual(8, matrix2[7]);
			Assert.AreEqual(0, matrix2[8]);
			Assert.AreEqual(6, matrix2[9]);
			Assert.AreEqual(4, matrix2[10]);
			Assert.AreEqual(0, matrix2[11]);
			Assert.AreEqual(6, matrix2[12]);
			Assert.AreEqual(0, matrix2[13]);
			Assert.AreEqual(1, matrix2[14]);
			Assert.AreEqual(4, matrix2[15]);
		}

		[Test]
		public void IsNotNearlyEqual()
		{
			var matrix1 = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			Assert.IsFalse(matrix1.IsNearlyEqual(Matrix.Identity));
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