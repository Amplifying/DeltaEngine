using NUnit.Framework;

namespace DeltaEngine.Core.Tests
{
	public class MathExtensionsTests
	{
		private const float Precision = 0.0001f;

		[Test]
		public void Abs()
		{
			Assert.AreEqual(5, MathExtensions.Abs(2) + MathExtensions.Abs(3));
			Assert.AreEqual(5, MathExtensions.Abs(-2) + MathExtensions.Abs(3));

			Assert.AreEqual(5, 2.0f.Abs() + 3.0f.Abs());
			Assert.AreEqual(5, (-2.0f).Abs() + 3.0f.Abs());
		}

		[Test]
		public void IsNearlyEqual()
		{
			Assert.IsTrue(2.0f.IsNearlyEqual(2.00001f));
			Assert.IsFalse(2.0f.IsNearlyEqual(2.0002f));
			Assert.IsTrue(MathExtensions.Pi.IsNearlyEqual(3.1415f));
		}

		[Test]
		public static void Round()
		{
			Assert.AreEqual(1, MathExtensions.Round(1.25f));
			Assert.AreEqual(10, MathExtensions.Round(9.68f));
			Assert.AreEqual(1.23f, MathExtensions.Round(1.2345f, 2));
		}

		[Test]
		public void Sin()
		{
			Assert.AreEqual(0, MathExtensions.Sin(0));
			Assert.AreEqual(1, MathExtensions.Sin(90));
			Assert.AreEqual(0, MathExtensions.Sin(180), Precision);
			Assert.AreEqual(0, MathExtensions.Sin(360), Precision);
			Assert.AreNotEqual(0, MathExtensions.Sin(32));
		}

		[Test]
		public void Cos()
		{
			Assert.AreEqual(1, MathExtensions.Cos(0));
			Assert.AreEqual(0, MathExtensions.Cos(90), Precision);
			Assert.AreEqual(-1, MathExtensions.Cos(180), Precision);
			Assert.AreEqual(1, MathExtensions.Cos(360), Precision);
			Assert.AreNotEqual(0, MathExtensions.Cos(32));
		}

		[Test]
		public void Lerp()
		{
			Assert.AreEqual(1, MathExtensions.Lerp(1, 3, -1));
			Assert.AreEqual(0.2f, MathExtensions.Lerp(0, 1, 0.2f));
			Assert.AreEqual(0, MathExtensions.Lerp(-1, 1, 0.5f));
			Assert.AreEqual(-1, MathExtensions.Lerp(-4, -1, 1.5f));
		}
	}
}