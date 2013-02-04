using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class RectangleTests
	{
		[Test]
		public void Create()
		{
			var point = new Point(2f, 2f);
			var size = new Size(1f, 1f);
			var rect = new Rectangle(point, size);
			Assert.AreEqual(point.X, rect.Left);
			Assert.AreEqual(point.Y, rect.Top);
			Assert.AreEqual(size.Width, rect.Width);
			Assert.AreEqual(size.Height, rect.Height);
			Assert.AreEqual(point, rect.TopLeft);
			Assert.AreEqual(size, rect.Size);
			Assert.AreEqual(new Rectangle(0, 0, 0, 0), Rectangle.Zero);
			Assert.AreEqual(new Rectangle(0, 0, 1, 1), Rectangle.One);
		}

		[Test]
		public void ChangeValues()
		{
			var rect = Rectangle.One;
			rect.Left = 2;
			rect.Top = 1;
			rect.Width = 2;
			rect.Height = 3;
			Assert.AreEqual(new Rectangle(2, 1, 2, 3), rect);
		}

		[Test]
		public void Equals()
		{
			var rect1 = new Rectangle(3, 4, 1, 2);
			var rect2 = new Rectangle(5, 6, 1, 2);
			Assert.AreNotEqual(rect1, rect2);
			Assert.AreEqual(rect1, new Rectangle(3, 4, 1, 2));
			Assert.IsTrue(rect1 == new Rectangle(3, 4, 1, 2));
			Assert.IsTrue(rect1 != rect2);
			Assert.IsFalse(rect1.Equals(rect2));
			Assert.IsTrue(rect1.Equals(rect1));
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var rect1 = new Rectangle(3, 4, 1, 2);
			var rect2 = new Rectangle(5, 6, 1, 2);
			var rectValues = new Dictionary<Rectangle, int> { { rect1, 1 }, { rect2, 2 } };
			Assert.IsTrue(rectValues.ContainsKey(rect1));
			Assert.IsTrue(rectValues.ContainsKey(rect2));
			Assert.IsFalse(rectValues.ContainsKey(new Rectangle(3, 9, 1, 2)));
		}

		[Test]
		public void RectangleToString()
		{
			var p = new Point(2f, 2f);
			var s = new Size(1f, 1f);
			var rect = new Rectangle(p, s);
			Assert.AreEqual("2 2 1 1", rect.ToString());
		}

		[Test]
		public void RectangleToStringAndFromString()
		{
			var rect = new Rectangle(2.12f, 2.12f, 1.12f, 1.12f);
			string rectString = rect.ToString();
			Assert.AreEqual(rect, new Rectangle(rectString));
			Assert.AreEqual(Rectangle.One, new Rectangle("0 0 1 1"));
			Assert.Throws<Rectangle.InvalidNumberOfComponents>(() => new Rectangle("abc"));
		}

		[Test]
		public void Right()
		{
			var rect = new Rectangle(1, 2, 10, 20) { Right = 13 };
			Assert.AreEqual(3, rect.Left);
			Assert.AreEqual(13, rect.Right);
			Assert.AreEqual(10, rect.Width);
		}

		[Test]
		public void Bottom()
		{
			var rect = new Rectangle(1, 2, 10, 20) { Bottom = 23 };
			Assert.AreEqual(3, rect.Top);
			Assert.AreEqual(23, rect.Bottom);
			Assert.AreEqual(20, rect.Height);
		}

		[Test]
		public void TopRight()
		{
			var rect = new Rectangle(1, 2, 10, 20) { TopRight = new Point(13, 4) };
			Assert.AreEqual(3, rect.Left);
			Assert.AreEqual(4, rect.Top);
			Assert.AreEqual(10, rect.Width);
			Assert.AreEqual(20, rect.Height);
			Assert.AreEqual(new Point(13, 4), rect.TopRight);
		}

		[Test]
		public void BottomLeft()
		{
			var rect = new Rectangle(1, 2, 10, 20) { BottomLeft = new Point(3, 24) };
			Assert.AreEqual(3, rect.Left);
			Assert.AreEqual(4, rect.Top);
			Assert.AreEqual(10, rect.Width);
			Assert.AreEqual(20, rect.Height);
			Assert.AreEqual(new Point(3, 24), rect.BottomLeft);
		}

		[Test]
		public void BottomRight()
		{
			var rect = new Rectangle(1, 2, 10, 20) { BottomRight = new Point(13, 24) };
			Assert.AreEqual(3, rect.Left);
			Assert.AreEqual(4, rect.Top);
			Assert.AreEqual(10, rect.Width);
			Assert.AreEqual(20, rect.Height);
			Assert.AreEqual(new Point(13, 24), rect.BottomRight);
		}

		[Test]
		public void GetCenter()
		{
			var rect = new Rectangle(4, 4, 4, 4);
			Assert.AreEqual(new Point(4, 4), rect.TopLeft);
			Assert.AreEqual(new Point(8, 8), rect.BottomRight);
			Assert.AreEqual(new Point(6, 6), rect.Center);
		}

		[Test]
		public void SetCenter()
		{
			var rect = new Rectangle(8, 10, 2, 2) { Center = Point.One };
			Assert.AreEqual(new Point(0, 0), rect.TopLeft);
			Assert.AreEqual(new Point(2, 2), rect.BottomRight);
			Assert.AreEqual(new Point(1, 1), rect.Center);
		}

		[Test]
		public void Contains()
		{
			var rect = new Rectangle(1, 2, 10, 20);
			Assert.IsTrue(rect.Contains(new Point(1, 2)));
			Assert.IsTrue(rect.Contains(new Point(5, 5)));
			Assert.IsFalse(rect.Contains(new Point(11, 5)));
			Assert.IsFalse(rect.Contains(new Point(5, 22)));
		}

		[Test]
		public void FromCenter()
		{
			Rectangle rect = Rectangle.FromCenter(new Point(11, 12), new Size(4, 6));
			Assert.AreEqual(new Rectangle(9, 9, 4, 6), rect);
			Rectangle anotherRect = Rectangle.FromCenter(0.5f, 0.5f, 1.0f, 1.0f);
			Assert.AreEqual(new Rectangle(0, 0, 1, 1), anotherRect);
		}

		[Test]
		public void Aspect()
		{
			Assert.AreEqual(0.5f, new Rectangle(0, 0, 1, 2).Aspect);
			Assert.AreEqual(2.0f, new Rectangle(0, 0, 4, 2).Aspect);
		}

		[Test]
		public void Reduce()
		{
			var rect = new Rectangle(1, 1, 2, 2);
			Assert.AreEqual(new Rectangle(1.5f, 1.5f, 1, 1), rect.Reduce(Size.One));
		}

		[Test]
		public void GetInnerRectangle()
		{
			var rect = new Rectangle(1, 1, 2, 2);
			Assert.AreEqual(rect, rect.GetInnerRectangle(Rectangle.One));
			Assert.AreEqual(new Rectangle(1.0f, 1.0f, 1.0f, 1.0f),
				rect.GetInnerRectangle(new Rectangle(0.0f, 0.0f, 0.5f, 0.5f)));
			Assert.AreEqual(new Rectangle(2.0f, 2.0f, 1.0f, 1.0f),
				rect.GetInnerRectangle(new Rectangle(0.5f, 0.5f, 0.5f, 0.5f)));
		}

		[Test]
		public void Move()
		{
			var rect = new Rectangle(1, 1, 1, 1);
			Assert.AreEqual(rect, rect.Move(Point.Zero));
			Assert.AreEqual(new Rectangle(2.0f, 2.0f, 1.0f, 1.0f), rect.Move(Point.One));
			Assert.AreEqual(new Rectangle(-1.0f, -2.0f, 1.0f, 1.0f), rect.Move(new Point(-2, -3)));
		}
	}
}