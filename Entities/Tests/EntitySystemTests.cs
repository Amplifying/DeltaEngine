using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class EntitySystemTests
	{
		[SetUp]
		public void CreateSystem()
		{
			entitySystem = new TestEntitySystem<CountEntities>();
		}

		private EntitySystem entitySystem;

		[Test]
		public void AddToEntitySystemActivatesEntity()
		{
			var entity = new EntityTests.EmptyEntity();
			Assert.IsFalse(entity.IsActive);
			entitySystem.Add(entity);
			Assert.IsTrue(entity.IsActive);
		}

		[Test]
		public void AddAndRemoveFromEntitySystem()
		{
			var entity = new EntityTests.EmptyEntity();
			entity.Add<CountEntities>();
			entitySystem.Add(entity);
			Assert.IsTrue(entity.IsActive);
			entitySystem.Remove(entity);
			Assert.IsFalse(entity.IsActive);
		}

		[Test]
		public void CallingGetHandlerAgainReturnsACachedCopy()
		{
			var counter = entitySystem.GetHandler<CountEntities>();
			Assert.AreEqual(counter, entitySystem.GetHandler<CountEntities>());
		}

		public class CountEntities : EntityHandler
		{
			public void Handle(List<Entity> activeEntities)
			{
				NumberOfEntitiesCounted = activeEntities.Count;
			}

			public int NumberOfEntitiesCounted { get; private set; }

			public EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.First; }
			}
		}

		[Test]
		public void CanCheckEntityHandlersInformation()
		{
			var counter = entitySystem.GetHandler<CountEntities>();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
			Assert.AreEqual(EntityHandlerPriority.First, counter.Priority);
		}

		[Test]
		public void CallingGetUnresolvableHandlerFails()
		{
			Assert.Throws<EntitySystem.UnableToResolveEntityHandler>(
				() => entitySystem.GetHandler<EntityHandler>());
		}

		[Test]
		public void AddingTheSameEntityWithoutHandlerTwiceToTheEntitySystemIsOk()
		{
			var entity = new EntityTests.EmptyEntity();
			entitySystem.Add(entity);
			Assert.DoesNotThrow(() => entitySystem.Add(entity));
		}

		[Test]
		public void AddingTheSameEntityWithHandlerTwiceToTheEntitySystemIsOk()
		{
			var entity = new EntityTests.EmptyEntity();
			entity.Add<CountEntities>();
			entitySystem.Add(entity);
			Assert.DoesNotThrow(() => entitySystem.Add(entity));
		}

		[Test]
		public void AddHandler()
		{
			entitySystem.Add(new EntityTests.EmptyEntity().Add<CountEntities>());
			entitySystem.Add(new EntityTests.EmptyEntity().Add<CountEntities>());
			entitySystem.Add(new EntityTests.EmptyEntity());
			entitySystem.Run();
			Assert.AreEqual(2, entitySystem.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void AddEntityAndAttachHandlerLater()
		{
			var entity = new EntityTests.EmptyEntity();
			entitySystem.Add(entity);
			entitySystem.Run();
			var counter = entitySystem.GetHandler<CountEntities>();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
			entity.Add<CountEntities>();
			entitySystem.Run();
			Assert.AreEqual(1, counter.NumberOfEntitiesCounted);
			entity.Remove<CountEntities>();
			entitySystem.Run();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingTheSameHandlerTwiceDoesNothing()
		{
			var entity = new EntityTests.EmptyEntity();
			entity.Add<CountEntities>();
			entity.Add<CountEntities>();
			entitySystem.Add(entity);
			entitySystem.Run();
			Assert.AreEqual(1, entitySystem.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingTwoHandlersInOneCall()
		{
			var entity = new EntityTests.EmptyEntity();
			entity.Add<CountEntities, CountEntities>();
			entitySystem.Add(entity);
			entitySystem.Run();
			Assert.AreEqual(1, entitySystem.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingThreeHandlersInOneCall()
		{
			var entity = new EntityTests.EmptyEntity().Add<CountEntities, CountEntities, CountEntities>();
			entitySystem.Add(entity);
			entitySystem.Run();
			Assert.AreEqual(1, entitySystem.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingAndRemovingTheSameHandlerDoesNothing()
		{
			var entity = new EntityTests.EmptyEntity();
			entity.Add<CountEntities>();
			entity.Remove<CountEntities>();
			entitySystem.Add(entity);
			entitySystem.Run();
			Assert.AreEqual(0, entitySystem.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void DisabledEntityDoesntDoAnything()
		{
			var entity = new EntityTests.EmptyEntity().Add<CountEntities>();
			entitySystem.Add(entity);
			entitySystem.Run();
			var counter = entitySystem.GetHandler<CountEntities>();
			Assert.AreEqual(1, counter.NumberOfEntitiesCounted);
			entity.IsActive = false;
			entitySystem.Run();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
		}

		[Test]
		public void GetAllEntitiesWithCertainTag()
		{
			entitySystem.Add(new EntityTests.EmptyEntity { Tag = "test1" });
			entitySystem.Add(new EntityTests.EmptyEntity { Tag = "test1" });
			Assert.AreEqual(0, entitySystem.GetEntitiesWithTag("abc").Count);
			Assert.AreEqual(2, entitySystem.GetEntitiesWithTag("test1").Count);
		}

		[Test]
		public void GetAllEntitiesWithHandlersThatHaveATag()
		{
			entitySystem.Add(new EntityTests.EmptyEntity { Tag = "abc" }.Add<CountEntities>());
			entitySystem.Add(new EntityTests.EmptyEntity { Tag = "abc" }.Add<CountEntities>());
			Assert.AreEqual(2, entitySystem.GetEntitiesWithTag("abc").Count);
		}

		[Test]
		public void CreateAndRemoveEntitiesInEntityHandler()
		{
			var system = new TestEntityCreatorSystem();
			system.Add(new EntityTests.EmptyEntity().Add<EntityCreator>());
			Assert.AreEqual(1, system.NumberOfEntities);
			system.Run();
			Assert.AreEqual(2, system.NumberOfEntities);
			system.Run();
			Assert.AreEqual(3, system.NumberOfEntities);
			system.Run();
			Assert.AreEqual(2, system.NumberOfEntities);
		}

		public class EntityCreator : EntityHandler
		{
			public EntityCreator(EntitySystem entitySystem)
			{
				this.entitySystem = entitySystem;
			}

			private readonly EntitySystem entitySystem;

			public void Handle(List<Entity> activeEntities)
			{
				if (entitySystem.NumberOfEntities < 3)
					entitySystem.Add(new EntityTests.EmptyEntity().Add<CountEntities>());
				else
					foreach (var entity in activeEntities)
						entitySystem.Remove(entity);
			}

			public EntityHandlerPriority Priority
			{
				get { return EntityHandlerPriority.Low; }
			}
		}

		public class TestEntityCreatorSystem : EntitySystem
		{
			public TestEntityCreatorSystem()
				: base(new TestHandlerResolver())
			{
				remEntitySystem = this;
			}

			private static EntitySystem remEntitySystem;

			private class TestHandlerResolver : EntityHandlerResolver
			{
				public EntityHandler Resolve(Type handlerType)
				{
					if (handlerType == typeof(CountEntities))
						return new CountEntities();
					return new EntityCreator(remEntitySystem);
				}
			}
		}
	}
}