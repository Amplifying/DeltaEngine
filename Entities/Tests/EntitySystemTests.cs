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
			EntitySystem.Use(new TestEntitySystem<EmptyHandler, IncrementCounter, DerivedHandler>());
		}

		public class EmptyHandler : EntityHandler
		{
			public override void Handle(Entity entity) {}
		}

		public class IncrementCounter : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				entity.Set(entity.Get<int>() + 1);
			}
		}

		public class DerivedHandler : EmptyHandler {}

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
			new EmptyEntity().Add<EmptyHandler>();
			EntitySystem.Current.Run();
			Assert.AreEqual(2, EntitySystem.Current.NumberOfEntities);
			EntitySystem.Current.Clear();
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
		}

		[Test]
		public void CallingGetHandlerAgainReturnsACachedCopy()
		{
			var handler = EntitySystem.Current.GetHandler<EmptyHandler>();
			Assert.AreEqual(handler, EntitySystem.Current.GetHandler<EmptyHandler>());
		}

		[Test]
		public void CanCheckEntityHandlersInformation()
		{
			var handler = EntitySystem.Current.GetHandler<EmptyHandler>();
			Assert.AreEqual(EntityHandlerPriority.Normal, handler.Priority);
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
			var entity2 = new EmptyEntity().Add<EmptyHandler>();
			EntitySystem.Current.Run();
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity1));
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity2));
			var entity3 = new EmptyEntity();
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity3));
		}

		[Test]
		public void AddHandler()
		{
			new EmptyEntity().Add<EmptyHandler>();
			new EmptyEntity().Add<EmptyHandler>();
			new EmptyEntity();
			EntitySystem.Current.Run();
			var handler = EntitySystem.Current.GetHandler<EmptyHandler>();
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesByHandler(handler).Count);
		}

		[Test]
		public void AddingHandlerTwiceIsIgnored()
		{
			var entity = new EmptyEntity().Add<IncrementCounter>().Add(0);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
			entity.Add<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual(2, entity.Get<int>());
			Assert.AreEqual(1,
				EntitySystem.Current.GetEntitiesByHandler(
					EntitySystem.Current.GetHandler<IncrementCounter>()).Count);
		}

		[Test]
		public void AddEntityAndAttachHandlerLater()
		{
			var entity = new EmptyEntity().Add(0);
			EntitySystem.Current.Run();
			Assert.AreEqual(0, entity.Get<int>());
			entity.Add<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
			entity.Remove<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void AddingTwoHandlersInOneCall()
		{
			var entity = new EmptyEntity().Add(0);
			entity.Add<IncrementCounter, EmptyHandler>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void AddingThreeHandlersInOneCall()
		{
			var entity = new EmptyEntity().Add<IncrementCounter, DerivedHandler, EmptyHandler>().Add(0);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void AddingAndRemovingTheSameHandlerDoesNothing()
		{
			var entity = new EmptyEntity().Add(0);
			entity.Add<IncrementCounter>();
			entity.Remove<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual(0, entity.Get<int>());
		}

		[Test]
		public void DisabledEntityDoesntDoAnything()
		{
			var entity = new EmptyEntity().Add<IncrementCounter>().Add(0);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
			entity.IsActive = false;
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
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
			new EmptyEntity { Tag = "abc" }.Add<IncrementCounter>();
			new EmptyEntity { Tag = "abc" }.Add<IncrementCounter>();
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesWithTag("abc").Count);
		}

		[Test]
		public void GetEntitiesByHandlerReturnsEmptyListIfHandlerNeverInstantiated()
		{
			new EmptyEntity();
			EntitySystem.Current.Run();
			Assert.AreEqual(new List<Entity>(),
				EntitySystem.Current.GetEntitiesByHandler(new IncrementCounter()));
		}

		[Test]
		public void GetEntitiesByHandlerReturnsHandlersEntities()
		{
			new EmptyEntity().Add<IncrementCounter>().Add(0);
			var entity = new EmptyEntity().Add<IncrementCounter>().Add(0);
			new EmptyEntity().Add<EmptyHandler>();
			EntitySystem.Current.Run();
			EntityHandler handler = EntitySystem.Current.GetHandler<IncrementCounter>();
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

			public override void Handle(Entity entity)
			{
				if (entitySystem.NumberOfEntities < 3)
					entitySystem.Add(new EmptyEntity().Add<IncrementCounter>().Add(0));
				else
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
					if (handlerType == typeof(IncrementCounter))
						return new IncrementCounter();

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

		[Test]
		public void UnorderedEntityHandlerProcessesEntitiesInTheOrderAdded()
		{
			EntitySystem.Use(new TestEntitySystem<UnorderedHandler>());
			var entity1 = new EmptyEntity().Add<UnorderedHandler>();
			var entity2 = new EmptyEntity().Add<UnorderedHandler>();
			var entity3 = new EmptyEntity().Add<UnorderedHandler>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity1.Get<int>());
			Assert.AreEqual(2, entity2.Get<int>());
			Assert.AreEqual(3, entity3.Get<int>());
		}

		private class UnorderedHandler : EntityHandler
		{
			public override void Handle(Entity entity)
			{
				entity.Add(++position);
			}

			private static int position;
		}

		[Test]
		public void OrderedEntityHandlerProcessesEntitiesInTheRequiredSequence()
		{
			EntitySystem.Use(new TestEntitySystem<OrderedHandler>());
			var entity1 = new EmptyEntity().Add<OrderedHandler>().Add(3.0f);
			var entity2 = new EmptyEntity().Add<OrderedHandler>().Add(1.0f);
			var entity3 = new EmptyEntity().Add<OrderedHandler>().Add(2.0f);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity2.Get<int>());
			Assert.AreEqual(2, entity3.Get<int>());
			Assert.AreEqual(3, entity1.Get<int>());
		}

		private class OrderedHandler : EntityHandler
		{
			public OrderedHandler()
			{
				Order = entity => entity.Get<float>();
			}

			public override void Handle(Entity entity)
			{
				entity.Add(++position);
			}

			private static int position;
		}

		[Test]
		public void SelectingEntityHandlerProcessesEntitiesThatPassTheSelectionCriteria()
		{
			EntitySystem.Use(new TestEntitySystem<SelectingHandler>());
			var entity1 = new EmptyEntity().Add<SelectingHandler>().Add(3.0f);
			var entity2 = new EmptyEntity().Add<SelectingHandler>().Add(-1.0f);
			var entity3 = new EmptyEntity().Add<SelectingHandler>().Add(2.0f);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity1.Get<int>());
			Assert.AreEqual(2, entity3.Get<int>());
			Assert.IsFalse(entity2.Contains<int>());
		}

		private class SelectingHandler : EntityHandler
		{
			public SelectingHandler()
			{
				Filter = entity => entity.Get<float>() > 0.0f;
			}

			public override void Handle(Entity entity)
			{
				entity.Add(++position);
			}

			private static int position;
		}

		[Test]
		public void SelectingAndOrderingHandlerBothSelectsAndOrders()
		{
			EntitySystem.Use(new TestEntitySystem<SelectingOrderingHandler>());
			var entity1 = new EmptyEntity().Add<SelectingOrderingHandler>().Add(3.0f);
			var entity2 = new EmptyEntity().Add<SelectingOrderingHandler>().Add(-1.0f);
			var entity3 = new EmptyEntity().Add<SelectingOrderingHandler>().Add(2.0f);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity3.Get<int>());
			Assert.AreEqual(2, entity1.Get<int>());
			Assert.IsFalse(entity2.Contains<int>());
		}

		private class SelectingOrderingHandler : EntityHandler
		{
			public SelectingOrderingHandler()
			{
				Filter = entity => entity.Get<float>() > 0.0f;
				Order = entity => entity.Get<float>();
			}

			public override void Handle(Entity entity)
			{
				entity.Add(++position);
			}

			private static int position;
		}
	}
}