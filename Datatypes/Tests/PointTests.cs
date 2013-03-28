using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class PointTests
	{
		[Test]
		public void Create()
		{
			const float X = 3.51f;
			const float Y = 0.23f;
			var p = new Point(X, Y);
			Assert.AreEqual(p.X, X);
			Assert.AreEqual(p.Y, Y);
		}

		[Test]
		public void Statics()
		{
			Assert.AreEqual(new Point(0, 0), Point.Zero);
			Assert.AreEqual(new Point(1, 1), Point.One);
			Assert.AreEqual(new Point(0.5f, 0.5f), Point.Half);
			Assert.AreEqual(new Point(1, 0), Point.UnitX);
			Assert.AreEqual(new Point(0, 1), Point.UnitY);
			Assert.AreEqual(8, Point.SizeInBytes);
		}

		[Test]
		public void ChangePoint()
		{
			var point = new Point(1.0f, 1.0f) { X = 2.1f, Y = 2.1f };
			Assert.AreEqual(2.1f, point.X);
			Assert.AreEqual(2.1f, point.Y);
		}

		[Test]
		public void Addition()
		{
			var p1 = new Point(1, 2);
			var p2 = new Point(3, 4);
			Assert.AreEqual(new Point(4, 6), p1 + p2);
		}

		[Test]
		public void Subtraction()
		{
			var p1 = new Point(1, 2);
			var p2 = new Point(3, 4);
			Assert.AreEqual(new Point(-2, -2), p1 - p2);
		}

		[Test]
		public void Negation()
		{
			var point = new Point(1, 2);
			Assert.AreEqual(-point, new Point(-1, -2));
		}

		[Test]
		public void Multiplication()
		{
			var p = new Point(2, 4);
			const float F = 1.5f;
			Assert.AreEqual(new Point(3, 6), p * F);
			Assert.AreEqual(new Point(3, 6), F * p);
		}

		[Test]
		public void Division()
		{
			var p = new Point(2, 4);
			const float F = 2f;
			Assert.AreEqual(new Point(1, 2), p / F);
		}

		[Test]
		public void Equals()
		{
			var p1 = new Point(1, 2);
			var p2 = new Point(3, 4);
			Assert.AreNotEqual(p1, p2);
			Assert.AreEqual(p1, new Point(1, 2));
			Assert.IsTrue(p1 == new Point(1, 2));
			Assert.IsTrue(p1 != p2);
			Assert.IsTrue(p1.Equals((object)new Point(1, 2)));
		}

		[Test]
		public void ImplicitCastFromSize()
		{
			var p = new Point(1, 2);
			var s = new Size(1, 2);
			Point addition = p + s;
			Assert.AreEqual(new Point(2, 4), addition);
		}

		[Test]
		public void DistanceTo()
		{
			var zero = new Point();
			var p = new Point(3, 4);
			Assert.AreEqual(5, zero.DistanceTo(p));
			Assert.AreEqual(0, zero.DistanceTo(zero));
		}

		[Test]
		public void DistanceToSquared()
		{
			var zero = new Point();
			var p = new Point(3, 4);
			Assert.AreEqual(25, zero.DistanceToSquared(p));
			Assert.AreEqual(0, zero.DistanceToSquared(zero));
		}

		[Test]
		public void DirectionTo()
		{
			var p1 = new Point(1, 2);
			var p2 = new Point(4, -5);
			Assert.AreEqual(new Point(3, -7), p1.DirectionTo(p2));
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var first = new Point(1, 2);
			var second = new Point(3, 4);
			var pointValues = new Dictionary<Point, int> { { first, 1 }, { second, 2 } };
			Assert.IsTrue(pointValues.ContainsKey(first));
			Assert.IsTrue(pointValues.ContainsKey(second));
			Assert.IsFalse(pointValues.ContainsKey(new Point(5, 6)));
		}

		[Test]
		public void PointToString()
		{
			var p = new Point(3, 4);
			Assert.AreEqual("(3, 4)", p.ToString());
		}

		[Test]
		public void PointToStringAndFromString()
		{
			var p = new Point(2.23f, 3.45f);
			string pointString = p.ToString();
			Assert.AreEqual(p, new Point(pointString));
			Assert.Throws<Point.InvalidNumberOfComponents>(() => new Point("0.0"));
		}

		[Test]
		public void ReflectIfHittingBorder()
		{
			var direction = Point.One;
			var areaInsideBorders = new Rectangle(Point.Half, Size.Zero);
			var borders = Rectangle.One;
			direction.ReflectIfHittingBorder(areaInsideBorders, borders);
			Assert.AreEqual(Point.One, direction);
			var bottomRightArea = new Rectangle(Point.One, Size.Half);
			direction.ReflectIfHittingBorder(bottomRightArea, borders);
			Assert.AreEqual(-Point.One, direction);
			var topLeftArea = new Rectangle(Point.Zero, Size.Zero);
			direction.ReflectIfHittingBorder(topLeftArea, borders);
			Assert.AreEqual(Point.One, direction);
		}

		[Test]
		public void Lerp()
		{
			Assert.AreEqual(Point.Half, Point.Lerp(Point.Zero, Point.One, 0.5f));
			Assert.AreEqual(Point.One, Point.Lerp(Point.Zero, Point.One, 1.1f));
			Assert.AreEqual(new Point(1.5f, 1.0f), Point.Lerp(new Point(1, 2), new Point(5, -6), 0.125f));
		}

		[Test]
		public void RotateAround()
		{
			var point = Point.UnitX;
			point.RotateAround(Point.Zero, 90.0f);
			Assert.AreEqual(Point.UnitY, point);
			point.RotateAround(new Point(0.0f, 0.5f), 180.0f);
			Assert.AreEqual(Point.Zero, point);
		}

		[Test]
		public void Normalize()
		{
			var point = new Point(0.3f, -0.4f);
			point.Normalize();
			Assert.AreEqual(new Point(0.6f, -0.8f), point);
		}

		[Test]
		public void DotProduct()
		{
			var point1 = new Point(1, 1);
			point1.Normalize();
			var point2 = new Point(-1, 1);
			point2.Normalize();
			Assert.AreEqual(0.0f, point1.DotProduct(point2));
			Assert.AreEqual(0.7071f, point1.DotProduct(Point.UnitY), 0.0001f);
		}
	}
}