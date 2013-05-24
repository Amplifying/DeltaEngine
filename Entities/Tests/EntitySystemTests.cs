using System;
using System.Collections.Generic;
using DeltaEngine.Platforms.All;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class EntitySystemTests : TestWithAllFrameworks
	{
		[SetUp]
		public void CreateSystem()
		{
			EntitySystem.Use(new TestEntitySystem<CountEntities, EmptyHandler>());
		}

		[Test]
		public void EntityIsCreatedActiveAndAutomaticallyAddedToEntitySystem()
		{
			var entity = new EmptyEntity();
			Assert.IsTrue(entity.IsActive);
			Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
		}

		[Test]
		public void InactivateAndReactivateEntity()
		{
			var entity = new EmptyEntity { IsActive = false };
			Assert.IsFalse(entity.IsActive);
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			entity.IsActive = true;
			Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
		}

		[Test]
		public void ClearEntities()
		{
			new EmptyEntity();
			new EmptyEntity().Add<CountEntities>();
			EntitySystem.Current.Run();
			Assert.AreEqual(2, EntitySystem.Current.NumberOfEntities);
			EntitySystem.Current.Clear();
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
		}

		[Test]
		public void CallingGetHandlerAgainReturnsACachedCopy()
		{
			var counter = EntitySystem.Current.GetHandler<CountEntities>();
			Assert.AreEqual(counter, EntitySystem.Current.GetHandler<CountEntities>());
		}

		public class CountEntities : EntityHandler
		{
			public override void Handle(List<Entity> activeEntities)
			{
				NumberOfEntitiesCounted = activeEntities.Count;
			}

			public int NumberOfEntitiesCounted { get; private set; }
		}

		[Test]
		public void CanCheckEntityHandlersInformation()
		{
			var counter = EntitySystem.Current.GetHandler<CountEntities>();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
			Assert.AreEqual(EntityHandlerPriority.Normal, counter.Priority);
		}

		[Test]
		public void CallingGetUnresolvableHandlerFails()
		{
			Assert.Throws<EntitySystem.UnableToResolveEntityHandler>(
				() => EntitySystem.Current.GetHandler<EntityHandler>());
		}

		[Test]
		public void AddingTheSameEntityTwiceIsNotOk()
		{
			var entity1 = new EmptyEntity();
			var entity2 = new EmptyEntity().Add<CountEntities>();
			EntitySystem.Current.Run();
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity1));
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity2));
			var entity3 = new EmptyEntity();
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity3));
		}

		[Test]
		public void AddHandler()
		{
			new EmptyEntity().Add<CountEntities>();
			new EmptyEntity().Add<CountEntities>();
			new EmptyEntity();
			EntitySystem.Current.Run();
			Assert.AreEqual(2, EntitySystem.Current.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingHandlerTwiceIsIgnored()
		{
			var entity = new EmptyEntity().Add<CountEntities>();
			EntitySystem.Current.Run();
			entity.Add<CountEntities>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, EntitySystem.Current.GetHandler<CountEntities>().NumberOfEntitiesCounted);
			Assert.AreEqual(1,
				EntitySystem.Current.GetEntitiesByHandler(EntitySystem.Current.GetHandler<CountEntities>()).
										Count);
		}

		[Test]
		public void AddEntityAndAttachHandlerLater()
		{
			var entity = new EmptyEntity();
			EntitySystem.Current.Run();
			var counter = EntitySystem.Current.GetHandler<CountEntities>();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
			entity.Add<CountEntities>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, counter.NumberOfEntitiesCounted);
			entity.Remove<CountEntities>();
			EntitySystem.Current.Run();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingTwoHandlersInOneCall()
		{
			var entity = new EmptyEntity();
			entity.Add<CountEntities, EmptyHandler>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, EntitySystem.Current.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingThreeHandlersInOneCall()
		{
			new EmptyEntity().Add<CountEntities, CountEntities, EmptyHandler>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, EntitySystem.Current.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void AddingAndRemovingTheSameHandlerDoesNothing()
		{
			var entity = new EmptyEntity();
			entity.Add<CountEntities>();
			entity.Remove<CountEntities>();
			EntitySystem.Current.Run();
			Assert.AreEqual(0, EntitySystem.Current.GetHandler<CountEntities>().NumberOfEntitiesCounted);
		}

		[Test]
		public void DisabledEntityDoesntDoAnything()
		{
			var entity = new EmptyEntity().Add<CountEntities>();
			EntitySystem.Current.Run();
			var counter = EntitySystem.Current.GetHandler<CountEntities>();
			Assert.AreEqual(1, counter.NumberOfEntitiesCounted);
			entity.IsActive = false;
			EntitySystem.Current.Run();
			Assert.AreEqual(0, counter.NumberOfEntitiesCounted);
		}

		[Test]
		public void GetAllEntitiesWithCertainTag()
		{
			new EmptyEntity { Tag = "test1" };
			new EmptyEntity { Tag = "test1" };
			Assert.AreEqual(0, EntitySystem.Current.GetEntitiesWithTag("abc").Count);
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesWithTag("test1").Count);
		}

		[Test]
		public void GetAllEntitiesWithHandlersThatHaveATag()
		{
			new EmptyEntity { Tag = "abc" }.Add<CountEntities>();
			new EmptyEntity { Tag = "abc" }.Add<CountEntities>();
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesWithTag("abc").Count);
		}

		[Test]
		public void GetEntitiesByHandlerReturnsEmptyListIfHandlerNeverInstantiated()
		{
			new EmptyEntity();
			EntitySystem.Current.Run();
			Assert.AreEqual(new List<Entity>(),
				EntitySystem.Current.GetEntitiesByHandler(new CountEntities()));
		}

		[Test]
		public void GetEntitiesByHandlerReturnsHandlersEntities()
		{
			new EmptyEntity().Add<CountEntities>();
			var entity = new EmptyEntity().Add<CountEntities>();
			new EmptyEntity().Add<EmptyHandler>();
			EntitySystem.Current.Run();
			EntityHandler handler = EntitySystem.Current.GetHandler<CountEntities>();
			List<Entity> entities = EntitySystem.Current.GetEntitiesByHandler(handler);
			Assert.AreEqual(2, entities.Count);
			Assert.AreEqual(entity, entities[1]);
		}

		[Test]
		public void CreateAndRemoveEntitiesInEntityHandler()
		{
			var system = new TestEntityCreatorSystem();
			system.Add(new EmptyEntity().Add<EntityCreator>());
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

			public override void Handle(List<Entity> activeEntities)
			{
				if (entitySystem.NumberOfEntities < 3)
					entitySystem.Add(new EmptyEntity().Add<CountEntities>());
				else
					foreach (var entity in activeEntities)
						entitySystem.Remove(entity);
			}
		}

		private class TestEntityCreatorSystem : EntitySystem
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

		[Test]
		public void ResolvesCorrectEntityHandler()
		{
			new EmptyEntity().Add<EmptyHandler>();
			var handler = EntitySystem.Current.GetHandler<EmptyHandler>();
			Assert.IsTrue(handler.GetType() == typeof(EmptyHandler));
		}

		public class EmptyHandler : EntityHandler
		{
			public override void Handle(List<Entity> entities) {}
		}

		public class DerivedHandler : EmptyHandler {}
	}
}