using System.IO;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class DatatypesLoadSaveTests
	{
		[Test]
		public void SaveColor()
		{
			byte[] savedBytes = Color.Red.ToByteArray();
			Assert.AreEqual(4, savedBytes.Length);
			Assert.AreEqual(255, savedBytes[0]);
			Assert.AreEqual(0, savedBytes[1]);
			Assert.AreEqual(0, savedBytes[2]);
			Assert.AreEqual(255, savedBytes[3]);
		}

		[Test]
		public void LoadColor()
		{
			var data = Color.Red.SaveToMemoryStream();
			var reconstructedColor = data.CreateFromMemoryStream();
			Assert.AreEqual(Color.Red, reconstructedColor);
		}

		[Test]
		public void SaveAndLoadSize()
		{
			var data = Size.Half.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(1 + "Size".Length + Size.SizeInBytes, savedBytes.Length);
			Assert.AreEqual("Size".Length, savedBytes[0]);
			var reconstructed = data.CreateFromMemoryStream();
			Assert.AreEqual(Size.Half, reconstructed);
		}

		[Test]
		public void SaveAndLoadPoint()
		{
			var data = Point.Half.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(1 + "Point".Length + Point.SizeInBytes, savedBytes.Length);
			Assert.AreEqual("Point".Length, savedBytes[0]);
			var reconstructed = data.CreateFromMemoryStream();
			Assert.AreEqual(Point.Half, reconstructed);
		}

		[Test]
		public void SaveAndLoadRectangle()
		{
			var data = Rectangle.One.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(1 + "Rectangle".Length + Rectangle.SizeInBytes, savedBytes.Length);
			Assert.AreEqual("Rectangle".Length, savedBytes[0]);
			var reconstructed = data.CreateFromMemoryStream();
			Assert.AreEqual(Rectangle.One, reconstructed);
		}

		[Test]
		public void SaveAndLoadRectangleManuallyWithBinaryWriterAndReader()
		{
			using (var dataStream = new MemoryStream())
			{
				var writer = new BinaryWriter(dataStream);
				var data = Rectangle.One;
				data.Save(writer);
				dataStream.Seek(0, SeekOrigin.Begin);
				var reader = new BinaryReader(dataStream);
				data = (Rectangle)reader.Create();
				Assert.AreEqual(Rectangle.One, data);
			}
		}

		[Test]
		public void SaveAndLoadVector()
		{
			var data = Vector.UnitZ.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(1 + "Vector".Length + Vector.SizeInBytes, savedBytes.Length);
			Assert.AreEqual("Vector".Length, savedBytes[0]);
			var reconstructed = data.CreateFromMemoryStream();
			Assert.AreEqual(Vector.UnitZ, reconstructed);
		}

		[Test]
		public void SaveAndLoadMatrix()
		{
			var data = Matrix.Identity.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(1 + "Matrix".Length + Matrix.SizeInBytes, savedBytes.Length);
			Assert.AreEqual("Matrix".Length, savedBytes[0]);
			var reconstructed = (Matrix)data.CreateFromMemoryStream();
			Assert.AreEqual(Matrix.Identity, reconstructed);
		}
	}
}
