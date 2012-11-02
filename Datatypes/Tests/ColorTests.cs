using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class ColorTests
	{
		[Test]
		public void CreateWithBytes()
		{
			var color = new Color(1, 2, 3, 4);
			Assert.AreEqual(1, color.R);
			Assert.AreEqual(2, color.G);
			Assert.AreEqual(3, color.B);
			Assert.AreEqual(4, color.A);
		}

		[Test]
		public void ChangeColor()
		{
			var color = new Color(1, 2, 3, 4) { R = 5, G = 6, B = 7, A = 8 };
			Assert.AreEqual(5, color.R);
			Assert.AreEqual(6, color.G);
			Assert.AreEqual(7, color.B);
			Assert.AreEqual(8, color.A);
		}

		[Test]
		public void CreateWithFloats()
		{
			var color = new Color(1.0f, 0.0f, 0.5f);
			Assert.AreEqual(255, color.R);
			Assert.AreEqual(0, color.G);
			Assert.AreEqual(127, color.B);
			Assert.AreEqual(255, color.A);
		}

		[Test]
		public void CommonColors()
		{
			Assert.AreEqual(new Color(0, 0, 0), Color.Black);
			Assert.AreEqual(new Color(0, 0, 255), Color.Blue);
			Assert.AreEqual(new Color(100, 149, 237), Color.CornflowerBlue);
			Assert.AreEqual(new Color(0, 255, 255), Color.Cyan);
			Assert.AreEqual(new Color(128, 128, 128), Color.Gray);
			Assert.AreEqual(new Color(0, 255, 0), Color.Green);
			Assert.AreEqual(new Color(152, 251, 152), Color.PaleGreen);
			Assert.AreEqual(new Color(255, 192, 203), Color.Pink);
			Assert.AreEqual(new Color(255, 0, 255), Color.Purple);
			Assert.AreEqual(new Color(255, 0, 0), Color.Red);
			Assert.AreEqual(new Color(255, 255, 255), Color.White);
			Assert.AreEqual(new Color(255, 255, 0), Color.Yellow);
		}

		[Test]
		public void PackedArgb()
		{
			var color1 = new Color(10, 20, 30, 40);
			var color2 = new Color(20, 30, 40, 50);
			var color3 = new Color(200, 200, 200, 200);
			Assert.AreNotEqual(color1.PackedArgb, color2.PackedArgb);
			Assert.AreEqual(color1.PackedArgb,
				((uint)color1.A << 24) + ((uint)color1.R << 16) + ((uint)color1.G << 8) + color1.B);
			Assert.AreEqual((uint)color3.PackedArgb,
				((uint)color3.A << 24) + ((uint)color3.R << 16) + ((uint)color3.G << 8) + color3.B);
		}

		[Test]
		public void Equals()
		{
			var color1 = new Color(10, 20, 30, 40);
			var color2 = new Color(20, 30, 40, 50);

			Assert.AreNotEqual(color1, color2);
			Assert.AreEqual(color1, new Color(10, 20, 30, 40));

			Assert.IsTrue(color1 == new Color(10, 20, 30, 40));
			Assert.IsTrue(color1 != color2);
			Assert.IsTrue(color1.Equals((object)new Color(10, 20, 30, 40)));
			Assert.AreEqual(color1.PackedArgb, color1.GetHashCode());
		}

		[Test]
		public void Lerp()
		{
			var color1 = new Color(10, 20, 30, 40);
			var color2 = new Color(20, 30, 40, 50);
			var lerp20 = new Color(12, 22, 32, 42);
			Assert.AreEqual(lerp20, Color.Lerp(color1, color2, 0.2f));
			Assert.AreEqual(color1, Color.Lerp(color1, color2, -1));
			Assert.AreEqual(color2, Color.Lerp(color1, color2, 1.1f));
		}

		[Test]
		public void GetRandomBrightColor()
		{
			Color color = Color.GetRandomBrightColor();
			Assert.IsTrue(color.R > 127);
			Assert.IsTrue(color.G > 127);
			Assert.IsTrue(color.B > 127);
			Assert.AreEqual(255, color.A);
		}
	}
}