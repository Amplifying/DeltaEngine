using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Entities
{
	public class EntityTests
	{
		[SetUp]
		public void InitializeEntitiesRunner()
		{
			entities = new MockEntitiesRunner(typeof(MockUpdateBehavior), typeof(ComponentTests.Rotate));
			emptyEntity = new MockEntity();
		}

		private MockEntitiesRunner entities;
		private Entity emptyEntity;

		[TearDown]
		public void DisposeEntitiesRunner()
		{
			entities.Dispose();
		}

		[Test]
		public void DeactivateAndActivateEntity()
		{
			emptyEntity.Start<MockUpdateBehavior>();
			emptyEntity.IsActive = false;
			Assert.IsFalse(emptyEntity.IsActive);
			Assert.AreEqual(0, entities.GetEntitiesOfType<MockEntity>().Count);
			emptyEntity.IsActive = true;
			Assert.IsTrue(emptyEntity.IsActive);
			Assert.IsTrue(emptyEntity.ContainsBehavior<MockUpdateBehavior>());
			Assert.AreEqual(1, entities.GetEntitiesOfType<MockEntity>().Count);
		}

		[Test]
		public void CheckNameAndDefaultValues()
		{
			Assert.AreEqual(0, emptyEntity.NumberOfComponents);
			Assert.IsTrue(emptyEntity.IsActive);
		}

		[Test]
		public void AddAndRemoveComponent()
		{
			Assert.AreEqual(1, entities.NumberOfEntities);
			var entity = new MockEntity().Add(new object());
			Assert.AreEqual(2, entities.NumberOfEntities);
			Assert.AreEqual(1, entity.NumberOfComponents);
			Assert.IsNotNull(entity.Get<object>());
			entity.Remove<object>();
			Assert.AreEqual(0, entity.NumberOfComponents);
			Assert.IsFalse(entity.Contains<object>());
			Assert.Throws<ArgumentNullException>(() => new MockEntity().Add<object>(null));
		}

		[Test]
		public void ContainsComponentsThatHaveBeenAdded()
		{
			var entity = new MockEntity().Add(1.0f).Add("hello").Add(Rectangle.Zero);
			Assert.IsTrue(entity.Contains<float>());
			Assert.IsTrue(entity.Contains<string>());
			Assert.IsTrue(entity.Contains<Rectangle>());
			Assert.IsFalse(entity.Contains<int>());
		}

		[Test]
		public void ToStringWithTags()
		{
			Assert.AreEqual("MockEntity", emptyEntity.ToString());
			emptyEntity.AddTag("Empty");
			emptyEntity.AddTag("Empty");
			emptyEntity.AddTag("Entity");
			Assert.AreEqual("MockEntity Tags=Empty, Entity", emptyEntity.ToString());
			emptyEntity.RemoveTag("Entity");
			Assert.AreEqual("MockEntity Tags=Empty", emptyEntity.ToString());
			emptyEntity.ClearTags();
			Assert.AreEqual("MockEntity", emptyEntity.ToString());
		}

		[Test]
		public void ToStringWithComponentAndList()
		{
			var activeEntity = new MockEntity { IsActive = false };
			Assert.AreEqual("<Inactive> MockEntity", activeEntity.ToString());
			var entityWithComponent = new MockEntity().Add(new object()).Add(new Point(1, 2));
			Assert.AreEqual("MockEntity: Object, Point=1, 2", entityWithComponent.ToString());
			var entityWithList = new MockEntity().Add(new List<Color>());
			Assert.AreEqual("MockEntity: List<Color>", entityWithList.ToString());
		}

		[Test]
		public void ToStringWithArrayAndBehavior()
		{
			var entityWithArray = new MockEntity().Add(new Point[2]);
			Assert.AreEqual("MockEntity: Point[]", entityWithArray.ToString());
			var entityWithRunner =
				new MockEntity().Start<MockUpdateBehavior>().Start<ComponentTests.Rotate>();
			Assert.AreEqual("MockEntity [MockUpdateBehavior, Rotate]", entityWithRunner.ToString());
		}

		[Test]
		public void ChangeUpdatePriority()
		{
			var entityWithUpdate = new UpdateableEntity().Add(1);
			Assert.AreEqual(1, entityWithUpdate.Get<int>());
			Assert.AreEqual(1, entities.GetEntitiesOfType<UpdateableEntity>().Count);
			entityWithUpdate.UpdatePriority = Priority.High;
			entities.RunEntities();
			Assert.AreEqual(2, entityWithUpdate.Get<int>());
			Assert.AreEqual(1, entities.GetEntitiesOfType<UpdateableEntity>().Count);
		}

		private class UpdateableEntity : Entity, Updateable
		{
			public void Update()
			{
				Set(1 + Get<int>());
			}
		}

		[Test]
		public void SaveAndLoadEmptyEntityFromMemoryStream()
		{
			var entity = new MockEntity();
			var data = BinaryDataExtensions.SaveToMemoryStream(entity);
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(GetShortNameLength("MockEntity") + ListLength * 3 + BooleanByte * 2 + 4,
				savedBytes.Length);
			var loadedEntity = data.CreateFromMemoryStream() as Entity;
			Assert.AreEqual(0, loadedEntity.NumberOfComponents);
			Assert.IsTrue(loadedEntity.IsActive);
		}

		private static int GetShortNameLength(string text)
		{
			const int StringLengthByte = 1;
			return StringLengthByte + text.Length;
		}

		private const int ListLength = 1 + BooleanByte;
		private const int BooleanByte = 1;

		[Test]
		public void SaveAndLoadEntityWithOneHandlerFromMemoryStream()
		{
			var entity = new MockEntity().Start<MockUpdateBehavior>();
			entity.AddTag("ABC");
			var data = BinaryDataExtensions.SaveToMemoryStream(entity);
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(56, savedBytes.Length);
			var loadedEntity = data.CreateFromMemoryStream() as Entity;
			Assert.IsTrue(loadedEntity.ContainsTag("ABC"));
			Assert.AreEqual(0, loadedEntity.NumberOfComponents);
			Assert.IsTrue(entity.ContainsBehavior<MockUpdateBehavior>());
			Assert.IsTrue(loadedEntity.IsActive);
		}

		[Test]
		public void SaveAndLoadEntityWithTwoComponentsFromMemoryStream()
		{
			var entity = new MockEntity().Add(1).Add(0.1f);
			entity.AddTag("ABC");
			var data = BinaryDataExtensions.SaveToMemoryStream(entity);
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(59, savedBytes.Length);
			var loadedEntity = data.CreateFromMemoryStream() as Entity;
			Assert.IsTrue(loadedEntity.ContainsTag("ABC"));
			Assert.AreEqual(2, loadedEntity.NumberOfComponents);
			Assert.AreEqual(1, entity.Get<int>());
			Assert.AreEqual(0.1f, entity.Get<float>());
			Assert.IsTrue(loadedEntity.IsActive);
		}

		[Test]
		public void GetAndSetComponent()
		{
			var entity = new MockEntity();
			entity.Set(Color.Red);
			Assert.AreEqual(Color.Red, entity.Get<Color>());
			entity.Set(Color.Green);
			Assert.AreEqual(Color.Green, entity.Get<Color>());
		}

		[Test]
		public void CreateAndGetComponent()
		{
			var entity = new MockEntity();
			Assert.AreEqual(new Color(), entity.GetOrDefault(new Color()));
			entity.Set(Color.Red);
			Assert.AreEqual(Color.Red, entity.GetOrDefault(new Color()));
		}

		[Test]
		public void GettingComponentThatDoesNotExistFails()
		{
			Assert.Throws<Entity.ComponentNotFound>(() => emptyEntity.Get<Point>());
		}

		[Test]
		public void AddingInstantiatedHandlerThrowsException()
		{
			Assert.Throws<Entity.InstantiatedHandlerAddedToEntity>(
				() => new MockEntity().Add(new MockUpdateBehavior()));
		}

		[Test]
		public void AddingComponentOfTheSameTypeTwiceErrors()
		{
			var entity = new MockEntity().Add(Size.Zero);
			Assert.Throws<Entity.ComponentOfTheSameTypeAddedMoreThanOnce>(() => entity.Add(Size.One));
			entity.Remove<Size>();
			entity.Add(Size.One);
		}

		[Test]
		public void StartAndStopBehavior()
		{
			emptyEntity.Start<MockUpdateBehavior>();
			Assert.IsTrue(emptyEntity.ContainsBehavior<MockUpdateBehavior>());
			Assert.AreEqual(1, entities.GetEntitiesOfType<MockEntity>().Count);
			emptyEntity.Stop<MockUpdateBehavior>();
			Assert.IsFalse(emptyEntity.ContainsBehavior<MockUpdateBehavior>());
			Assert.AreEqual(1, entities.GetEntitiesOfType<MockEntity>().Count);
			emptyEntity.Stop<MockUpdateBehavior>();
		}

		[Test]
		public void AddTwoTags()
		{
			var entity = CreateEntityWithTwoTags();
			Assert.IsTrue(entity.ContainsTag(Tag1));
			Assert.IsTrue(entity.ContainsTag(Tag2));
			var entitiesWithTag1 = entities.GetEntitiesWithTag(Tag1);
			Assert.AreEqual(1, entitiesWithTag1.Count);
			Assert.AreEqual(entity, entitiesWithTag1[0]);
		}

		private static MockEntity CreateEntityWithTwoTags()
		{
			var entity = new MockEntity();
			entity.AddTag(Tag1);
			entity.AddTag(Tag2);
			return entity;
		}

		private const string Tag1 = "Tag1";
		private const string Tag2 = "Tag2";

		[Test]
		public void AddingSameTagAgainDoesNothing()
		{
			var entity = CreateEntityWithTwoTags();
			entity.AddTag(Tag1);
			Assert.AreEqual(1, entities.GetEntitiesWithTag(Tag1).Count);
		}

		[Test]
		public void RemoveTag()
		{
			var entity = CreateEntityWithTwoTags();
			entity.RemoveTag(Tag1);
			Assert.IsFalse(entity.ContainsTag(Tag1));
			Assert.IsTrue(entity.ContainsTag(Tag2));
			Assert.AreEqual(0, entities.GetEntitiesWithTag(Tag1).Count);
		}

		[Test]
		public void ClearTags()
		{
				var entity = CreateEntityWithTwoTags();
				entity.ClearTags();
				Assert.IsFalse(entity.ContainsTag(Tag1));
				Assert.IsFalse(entity.ContainsTag(Tag2));
				Assert.AreEqual(0, entities.GetEntitiesWithTag(Tag1).Count);
		}

		[Test]
		public void InactivatingEntityClearsTags()
		{
				var entity = CreateEntityWithTwoTags();
				entity.IsActive = false;
				Assert.AreEqual(0, entities.GetEntitiesWithTag(Tag1).Count);
		}

		[Test]
		public void ReactivatingEntityRestoresTags()
		{
				var entity = CreateEntityWithTwoTags();
				entity.IsActive = false;
				entity.IsActive = true;
				Assert.AreEqual(1, entities.GetEntitiesWithTag(Tag1).Count);
		}

		[Test]
		public void TagsAddedWhileInactiveTakeEffectAfterReactivation()
		{
			var entity = new MockEntity { IsActive = false };
			entity.AddTag(Tag1);
			Assert.AreEqual(0, entities.GetEntitiesWithTag(Tag1).Count);
			entity.IsActive = true;
			Assert.AreEqual(1, entities.GetEntitiesWithTag(Tag1).Count);
		}
	}
}