using NUnit.Framework;

namespace DeltaEngine.Datatypes.Tests
{
	public class EntityTests
	{
		[Test]
		public void CheckNameAndDefaultValues()
		{
			Assert.AreEqual("EmptyTest", emptyEntity.Name);
			Assert.IsEmpty(emptyEntity.Components);
			Assert.IsNull(emptyEntity.BasedOn);
		}

		private readonly Entity emptyEntity = new Entity("EmptyTest");

		[Test]
		public void EntityBasedOnEmptyEntity()
		{
			var entity = new Entity("Test", emptyEntity);
			Assert.IsNotNull(entity.BasedOn);
		}

		[Test]
		public void CreateDerivedEntity()
		{
			var derivedEntity = new DerivedEntity();
			Assert.AreEqual("", derivedEntity.Name);
		}

		private class DerivedEntity : Entity {}

		[Test]
		public void AddComponent()
		{
			var entity = new Entity("TestWithComponent");
			var someComponent = new Component();
			entity.Components.Add(someComponent);
			Assert.AreEqual(1, entity.Components.Count);
			entity.Components.Remove(someComponent);
			Assert.AreEqual(0, entity.Components.Count);
		}

		[Test]
		public void Equals()
		{
			var sameEmptyEntity = new Entity("EmptyTest");
			Assert.AreEqual(sameEmptyEntity, emptyEntity);
			var otherEntity = new Entity("SomethingElse");
			Assert.AreNotEqual(otherEntity, emptyEntity);
			Assert.IsTrue(sameEmptyEntity.Equals((object)emptyEntity));
			Assert.AreEqual(emptyEntity.Name.GetHashCode(), emptyEntity.GetHashCode());
		}

		[Test]
		public new void ToString()
		{
			Assert.AreEqual("Entity: EmptyTest, Components=0, BasedOn=", emptyEntity.ToString());
		}

		[Test]
		public void SaveAndLoadFromMemoryStream()
		{
			var entity = new Entity("EntityToSave");
			var data = entity.SaveToMemoryStream();
			var loadedEntity = data.CreateFromMemoryStream<Entity>();
			Assert.AreEqual(loadedEntity.Name, entity.Name);
		}

		[Test]
		public void SaveAndLoadDerivedEntityWithComponent()
		{
			var entity = new Entity("Test", emptyEntity);
			entity.Components.Add(new Component());
			var data = entity.SaveToMemoryStream();
			var loadedEntity = data.CreateFromMemoryStream<Entity>();
			Assert.AreEqual(loadedEntity.Name, entity.Name);
			Assert.AreEqual(1, loadedEntity.Components.Count);
			Assert.AreEqual(emptyEntity, loadedEntity.BasedOn);
		}
	}
}