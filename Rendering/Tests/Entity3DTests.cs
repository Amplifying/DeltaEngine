using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class Entity3DTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeEntitiesRunner()
		{
			entities = new MockEntitiesRunner(typeof(MockUpdateBehavior));
		}

		private EntitiesRunner entities;

		[TearDown]
		public void DisposeEntitiesRunner()
		{
			entities.Dispose();
		}

		[Test]
		public void CreateEntity3D()
		{
			var entity = new Entity3D();
			Assert.AreEqual(Vector.Zero, entity.Position);
			Assert.AreEqual(Quaternion.Identity, entity.Orientation);
			Assert.AreEqual(Visibility.Show, entity.Visibility);
		}

		[Test]
		public void CreateEntity3DPositionAndOrientation()
		{
			var position = new Vector(10.0f, -3.0f, 27.0f);
			var orientation = Quaternion.Identity;
			var entity = new Entity3D(position, orientation);
			Assert.AreEqual(position, entity.Position);
			Assert.AreEqual(orientation, entity.Orientation);
		}

		[Test]
		public void SetPositionProperty()
		{
			var entity = new Entity3D { Position = Vector.One };
			Assert.AreEqual(Vector.One, entity.Position);
		}

		[Test]
		public void SetOrientationProperty()
		{
			var entity = new Entity3D { Orientation = Quaternion.Identity };
			Assert.AreEqual(Quaternion.Identity, entity.Orientation);
		}

		[Test]
		public void SetVisibilityProperty()
		{
			var entity = new Entity3D { Visibility = Visibility.Hide };
			Assert.AreEqual(Visibility.Hide, entity.Visibility);
		}
	}
}
