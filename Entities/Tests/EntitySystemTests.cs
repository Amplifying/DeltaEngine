using System;
using System.Collections.Generic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Entities.Tests
{
	public class EntitySystemTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateSystem()
		{
			EntitySystem.Use(new TestEntitySystem<EmptyHandler, IncrementCounter, DerivedHandler>());
		}

		public class EmptyHandler : Behavior<Entity>
		{
			internal override void Handle(Entity entity) {}
		}

		public class IncrementCounter : Behavior<Entity>
		{
			internal override void StartedHandling(Entity entity)
			{
				entity.Set("IncrementCounter Is Running");
			}

			internal override void Handle(Entity entity)
			{
				entity.Set(entity.Get<int>() + 1);
			}

			internal override void StoppedHandling(Entity entity)
			{
				entity.Set("IncrementCounter Is Not Running");
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
		public void InactivateEntity()
		{
			var entity = new EmptyEntity();
			bool isInactivated = false;
			entity.Inactivated += () => isInactivated = true;
			entity.IsActive = false;
			Assert.IsFalse(entity.IsActive);
			Assert.AreEqual(0, EntitySystem.Current.NumberOfEntities);
			Assert.IsTrue(isInactivated);
		}

		[Test]
		public void ActivateEntity()
		{
			var entity = new EmptyEntity { IsActive = false };
			bool isActivated = false;
			entity.Activated += () => isActivated = true;
			entity.IsActive = true;
			Assert.AreEqual(1, EntitySystem.Current.NumberOfEntities);
			Assert.IsTrue(isActivated);
		}

		[Test]
		public void ClearEntities()
		{
			new EmptyEntity();
			new EmptyEntity().Start<EmptyHandler>();
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
			Assert.AreEqual(Priority.Normal, handler.Priority);
		}

		[Test]
		public void CallingGetUnresolvableHandlerFails()
		{
			Assert.Throws<EntitySystem.UnableToResolveEntityHandler>(
				() => EntitySystem.Current.GetHandler<Handler>());
		}

		[Test]
		public void AddingTheSameEntityTwiceIsNotOk()
		{
			var entity1 = new EmptyEntity();
			var entity2 = new EmptyEntity().Start<EmptyHandler>();
			EntitySystem.Current.Run();
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity1));
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity2));
			var entity3 = new EmptyEntity();
			Assert.Throws<EntitySystem.EntityAlreadyAdded>(() => EntitySystem.Current.Add(entity3));
		}

		[Test]
		public void AddHandler()
		{
			new EmptyEntity().Start<EmptyHandler>();
			new EmptyEntity().Start<EmptyHandler>();
			new EmptyEntity();
			EntitySystem.Current.Run();
			var handler = EntitySystem.Current.GetHandler<EmptyHandler>();
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesByHandler(handler).Count);
		}

		[Test]
		public void AddingHandlerTwiceIsIgnored()
		{
			var entity = new EmptyEntity().Start<IncrementCounter>().Add(0);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
			entity.Start<IncrementCounter>();
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
			entity.Start<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
			entity.Stop<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void AddingTwoHandlersInOneCall()
		{
			var entity = new EmptyEntity().Add(0);
			entity.Start<IncrementCounter, EmptyHandler>();
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void AddingThreeHandlersInOneCall()
		{
			var entity = new EmptyEntity().Start<IncrementCounter, DerivedHandler, EmptyHandler>().Add(0);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void AddingAndRemovingTheSameHandlerDoesNothing()
		{
			var entity = new EmptyEntity().Add(0);
			entity.Start<IncrementCounter>();
			entity.Stop<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual(0, entity.Get<int>());
		}

		[Test]
		public void DisabledEntityDoesntDoAnything()
		{
			var entity = new EmptyEntity().Start<IncrementCounter>().Add(0);
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
			entity.IsActive = false;
			EntitySystem.Current.Run();
			Assert.AreEqual(1, entity.Get<int>());
		}

		[Test]
		public void GetAllEntitiesWithCertainTag()
		{
			new EmptyEntity().AddTag("test1");
			new EmptyEntity().AddTag("test1"); 
			Assert.AreEqual(0, EntitySystem.Current.GetEntitiesWithTag("abc").Count);
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesWithTag("test1").Count);
		}

		[Test]
		public void GetAllEntitiesWithHandlersThatHaveATag()
		{
			new EmptyEntity().Start<IncrementCounter>().AddTag("abc");
			new EmptyEntity().Start<IncrementCounter>().AddTag("abc");
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesWithTag("abc").Count);
		}

		[Test]
		public void GetAllEntitiesOfCertainType()
		{
			new EmptyEntity();
			new EmptyEntity();
			Assert.AreEqual(2, EntitySystem.Current.GetEntitiesOfType<EmptyEntity>().Count);
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
			new EmptyEntity().Start<IncrementCounter>().Add(0);
			var entity = new EmptyEntity().Start<IncrementCounter>().Add(0);
			new EmptyEntity().Start<EmptyHandler>();
			EntitySystem.Current.Run();
			Handler handler = EntitySystem.Current.GetHandler<IncrementCounter>();
			List<Entity> entities = EntitySystem.Current.GetEntitiesByHandler(handler);
			Assert.AreEqual(2, entities.Count);
			Assert.AreEqual(entity, entities[1]);
		}

		[Test]
		public void CreateAndRemoveEntitiesInEntityHandler()
		{
			var system = new TestEntityCreatorSystem();
			system.Add(new EmptyEntity().Start<EntityCreator>());
			Assert.AreEqual(1, system.NumberOfEntities);
			system.Run();
			Assert.AreEqual(2, system.NumberOfEntities);
			system.Run();
			Assert.AreEqual(3, system.NumberOfEntities);
			system.Run();
			Assert.AreEqual(2, system.NumberOfEntities);
		}

		public class EntityCreator : Behavior<Entity>
		{
			public EntityCreator(EntitySystem entitySystem)
			{
				this.entitySystem = entitySystem;
			}

			private readonly EntitySystem entitySystem;

			internal override void Handle(Entity entity)
			{
				if (entitySystem.NumberOfEntities < 3)
					entitySystem.Add(new EmptyEntity().Start<IncrementCounter>().Add(0));
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

			private class TestHandlerResolver : HandlerResolver
			{
				public Handler Resolve(Type handlerType)
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
			new EmptyEntity().Start<EmptyHandler>();
			var handler = EntitySystem.Current.GetHandler<EmptyHandler>();
			Assert.IsTrue(handler.GetType() == typeof(EmptyHandler));
		}

		[Test]
		public void UnorderedEntityHandlerProcessesEntitiesInTheOrderAdded()
		{
			EntitySystem.Use(new TestEntitySystem<UnorderedHandler>());
			var entity1 = new EmptyEntity().Start<UnorderedHandler>();
			var entity2 = new EmptyEntity().Start<UnorderedHandler>();
			var entity3 = new EmptyEntity().Start<UnorderedHandler>();
			EntitySystem.Current.Run();
			var start = entity1.Get<int>();
			Assert.AreEqual(start + 1, entity2.Get<int>());
			Assert.AreEqual(start + 2, entity3.Get<int>());
		}

		private class UnorderedHandler : BatchedBehavior<Entity>
		{
			internal override void Handle(IEnumerable<Entity> entities)
			{
				int position = 0;
				foreach (Entity entity in entities)
					entity.Add(++position);
			}
		}

		[Test]
		public void OrderedEntityHandlerProcessesEntitiesInTheRequiredSequence()
		{
			EntitySystem.Use(new TestEntitySystem<OrderedHandler>());
			var entity1 = new EmptyEntity().Start<OrderedHandler>().Add(3.0f);
			var entity2 = new EmptyEntity().Start<OrderedHandler>().Add(1.0f);
			var entity3 = new EmptyEntity().Start<OrderedHandler>().Add(2.0f);
			EntitySystem.Current.Run();
			var start = entity2.Get<int>();
			Assert.AreEqual(start + 1, entity3.Get<int>());
			Assert.AreEqual(start + 2, entity1.Get<int>());
		}

		private class OrderedHandler : BatchedBehavior<Entity>
		{
			public OrderedHandler()
			{
				Order = entity => entity.Get<float>();
			}

			internal override void Handle(IEnumerable<Entity> entities)
			{
				int position = 0;
				foreach (Entity entity in entities)
					entity.Add(++position);
			}
		}

		[Test]
		public void SelectingEntityHandlerProcessesEntitiesThatPassTheSelectionCriteria()
		{
			EntitySystem.Use(new TestEntitySystem<SelectingHandler>());
			var entity1 = new EmptyEntity().Start<SelectingHandler>().Add(3.0f);
			var entity2 = new EmptyEntity().Start<SelectingHandler>().Add(-1.0f);
			var entity3 = new EmptyEntity().Start<SelectingHandler>().Add(2.0f);
			EntitySystem.Current.Run();
			var start = entity1.Get<int>();
			Assert.AreEqual(start + 1, entity3.Get<int>());
			Assert.IsFalse(entity2.Contains<int>());
		}

		private class SelectingHandler : BatchedBehavior<Entity>
		{
			public SelectingHandler()
			{
				Filter = entity => entity.Get<float>() > 0.0f;
			}

			internal override void Handle(IEnumerable<Entity> entities)
			{
				int position = 0;
				foreach (Entity entity in entities)
					entity.Add(++position);
			}
		}

		[Test]
		public void SelectingAndOrderingHandlerBothSelectsAndOrders()
		{
			EntitySystem.Use(new TestEntitySystem<SelectingOrderingHandler>());
			var entity1 = new EmptyEntity().Start<SelectingOrderingHandler>().Add(3.0f);
			var entity2 = new EmptyEntity().Start<SelectingOrderingHandler>().Add(-1.0f);
			var entity3 = new EmptyEntity().Start<SelectingOrderingHandler>().Add(2.0f);
			EntitySystem.Current.Run();
			var start = entity3.Get<int>();
			Assert.AreEqual(start + 1, entity1.Get<int>());
			Assert.IsFalse(entity2.Contains<int>());
		}

		private class SelectingOrderingHandler : BatchedBehavior<Entity>
		{
			public SelectingOrderingHandler()
			{
				Filter = entity => entity.Get<float>() > 0.0f;
				Order = entity => entity.Get<float>();
			}

			internal override void Handle(IEnumerable<Entity> entities)
			{
				int position = 0;
				foreach (Entity entity in entities)
					entity.Add(++position);
			}
		}

		[Test]
		public void AddingHandlerExecutesStartingCode()
		{
			var entity = new EmptyEntity().Add(0);
			EntitySystem.Current.Run();
			Assert.IsFalse(entity.Contains<string>());
			entity.Start<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual("IncrementCounter Is Running", entity.Get<string>());
		}

		[Test]
		public void AddingHandlerTwiceOnlyExecutesStartingCodeOnce()
		{
			var entity = new EmptyEntity().Add(0).Start<IncrementCounter>();
			EntitySystem.Current.Run();
			entity.Set("Method Didn't Get Called Again");
			entity.Start<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual("Method Didn't Get Called Again", entity.Get<string>());
		}

		[Test]
		public void RemovingHandlerExecutesStoppingCode()
		{
			var entity = new EmptyEntity().Add(0).Start<IncrementCounter>();
			EntitySystem.Current.Run();
			entity.Stop<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual("IncrementCounter Is Not Running", entity.Get<string>());
		}

		[Test]
		public void RemovingHandlerTwiceOnlyExecutesStoppingCodeOnce()
		{
			var entity = new EmptyEntity().Add(0).Start<IncrementCounter>();
			EntitySystem.Current.Run();
			entity.Stop<IncrementCounter>();
			EntitySystem.Current.Run();
			entity.Set("Method Didn't Get Called Again");
			entity.Stop<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.AreEqual("Method Didn't Get Called Again", entity.Get<string>());
		}

		[Test]
		public void RemovingHandlerWhenNeverAddedExecutesNoCode()
		{
			var entity = new EmptyEntity();
			EntitySystem.Current.Run();
			entity.Stop<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.IsFalse(entity.Contains<string>());
		}

		[Test]
		public void AddingAndRemovingHandlerInTheSameFrameExecutesNoCode()
		{
			var entity = new EmptyEntity().Start<IncrementCounter>().Stop<IncrementCounter>();
			EntitySystem.Current.Run();
			Assert.IsFalse(entity.Contains<string>());
		}
	}
}