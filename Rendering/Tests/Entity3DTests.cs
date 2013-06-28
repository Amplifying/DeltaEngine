using NUnit.Framework;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Tests
{
	public class Entity3DTests
	{
		[Test]
		public void CreateEntity3D()
		{
			var entity = new Entity3D();
			Assert.AreEqual(Vector.Zero, entity.Position);
			Assert.AreEqual(Vector.Zero, entity.EulerAngles);
			Assert.AreEqual(Visibility.Show, entity.Visibility);
		}

		[Test]
		public void CreateEntity3DWithTransform()
		{
			var position = new Vector(10.0f, -3.0f, 27.0f);
			var eulerAngles = new Vector(90.0f, 10.0f, -30.0f);
			var entity = new Entity3D(new Transform(position, eulerAngles));
			Assert.AreEqual(position, entity.Position);
			Assert.AreEqual(eulerAngles, entity.EulerAngles);
		}

		[Test]
		public void SetPositionProperty()
		{
			var entity = new Entity3D { Position = Vector.One };
			Assert.AreEqual(Vector.One, entity.Position);
		}

		[Test]
		public void SetEulerAnglesProperty()
		{
			var entity = new Entity3D { EulerAngles = Vector.One };
			Assert.AreEqual(Vector.One, entity.EulerAngles);
		}

		[Test]
		public void SetVisibilityProperty()
		{
			var entity = new Entity3D { Visibility = Visibility.Hide };
			Assert.AreEqual(Visibility.Hide, entity.Visibility);
		}
	}
}
