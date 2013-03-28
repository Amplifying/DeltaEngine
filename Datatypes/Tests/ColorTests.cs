using System.IO;
using DeltaEngine.Core;
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
		public void CreateFromString()
		{
			var color = new Color("R=1,G=2,B=3,A=4");
			Assert.AreEqual(1, color.R);
			Assert.AreEqual(2, color.G);
			Assert.AreEqual(3, color.B);
			Assert.AreEqual(4, color.A);
		}

		[Test]
		public void MissingComponentThrowsError()
		{
			Assert.Throws<Color.InvalidNumberOfComponents>(() => new Color("R=1,B=3,A=4"));
		}

		[Test]
		public void DuplicateComponentThrowsError()
		{
			Assert.Throws<Color.InvalidNumberOfComponents>(() => new Color("R=1,B=2,B=3,A=4"));
		}

		[Test]
		public void MalformedComponentThrowsError()
		{
			Assert.Throws<Color.InvalidNumberOfComponents>(() => new Color("R=1,G,B=3,A=4"));
		}

		[Test]
		public void NonByteComponentThrowsError()
		{
			Assert.Throws<Color.InvalidNumberOfComponents>(() => new Color("R=1,G=999,B=3,A=4"));
		}

		[Test]
		public void CommonColors()
		{
			Assert.AreEqual(new Color(0, 0, 0), Color.Black);
			Assert.AreEqual(new Color(255, 255, 255), Color.White);
			Assert.AreEqual(new Color(0, 0, 255), Color.Blue);
			Assert.AreEqual(new Color(0, 255, 255), Color.Cyan);
			Assert.AreEqual(new Color(128, 128, 128), Color.Gray);
			Assert.AreEqual(new Color(0, 255, 0), Color.Green);
			Assert.AreEqual(new Color(255, 165, 0), Color.Orange);
			Assert.AreEqual(new Color(255, 192, 203), Color.Pink);
			Assert.AreEqual(new Color(255, 0, 255), Color.Purple);
			Assert.AreEqual(new Color(255, 0, 0), Color.Red);
			Assert.AreEqual(new Color(0, 128, 128), Color.Teal);
			Assert.AreEqual(new Color(255, 255, 0), Color.Yellow);
		}

		[Test]
		public void NotSoCommonColors()
		{
			Assert.AreEqual(new Color(100, 149, 237), Color.CornflowerBlue);
			Assert.AreEqual(new Color(0.65f, 0.795f, 1f), Color.LightBlue);
			Assert.AreEqual(new Color(165, 165, 165), Color.LightGray);
			Assert.AreEqual(new Color(89, 89, 89), Color.DarkGray);
			Assert.AreEqual(new Color(0, 100, 0), Color.DarkGreen);
			Assert.AreEqual(new Color(255, 215, 0), Color.Gold);
			Assert.AreEqual(new Color(152, 251, 152), Color.PaleGreen);
		}

		[Test]
		public void SizeOfColor()
		{
			Assert.AreEqual(4, Color.SizeInBytes);
		}

		[Test]
		public void GetColorComponentsAsFloats()
		{
			var color = new Color(1.0f, 0.0f, 0.5f);
			Assert.AreEqual(1.0f, color.RedValue);
			Assert.AreEqual(0.0f, color.GreenValue);
			Assert.AreEqual(0.498039216f, color.BlueValue);
			Assert.AreEqual(1.0f, color.AlphaValue);
			color.AlphaValue = 0.0f;
			Assert.AreEqual(0.0f, color.AlphaValue);
		}

		[Test]
		public void PackedRgba()
		{
			var color1 = new Color(10, 20, 30, 40);
			var color2 = new Color(20, 30, 40, 50);
			var color3 = new Color(200, 200, 200, 200);
			Assert.AreNotEqual(color1.PackedRgba, color2.PackedRgba);
			Assert.AreEqual(color1.PackedRgba,
				color1.R + ((uint)color1.G << 8) + ((uint)color1.B << 16) + ((uint)color1.A << 24));
			Assert.AreEqual((uint)color3.PackedRgba,
				color3.R + ((uint)color3.G << 8) + ((uint)color3.B << 16) + ((uint)color3.A << 24));
		}

		[Test]
		public void PackedBgra()
		{
			var color1 = new Color(10, 20, 30, 40);
			var color2 = new Color(20, 30, 40, 50);
			Assert.AreNotEqual(color1.PackedBgra, color2.PackedBgra);
			Assert.AreEqual(color1.PackedBgra,
				color1.B + ((uint)color1.G << 8) + ((uint)color1.R << 16) + ((uint)color1.A << 24));
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
			Assert.AreEqual(color1.PackedRgba, color1.GetHashCode());
		}

		[Test]
		public new void ToString()
		{
			Assert.AreEqual("(R=10, G=20, B=30, A=40)", new Color(10, 20, 30, 40).ToString());
			Assert.AreEqual("(R=255, G=255, B=255, A=255)", Color.White.ToString());
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
		public void Multiply()
		{
			var color1 = new Color(128, 128, 128);
			var halfColor1 = color1 * 0.5f;
			Assert.AreEqual(new Color(64, 64, 64, 127), halfColor1);
			var color2 = new Color(0.1f, 0.1f, 0.1f);
			Assert.AreEqual(new Color(25, 25, 25), color2);
			Assert.AreEqual(new Color(100, 100, 100), color2 * 4);
			var multipliedColor = color1 * color2;
			Assert.AreEqual(new Color(0.05f, 0.05f, 0.05f), multipliedColor);
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

		[Test]
		public void GetRandomBrightColorWithRandomizerInjected()
		{
			Color.Randomizer = new PseudoRandom();
			Color color = Color.GetRandomBrightColor();
			Assert.IsTrue(color.R > 127);
			Assert.IsTrue(color.G > 127);
			Assert.IsTrue(color.B > 127);
			Assert.AreEqual(255, color.A);
		}

		[Test]
		public void GetRandomColor()
		{
			Color.Randomizer = null;
			Color color = Color.GetRandomColor();
			Assert.IsTrue(color.R > 0 && color.G > 0 && color.B > 0);
			Assert.AreEqual(255, color.A);
		}

		[Test]
		public void SaveColor()
		{
			using (var dataStream = new MemoryStream())
			{
				var writer = new BinaryWriter(dataStream);
				var data = Color.Red;
				data.SaveData(writer);
				byte[] savedBytes = dataStream.ToArray();
				Assert.AreEqual(4, savedBytes.Length);
				Assert.AreEqual(255, savedBytes[0]);
				Assert.AreEqual(0, savedBytes[1]);
				Assert.AreEqual(0, savedBytes[2]);
				Assert.AreEqual(255, savedBytes[3]);
			}
		}

		[Test]
		public void LoadColor()
		{
			using (var dataStream = new MemoryStream())
			{
				var writer = new BinaryWriter(dataStream);
				var data = Color.Red;
				data.SaveData(writer);
				dataStream.Seek(0, SeekOrigin.Begin);
				var reader = new BinaryReader(dataStream);
				data.LoadData(reader);
				Assert.AreEqual(Color.Red, data);
			}
		}

		[Test]
		public void GetBytes()
		{
			var colors = new[] { Color.Black, Color.White };
			Assert.AreEqual(new byte[] { 0, 0, 0, 255, 255, 255, 255, 255 }, Color.GetBytes(colors));
		}
	}
}