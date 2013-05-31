using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class EntityTests
	{
		[Test]
		public void CheckNameAndDefaultValues()
		{
			Assert.AreEqual("Empty", emptyEntity.Tag);
			Assert.AreEqual(0, emptyEntity.NumberOfComponents);
			Assert.IsTrue(emptyEntity.IsActive);
		}

		private readonly Entity emptyEntity = new EmptyEntity { Tag = "Empty" };

		[Test]
		public void AddAndRemoveComponent()
		{
			var entity = new EmptyEntity().Add(new object());
			Assert.AreEqual(1, entity.NumberOfComponents);
			Assert.IsNotNull(entity.Get<object>());
			entity.Remove<object>();
			Assert.AreEqual(0, entity.NumberOfComponents);
			Assert.IsFalse(entity.Contains<object>());
			Assert.Throws<ArgumentNullException>(() => new EmptyEntity().Add<object>(null));
		}

		[Test]
		public void ContainsComponentsThatHaveBeenAdded()
		{
			var entity = new EmptyEntity().Add(1.0f).Add("hello").Add(Rectangle.Zero);
			Assert.IsTrue(entity.Contains<float>());
			Assert.IsTrue(entity.Contains<float, string>());
			Assert.IsTrue(entity.Contains<float, string, Rectangle>());
			Assert.IsFalse(entity.Contains<int>());
			Assert.IsFalse(entity.Contains<string, int>());
			Assert.IsFalse(entity.Contains<Point, string, Rectangle>());
		}

		[Test]
		public new void ToString()
		{
			Assert.AreEqual("EmptyEntity Tag=Empty", emptyEntity.ToString());
			var activeEntity = new EmptyEntity { IsActive = false };
			Assert.AreEqual("<Inactive> EmptyEntity", activeEntity.ToString());
			var entityWithComponent = new EmptyEntity().Add(new object()).Add(new Point());
			Assert.AreEqual("EmptyEntity: Object, Point", entityWithComponent.ToString());
			var entityWithList = new EmptyEntity().Add(new List<Color>());
			Assert.AreEqual("EmptyEntity: List<Color>", entityWithList.ToString());
			var entityWithArray = new EmptyEntity().Add(new Point[2]);
			Assert.AreEqual("EmptyEntity: Point[]", entityWithArray.ToString());
			var entityWithRunner =
				new EmptyEntity().Add<EntitySystemTests.CountEntities>().Add<ComponentTests.Rotate>();
			Assert.AreEqual("EmptyEntity [CountEntities, Rotate]", entityWithRunner.ToString());
		}

		[Test]
		public void SaveAndLoadEmptyEntityFromMemoryStream()
		{
			var entity = new EmptyEntity();
			var data = entity.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(
				GetShortNameLength("EmptyEntity") + ListLength * 4 + IsTagUsedBoolean + IsActiveBoolean * 2 +
					NullMessagedEvent, savedBytes.Length);
			var loadedEntity = data.CreateFromMemoryStream() as Entity;
			Assert.AreEqual(entity.Tag, loadedEntity.Tag);
			Assert.AreEqual(0, loadedEntity.NumberOfComponents);
			Assert.IsTrue(loadedEntity.IsActive);
		}

		private static int GetShortNameLength(string text)
		{
			const int StringLengthByte = 1;
			return StringLengthByte + text.Length;
		}

		private const int ListLength = 4 + BooleanByte;
		private const int BooleanByte = 1;
		private const int IsTagUsedBoolean = BooleanByte;
		private const int IsActiveBoolean = BooleanByte;
		private const int NullMessagedEvent = BooleanByte;

		[Test]
		public void SaveAndLoadEntityWithOneHandlerFromMemoryStream()
		{
			var entity = new EmptyEntity().Add<EntitySystemTests.CountEntities>();
			entity.Tag = "ABC";
			var data = entity.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(41, savedBytes.Length);
			var loadedEntity = data.CreateFromMemoryStream() as Entity;
			Assert.AreEqual(entity.Tag, loadedEntity.Tag);
			Assert.AreEqual(0, loadedEntity.NumberOfComponents);
			Assert.AreEqual(1, entity.handlerTypesToAdd.Count);
			Assert.AreEqual(typeof(EntitySystemTests.CountEntities), entity.handlerTypesToAdd[0]);
			Assert.IsTrue(loadedEntity.IsActive);
		}

		[Test]
		public void SaveAndLoadEntityWithTwoComponentsFromMemoryStream()
		{
			var entity = new EmptyEntity().Add(1).Add(0.1f);
			entity.Tag = "ABC";
			var data = entity.SaveToMemoryStream();
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(62, savedBytes.Length);
			var loadedEntity = data.CreateFromMemoryStream() as Entity;
			Assert.AreEqual(entity.Tag, loadedEntity.Tag);
			Assert.AreEqual(2, loadedEntity.NumberOfComponents);
			Assert.AreEqual(0, entity.handlerTypesToAdd.Count);
			Assert.AreEqual(1, entity.components[0]);
			Assert.AreEqual(0.1f, entity.components[1]);
			Assert.IsTrue(loadedEntity.IsActive);
		}

		[Test]
		public void GetAndSetComponent()
		{
			var entity = new EmptyEntity();
			entity.AddOrSet(Color.Red);
			Assert.AreEqual(Color.Red, entity.Get<Color>());
			entity.AddOrSet(Color.Green);
			Assert.AreEqual(Color.Green, entity.Get<Color>());
		}

		[Test]
		public void CreateAndGetComponent()
		{
			var entity = new EmptyEntity();
			Assert.AreEqual(new Color(), entity.GetOrCreate<Color>());
			entity.Set(Color.Red);
			Assert.AreEqual(Color.Red, entity.GetOrCreate<Color>());
		}

		[Test]
		public void GettingComponentThatDoesNotExistFails()
		{
			Assert.Throws<Entity.ComponentNotFound>(() => emptyEntity.Get<Point>());
		}

		[Test]
		public void SettingComponentThatDoesNotExistFails()
		{
			Assert.Throws<Entity.ComponentNotFound>(() => emptyEntity.Set(new Point(5, 5)));
		}

		[Test]
		public void AddingInstantiatedHandlerThrowsException()
		{
			Assert.Throws<Entity.InstantiatedEntityHandlerAddedToEntity>(
				() => new EmptyEntity().Add(new EntitySystemTests.CountEntities()));
		}

		[Test]
		public void AddingComponentOfTheSameTypeTwiceErrors()
		{
			var entity = new EmptyEntity().Add(Size.Zero);
			Assert.Throws<Entity.ComponentOfTheSameTypeAddedMoreThanOnce>(() => entity.Add(Size.One));
			entity.Remove<Size>();
			entity.Add(Size.One);
		}

		[Test]
		public void AddingTriggerAddsComponentAndHandler()
		{
			var trigger = CreateTrigger();
			var entity = new EmptyEntity().AddTrigger(trigger);
			Assert.AreEqual(trigger, entity.Get<List<Trigger>>()[0]);
			Assert.IsTrue(entity.handlerTypesToAdd[0] == typeof(CheckTriggers));
		}

		private static Trigger CreateTrigger()
		{
			var trigger = new Trigger(entity => entity.Contains<Color>());
			trigger.Fired += entity => entity.Set(Color.Red);
			return trigger;
		}

		[Test]
		public void RemovingTriggerRemovesItFromComponentList()
		{
			var trigger = CreateTrigger();
			var entity = new EmptyEntity().AddTrigger(trigger).RemoveTrigger(trigger);
			Assert.AreEqual(0, entity.Get<List<Trigger>>().Count);
		}

		[Test]
		public void RemovingNonExistentTriggerIsOk()
		{
			new EmptyEntity().RemoveTrigger(CreateTrigger());
		}

		[Test]
		public void AddingTriggerTwiceIsOk()
		{
			var trigger = CreateTrigger();
			new EmptyEntity().AddTrigger(trigger).AddTrigger(trigger);
		}

		[Test]
		public void ActivationTransfersToComponentsIfComponentsAreEntities()
		{
			var grandchild = new EmptyEntity();
			var child = new EmptyEntity().Add(grandchild);
			var parent = new EmptyEntity().Add(child);
			Assert.IsTrue(grandchild.IsActive);
			parent.IsActive = false;
			Assert.IsFalse(grandchild.IsActive);
			parent.IsActive = true;
			Assert.IsTrue(grandchild.IsActive);
		}
	}
}